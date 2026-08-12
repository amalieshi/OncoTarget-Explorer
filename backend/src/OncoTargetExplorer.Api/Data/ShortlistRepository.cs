using Microsoft.EntityFrameworkCore;

namespace OncoTargetExplorer.Api.Data;

public class ShortlistRepository(AppDbContext dbContext) : IShortlistRepository
{
    public async Task<IReadOnlyList<ShortlistItem>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.ShortlistItems
            .OrderByDescending(item => item.AddedAtUtc)
            .ToListAsync(ct);

    public Task<bool> ExistsAsync(string accession, CancellationToken ct = default) =>
        dbContext.ShortlistItems.AnyAsync(item => item.Accession == accession, ct);

    public async Task AddAsync(ShortlistItem item, CancellationToken ct = default)
    {
        dbContext.ShortlistItems.Add(item);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveAsync(string accession, CancellationToken ct = default)
    {
        var item = await dbContext.ShortlistItems
            .FirstOrDefaultAsync(x => x.Accession == accession, ct);

        if (item is null)
        {
            return false;
        }

        dbContext.ShortlistItems.Remove(item);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
