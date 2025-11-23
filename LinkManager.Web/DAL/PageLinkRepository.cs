using Microsoft.EntityFrameworkCore;
using LinkManager.Web.Models;

namespace LinkManager.Web.DAL;

/// <summary>
/// Implementação do repositório para operações de dados de PageLink.
/// Utiliza Entity Framework Core para acesso ao banco de dados.
/// </summary>
public class PageLinkRepository : IPageLinkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PageLinkRepository> _logger;

    public PageLinkRepository(
        ApplicationDbContext context,
        ILogger<PageLinkRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os links ordenados por data de criação (mais recentes primeiro).
    /// </summary>
    public async Task<List<PageLink>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os links");
        
        return await _context.PageLinks
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtém um link específico por ID.
    /// </summary>
    public async Task<PageLink?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Buscando link com ID: {Id}", id);
        
        return await _context.PageLinks
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    /// <summary>
    /// Obtém um link por URL (útil para evitar duplicatas).
    /// </summary>
    public async Task<PageLink?> GetByUrlAsync(string url)
    {
        _logger.LogInformation("Buscando link com URL: {Url}", url);
        
        return await _context.PageLinks
            .FirstOrDefaultAsync(p => p.Url == url && p.IsActive);
    }

    /// <summary>
    /// Adiciona um novo link ao banco de dados.
    /// </summary>
    public async Task<PageLink> AddAsync(PageLink pageLink)
    {
        _logger.LogInformation("Adicionando novo link: {Url}", pageLink.Url);

        // Verifica se já existe um link com a mesma URL
        var existing = await GetByUrlAsync(pageLink.Url);
        if (existing != null)
        {
            throw new InvalidOperationException($"Já existe um link com a URL: {pageLink.Url}");
        }

        pageLink.CreatedAt = DateTime.UtcNow;
        pageLink.IsActive = true;

        _context.PageLinks.Add(pageLink);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Link adicionado com sucesso. ID: {Id}", pageLink.Id);

        return pageLink;
    }

    /// <summary>
    /// Atualiza um link existente.
    /// </summary>
    public async Task<PageLink> UpdateAsync(PageLink pageLink)
    {
        _logger.LogInformation("Atualizando link ID: {Id}", pageLink.Id);

        var existing = await _context.PageLinks.FindAsync(pageLink.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Link com ID {pageLink.Id} não encontrado");
        }

        // Atualiza propriedades
        existing.Url = pageLink.Url;
        existing.Title = pageLink.Title;
        existing.Description = pageLink.Description;
        existing.Status = pageLink.Status;
        existing.HttpStatusCode = pageLink.HttpStatusCode;
        existing.ResponseTimeMs = pageLink.ResponseTimeMs;
        existing.LastCheckedAt = pageLink.LastCheckedAt;
        existing.ErrorMessage = pageLink.ErrorMessage;
        existing.Category = pageLink.Category;
        existing.Notes = pageLink.Notes;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Link atualizado com sucesso. ID: {Id}", pageLink.Id);

        return existing;
    }

    /// <summary>
    /// Remove um link (soft delete - marca como inativo).
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Removendo link ID: {Id}", id);

        var pageLink = await _context.PageLinks.FindAsync(id);
        if (pageLink == null)
        {
            throw new InvalidOperationException($"Link com ID {id} não encontrado");
        }

        // Soft delete - apenas marca como inativo
        pageLink.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Link removido com sucesso. ID: {Id}", id);
    }

    /// <summary>
    /// Busca links por categoria.
    /// </summary>
    public async Task<List<PageLink>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation("Buscando links da categoria: {Category}", category);

        return await _context.PageLinks
            .Where(p => p.IsActive && p.Category == category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Busca links por status.
    /// </summary>
    public async Task<List<PageLink>> GetByStatusAsync(string status)
    {
        _logger.LogInformation("Buscando links com status: {Status}", status);

        return await _context.PageLinks
            .Where(p => p.IsActive && p.Status == status)
            .OrderByDescending(p => p.LastCheckedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtém links que precisam de verificação (não verificados há X horas).
    /// </summary>
    public async Task<List<PageLink>> GetLinksNeedingCheckAsync(int hoursThreshold = 24)
    {
        _logger.LogInformation(
            "Buscando links que precisam de verificação (threshold: {Hours}h)",
            hoursThreshold);

        var threshold = DateTime.UtcNow.AddHours(-hoursThreshold);

        return await _context.PageLinks
            .Where(p => p.IsActive && 
                   (p.LastCheckedAt == null || p.LastCheckedAt < threshold))
            .OrderBy(p => p.LastCheckedAt)
            .ToListAsync();
    }
}
