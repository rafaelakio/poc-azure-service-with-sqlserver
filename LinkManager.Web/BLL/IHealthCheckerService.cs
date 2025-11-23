using LinkManager.Web.Models;

namespace LinkManager.Web.BLL;

/// <summary>
/// Interface para o serviço de verificação de saúde de links.
/// </summary>
public interface IHealthCheckerService
{
    /// <summary>
    /// Verifica a saúde de um link e extrai metadados.
    /// </summary>
    /// <param name="url">URL a ser verificada</param>
    /// <returns>Resultado da verificação com metadados</returns>
    Task<HealthCheckResult> CheckHealthAsync(string url);

    /// <summary>
    /// Verifica a saúde de um PageLink existente e atualiza seus dados.
    /// </summary>
    /// <param name="pageLink">Link a ser verificado</param>
    /// <returns>PageLink atualizado</returns>
    Task<PageLink> CheckAndUpdateAsync(PageLink pageLink);
}

/// <summary>
/// Resultado da verificação de saúde de um link.
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Indica se o link está acessível.
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Código HTTP retornado.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Tempo de resposta em milissegundos.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Título extraído do HTML.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Descrição extraída dos meta tags.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mensagem de erro (se houver).
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Data e hora da verificação.
    /// </summary>
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
