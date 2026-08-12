using OncoTargetExplorer.Api.Models;

namespace OncoTargetExplorer.Api.Services;

public interface IProteinService
{
    Task<IReadOnlyList<ProteinSummaryDto>> SearchAsync(string query, CancellationToken ct = default);

    Task<ProteinDetailDto?> GetDetailAsync(string accession, CancellationToken ct = default);
}
