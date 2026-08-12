using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OncoTargetExplorer.Api.Controllers;
using OncoTargetExplorer.Api.Data;
using OncoTargetExplorer.Api.Models;
using Xunit;

namespace OncoTargetExplorer.Api.Tests;

public class ShortlistControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _dbContext;
    private readonly ShortlistController _controller;

    public ShortlistControllerTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _controller = new ShortlistController(new ShortlistRepository(_dbContext));
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    private static readonly ShortlistCreateRequest SampleRequest =
        new("P04626", "ERBB2", "Receptor tyrosine-protein kinase erbB-2");

    [Fact]
    public async Task Add_ThenGetAll_RoundTripsTheItem()
    {
        await _controller.Add(SampleRequest, CancellationToken.None);

        var result = await _controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IReadOnlyList<ShortlistItemDto>>(ok.Value);
        var item = Assert.Single(items);
        Assert.Equal("P04626", item.Accession);
    }

    [Fact]
    public async Task Add_ReturnsConflict_WhenAccessionAlreadyShortlisted()
    {
        await _controller.Add(SampleRequest, CancellationToken.None);

        var result = await _controller.Add(SampleRequest, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenAccessionIsBlank()
    {
        var result = await _controller.Add(new ShortlistCreateRequest("", "ERBB2", "erbB-2"), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Remove_ReturnsNoContent_WhenItemExisted()
    {
        await _controller.Add(SampleRequest, CancellationToken.None);

        var result = await _controller.Remove("P04626", CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsNotFound_WhenItemDidNotExist()
    {
        var result = await _controller.Remove("UNKNOWN", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
