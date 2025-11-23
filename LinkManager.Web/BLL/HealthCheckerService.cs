using System.Diagnostics;
using HtmlAgilityPack;
using LinkManager.Web.Models;

namespace LinkManager.Web.BLL;

/// <summary>
/// Serviço responsável por verificar a saúde de links e extrair metadados HTML.
/// </summary>
public class HealthCheckerService : IHealthCheckerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthCheckerService> _logger;

    public HealthCheckerService(
        IHttpClientFactory httpClientFactory,
        ILogger<HealthCheckerService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Verifica a saúde de um link e extrai metadados HTML.
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(string url)
    {
        var result = new HealthCheckResult();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Iniciando verificação de saúde para: {Url}", url);

            // Cria cliente HTTP com timeout
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            // Faz requisição GET
            var response = await httpClient.GetAsync(url);
            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.IsHealthy = response.IsSuccessStatusCode;

            _logger.LogInformation(
                "Resposta recebida: Status={StatusCode}, Tempo={ResponseTime}ms",
                result.StatusCode,
                result.ResponseTimeMs);

            // Se a resposta for bem-sucedida, extrai metadados
            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                ExtractMetadata(html, result);
            }
            else
            {
                result.ErrorMessage = $"HTTP {result.StatusCode}: {response.ReasonPhrase}";
            }
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            result.IsHealthy = false;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = "Timeout: A requisição excedeu o tempo limite";
            _logger.LogWarning(ex, "Timeout ao verificar {Url}", url);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            result.IsHealthy = false;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = $"Erro de rede: {ex.Message}";
            _logger.LogError(ex, "Erro de rede ao verificar {Url}", url);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.IsHealthy = false;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = $"Erro inesperado: {ex.Message}";
            _logger.LogError(ex, "Erro inesperado ao verificar {Url}", url);
        }

        return result;
    }

    /// <summary>
    /// Verifica e atualiza um PageLink existente.
    /// </summary>
    public async Task<PageLink> CheckAndUpdateAsync(PageLink pageLink)
    {
        var healthResult = await CheckHealthAsync(pageLink.Url);

        // Atualiza dados do PageLink com resultado da verificação
        pageLink.HttpStatusCode = healthResult.StatusCode;
        pageLink.ResponseTimeMs = healthResult.ResponseTimeMs;
        pageLink.LastCheckedAt = healthResult.CheckedAt;
        pageLink.ErrorMessage = healthResult.ErrorMessage;

        // Atualiza título e descrição se foram extraídos
        if (!string.IsNullOrEmpty(healthResult.Title))
        {
            pageLink.Title = healthResult.Title;
        }

        if (!string.IsNullOrEmpty(healthResult.Description))
        {
            pageLink.Description = healthResult.Description;
        }

        // Define status baseado no resultado
        pageLink.Status = DetermineStatus(healthResult);

        _logger.LogInformation(
            "Link atualizado: {Url} - Status: {Status}",
            pageLink.Url,
            pageLink.Status);

        return pageLink;
    }

    /// <summary>
    /// Extrai metadados (título e descrição) do HTML.
    /// </summary>
    private void ExtractMetadata(string html, HealthCheckResult result)
    {
        try
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Extrai título
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
            if (titleNode != null)
            {
                result.Title = titleNode.InnerText.Trim();
                _logger.LogDebug("Título extraído: {Title}", result.Title);
            }

            // Extrai descrição do meta tag
            var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode(
                "//meta[@name='description']");
            
            if (descriptionNode != null)
            {
                result.Description = descriptionNode.GetAttributeValue("content", string.Empty).Trim();
                _logger.LogDebug("Descrição extraída: {Description}", result.Description);
            }

            // Tenta meta tag og:description como alternativa
            if (string.IsNullOrEmpty(result.Description))
            {
                var ogDescriptionNode = htmlDoc.DocumentNode.SelectSingleNode(
                    "//meta[@property='og:description']");
                
                if (ogDescriptionNode != null)
                {
                    result.Description = ogDescriptionNode.GetAttributeValue("content", string.Empty).Trim();
                }
            }

            // Tenta meta tag og:title como alternativa
            if (string.IsNullOrEmpty(result.Title))
            {
                var ogTitleNode = htmlDoc.DocumentNode.SelectSingleNode(
                    "//meta[@property='og:title']");
                
                if (ogTitleNode != null)
                {
                    result.Title = ogTitleNode.GetAttributeValue("content", string.Empty).Trim();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao extrair metadados do HTML");
        }
    }

    /// <summary>
    /// Determina o status do link baseado no resultado da verificação.
    /// </summary>
    private string DetermineStatus(HealthCheckResult result)
    {
        if (!result.IsHealthy)
        {
            if (result.ErrorMessage?.Contains("Timeout") == true)
            {
                return LinkStatus.Timeout;
            }
            return LinkStatus.Offline;
        }

        if (result.StatusCode >= 200 && result.StatusCode < 300)
        {
            return LinkStatus.Online;
        }

        if (result.StatusCode >= 400)
        {
            return LinkStatus.Error;
        }

        return LinkStatus.Offline;
    }
}
