using LinkManager.Web.DAL;
using LinkManager.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LinkManager.Tests;

public class PageLinkRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PageLinkRepository _repository;

    public PageLinkRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        var logger = new Mock<ILogger<PageLinkRepository>>();
        _repository = new PageLinkRepository(_context, logger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_AddsNewLink()
    {
        var link = new PageLink { Url = "https://new-link.example", Status = LinkStatus.Pending };

        var result = await _repository.AddAsync(link);

        Assert.True(result.Id > 0);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task AddAsync_WhenUrlDuplicated_Throws()
    {
        await _repository.AddAsync(new PageLink { Url = "https://duplicate.example", Status = LinkStatus.Pending });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.AddAsync(new PageLink { Url = "https://duplicate.example", Status = LinkStatus.Pending }));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveLinks()
    {
        var active = await _repository.AddAsync(new PageLink { Url = "https://active.example", Status = LinkStatus.Pending });
        var inactive = await _repository.AddAsync(new PageLink { Url = "https://inactive.example", Status = LinkStatus.Pending });
        await _repository.DeleteAsync(inactive.Id);

        var list = await _repository.GetAllAsync();

        Assert.Contains(list, p => p.Id == active.Id);
        Assert.DoesNotContain(list, p => p.Id == inactive.Id);
    }

    [Fact]
    public async Task DeleteAsync_PerformsSoftDelete()
    {
        var link = await _repository.AddAsync(new PageLink { Url = "https://to-delete.example", Status = LinkStatus.Pending });

        await _repository.DeleteAsync(link.Id);

        var stored = await _context.PageLinks.FindAsync(link.Id);
        Assert.NotNull(stored);
        Assert.False(stored!.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFieldsOfExistingLink()
    {
        var link = await _repository.AddAsync(new PageLink { Url = "https://original.example", Status = LinkStatus.Pending });
        link.Title = "Novo Titulo";
        link.Status = LinkStatus.Online;

        var updated = await _repository.UpdateAsync(link);

        Assert.Equal("Novo Titulo", updated.Title);
        Assert.Equal(LinkStatus.Online, updated.Status);
    }

    [Fact]
    public async Task GetByCategoryAsync_FiltersByCategory()
    {
        await _repository.AddAsync(new PageLink { Url = "https://cat-a.example", Category = "A", Status = LinkStatus.Pending });
        await _repository.AddAsync(new PageLink { Url = "https://cat-b.example", Category = "B", Status = LinkStatus.Pending });

        var results = await _repository.GetByCategoryAsync("A");

        Assert.Single(results);
        Assert.Equal("A", results[0].Category);
    }
}
