using System.Net;
using LinkManager.Web.BLL;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace LinkManager.Tests;

/// <summary>
/// Testes de extração de metadados (title/description) a partir do HTML
/// recebido pelo HealthCheckerService.
/// </summary>
public class MetadataExtractorTests
{
    private static HealthCheckerService CreateService(string html, HttpStatusCode status = HttpStatusCode.OK)
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
                Content = new StringContent(html)
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        var logger = new Mock<ILogger<HealthCheckerService>>();
        return new HealthCheckerService(factory.Object, logger.Object);
    }

    [Fact]
    public async Task ExtractsTitleFromTitleTag()
    {
        var html = "<html><head><title>Minha Pagina</title></head></html>";
        var service = CreateService(html);

        var result = await service.CheckHealthAsync("https://example.com");

        Assert.Equal("Minha Pagina", result.Title);
    }

    [Fact]
    public async Task ExtractsDescriptionFromMetaTag()
    {
        var html = "<html><head><meta name=\"description\" content=\"Uma descricao bonita\" /></head></html>";
        var service = CreateService(html);

        var result = await service.CheckHealthAsync("https://example.com");

        Assert.Equal("Uma descricao bonita", result.Description);
    }

    [Fact]
    public async Task FallsBackToOgTitleAndOgDescription()
    {
        var html = @"<html><head>
<meta property='og:title' content='OG Titulo' />
<meta property='og:description' content='OG Descricao' />
</head></html>";
        var service = CreateService(html);

        var result = await service.CheckHealthAsync("https://example.com");

        Assert.Equal("OG Titulo", result.Title);
        Assert.Equal("OG Descricao", result.Description);
    }

    [Fact]
    public async Task WhenHtmlIsEmpty_TitleAndDescriptionAreNull()
    {
        var service = CreateService(string.Empty);

        var result = await service.CheckHealthAsync("https://example.com");

        Assert.Null(result.Title);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task WhenResponseIsNon2xx_DoesNotExtractMetadata()
    {
        var html = "<html><head><title>Ignore-me</title></head></html>";
        var service = CreateService(html, HttpStatusCode.InternalServerError);

        var result = await service.CheckHealthAsync("https://example.com");

        Assert.False(result.IsHealthy);
        Assert.Null(result.Title);
    }
}
