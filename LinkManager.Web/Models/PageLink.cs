using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkManager.Web.Models;

/// <summary>
/// Modelo que representa um link de página gerenciado pelo sistema.
/// </summary>
public class PageLink
{
    /// <summary>
    /// Identificador único do link.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// URL completa da página.
    /// </summary>
    [Required(ErrorMessage = "A URL é obrigatória")]
    [Url(ErrorMessage = "URL inválida")]
    [MaxLength(2000)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Título da página extraído do HTML.
    /// </summary>
    [MaxLength(500)]
    public string? Title { get; set; }

    /// <summary>
    /// Descrição da página extraída dos meta tags.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Status atual do link (Online, Offline, etc).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Código HTTP retornado na última verificação.
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// Tempo de resposta em milissegundos.
    /// </summary>
    public long? ResponseTimeMs { get; set; }

    /// <summary>
    /// Data e hora da criação do registro.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora da última verificação de saúde.
    /// </summary>
    public DateTime? LastCheckedAt { get; set; }

    /// <summary>
    /// Mensagem de erro da última verificação (se houver).
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Categoria ou tag do link.
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Notas adicionais sobre o link.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Indica se o link está ativo no sistema.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Enumeração dos possíveis status de um link.
/// </summary>
public static class LinkStatus
{
    public const string Pending = "Pending";
    public const string Online = "Online";
    public const string Offline = "Offline";
    public const string Error = "Error";
    public const string Timeout = "Timeout";
}
