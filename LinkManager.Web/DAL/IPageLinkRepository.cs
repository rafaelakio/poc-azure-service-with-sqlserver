using LinkManager.Web.Models;

namespace LinkManager.Web.DAL;

/// <summary>
/// Interface do repositório para operações de dados de PageLink.
/// Implementa o padrão Repository para abstração da camada de dados.
/// </summary>
public interface IPageLinkRepository
{
    /// <summary>
    /// Obtém todos os links.
    /// </summary>
    Task<List<PageLink>> GetAllAsync();

    /// <summary>
    /// Obtém um link por ID.
    /// </summary>
    Task<PageLink?> GetByIdAsync(int id);

    /// <summary>
    /// Obtém um link por URL.
    /// </summary>
    Task<PageLink?> GetByUrlAsync(string url);

    /// <summary>
    /// Adiciona um novo link.
    /// </summary>
    Task<PageLink> AddAsync(PageLink pageLink);

    /// <summary>
    /// Atualiza um link existente.
    /// </summary>
    Task<PageLink> UpdateAsync(PageLink pageLink);

    /// <summary>
    /// Remove um link.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Busca links por categoria.
    /// </summary>
    Task<List<PageLink>> GetByCategoryAsync(string category);

    /// <summary>
    /// Busca links por status.
    /// </summary>
    Task<List<PageLink>> GetByStatusAsync(string status);

    /// <summary>
    /// Obtém links que precisam de verificação.
    /// </summary>
    Task<List<PageLink>> GetLinksNeedingCheckAsync(int hoursThreshold = 24);
}
