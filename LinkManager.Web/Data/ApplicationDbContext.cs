using Microsoft.EntityFrameworkCore;
using LinkManager.Web.Models;

namespace LinkManager.Web.Data;

/// <summary>
/// Contexto do banco de dados da aplicação.
/// Gerencia as entidades e configurações do Entity Framework.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Construtor que recebe as opções de configuração do contexto.
    /// </summary>
    /// <param name="options">Opções de configuração do DbContext</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet para a entidade PageLink.
    /// Representa a tabela de links no banco de dados.
    /// </summary>
    public DbSet<PageLink> PageLinks { get; set; } = null!;

    /// <summary>
    /// Configuração do modelo de dados.
    /// Define índices, relacionamentos e restrições.
    /// </summary>
    /// <param name="modelBuilder">Construtor do modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade PageLink
        modelBuilder.Entity<PageLink>(entity =>
        {
            // Define o nome da tabela
            entity.ToTable("PageLinks");

            // Configura a chave primária
            entity.HasKey(e => e.Id);

            // Cria índice na URL para buscas rápidas
            entity.HasIndex(e => e.Url)
                .IsUnique()
                .HasDatabaseName("IX_PageLinks_Url");

            // Cria índice no Status para filtros
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_PageLinks_Status");

            // Cria índice na data de criação
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_PageLinks_CreatedAt");

            // Cria índice na categoria
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("IX_PageLinks_Category");

            // Configura propriedades com valores padrão
            entity.Property(e => e.Status)
                .HasDefaultValue(LinkStatus.Pending);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // Dados iniciais (seed data) para testes
        modelBuilder.Entity<PageLink>().HasData(
            new PageLink
            {
                Id = 1,
                Url = "https://www.google.com",
                Title = "Google",
                Description = "Mecanismo de busca",
                Status = LinkStatus.Pending,
                Category = "Search Engine",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new PageLink
            {
                Id = 2,
                Url = "https://www.github.com",
                Title = "GitHub",
                Description = "Plataforma de desenvolvimento",
                Status = LinkStatus.Pending,
                Category = "Development",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
    }
}
