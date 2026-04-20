using System.Net;
using LinkManager.Web.BLL;
using LinkManager.Web.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace LinkManager.Tests;

public class HealthCheckerServiceTests
{
    private static HealthCheckerService CreateService(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        var logger = new Mock<ILogger<HealthCheckerService>>();
        return new HealthCheckerService(httpClientFactory.Object, logger.Object);
    }

    private static HttpMessageHandler BuildHandler(HttpStatusCode status, string content)
    {
        var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = status,
                Content = new StringContent(content),
                ReasonPhrase = status.ToString()
            });
        return mockHandler.Object;
    }

    [Fact]
    public async Task CheckHealthAsync_WhenSuccessResponse_ReturnsHealthyAndExtractsMetadata()
    {
        var html = "<html><head><title>Exemplo</title><meta name=\"description\" content=\"Descricao exemplo\" /></head><body></body></html>";
        var service = CreateService(BuildHandler(HttpStatusCode.OK, html));

        var result = await service.CheckHealthAsync("https://example.com");

        Assert.True(result.IsHealthy);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Exemplo", result.Title);
        Assert.Equal("Descricao exemplo", result.Description);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenNotFound_ReturnsNotHealthyWithErrorMessage()
    {
        var service = CreateService(BuildHandler(HttpStatusCode.NotFound, string.Empty));

        var result = await service.CheckHealthAsync("https://example.com/404");

        Assert.False(result.IsHealthy);
        Assert.Equal(404, result.StatusCode);
        Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
    }

    [Fact]
    public async Task CheckHealthAsync_WhenTimeout_ReturnsErrorMessageTimeout()
    {
        var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("timeout"));

        var service = CreateService(mockHandler.Object);

        var result = await service.CheckHealthAsync("https://slow.example.com");

        Assert.False(result.IsHealthy);
        Assert.Contains("Timeout", result.ErrorMessage);
    }

    [Fact]
    public async Task CheckAndUpdateAsync_WhenOnline_UpdatesPageLinkStatus()
    {
        var html = "<html><head><title>Title</title></head></html>";
        var service = CreateService(BuildHandler(HttpStatusCode.OK, html));
        var link = new PageLink { Url = "https://example.com", Status = LinkStatus.Pending };

        var updated = await service.CheckAndUpdateAsync(link);

        Assert.Equal(LinkStatus.Online, updated.Status);
        Assert.Equal(200, updated.HttpStatusCode);
        Assert.NotNull(updated.LastCheckedAt);
    }

    [Fact]
    public async Task CheckAndUpdateAsync_WhenServerError_SetsStatusOffline()
    {
        var service = CreateService(BuildHandler(HttpStatusCode.InternalServerError, string.Empty));
        var link = new PageLink { Url = "https://example.com/500", Status = LinkStatus.Pending };

        var updated = await service.CheckAndUpdateAsync(link);

        Assert.NotEqual(LinkStatus.Online, updated.Status);
        Assert.Equal(500, updated.HttpStatusCode);
    }
}
