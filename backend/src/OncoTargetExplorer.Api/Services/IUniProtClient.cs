using OncoTargetExplorer.Api.Models.UniProt;

namespace OncoTargetExplorer.Api.Services;

public interface IUniProtClient
{
    Task<IReadOnlyList<UniProtEntry>> SearchAsync(string query, CancellationToken ct = default);

    Task<UniProtEntry?> GetByAccessionAsync(string accession, CancellationToken ct = default);
}
