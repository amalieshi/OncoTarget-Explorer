using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OncoTargetExplorer.Api.Data;
using Xunit;

namespace OncoTargetExplorer.Api.Tests;

public class ShortlistRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _dbContext;
    private readonly ShortlistRepository _repository;

    public ShortlistRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new ShortlistRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task AddAsync_ThenGetAllAsync_ReturnsTheItem()
    {
        await _repository.AddAsync(new ShortlistItem
        {
            Accession = "P04626",
            GeneName = "ERBB2",
            ProteinName = "Receptor tyrosine-protein kinase erbB-2",
            AddedAtUtc = DateTime.UtcNow,
        });

        var items = await _repository.GetAllAsync();

        var item = Assert.Single(items);
        Assert.Equal("P04626", item.Accession);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenAccessionAlreadyShortlisted()
    {
        await _repository.AddAsync(new ShortlistItem
        {
            Accession = "P04626",
            GeneName = "ERBB2",
            ProteinName = "Receptor tyrosine-protein kinase erbB-2",
            AddedAtUtc = DateTime.UtcNow,
        });

        Assert.True(await _repository.ExistsAsync("P04626"));
        Assert.False(await _repository.ExistsAsync("Q9Y316"));
    }

    [Fact]
    public async Task RemoveAsync_DeletesTheItem_AndReturnsTrue()
    {
        await _repository.AddAsync(new ShortlistItem
        {
            Accession = "P04626",
            GeneName = "ERBB2",
            ProteinName = "Receptor tyrosine-protein kinase erbB-2",
            AddedAtUtc = DateTime.UtcNow,
        });

        var removed = await _repository.RemoveAsync("P04626");

        Assert.True(removed);
        Assert.Empty(await _repository.GetAllAsync());
    }

    [Fact]
    public async Task RemoveAsync_ReturnsFalse_WhenAccessionNotShortlisted()
    {
        var removed = await _repository.RemoveAsync("UNKNOWN");

        Assert.False(removed);
    }

    [Fact]
    public async Task AddAsync_ThrowsOnDuplicateAccession()
    {
        await _repository.AddAsync(new ShortlistItem
        {
            Accession = "P04626",
            GeneName = "ERBB2",
            ProteinName = "Receptor tyrosine-protein kinase erbB-2",
            AddedAtUtc = DateTime.UtcNow,
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(new ShortlistItem
        {
            Accession = "P04626",
            GeneName = "ERBB2",
            ProteinName = "Receptor tyrosine-protein kinase erbB-2",
            AddedAtUtc = DateTime.UtcNow,
        }));
    }
}
