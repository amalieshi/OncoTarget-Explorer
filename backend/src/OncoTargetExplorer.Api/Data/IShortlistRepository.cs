namespace OncoTargetExplorer.Api.Data;

public interface IShortlistRepository
{
    Task<IReadOnlyList<ShortlistItem>> GetAllAsync(CancellationToken ct = default);

    Task<bool> ExistsAsync(string accession, CancellationToken ct = default);

    Task AddAsync(ShortlistItem item, CancellationToken ct = default);

    Task<bool> RemoveAsync(string accession, CancellationToken ct = default);
}
