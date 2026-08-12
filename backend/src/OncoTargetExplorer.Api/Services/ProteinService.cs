using OncoTargetExplorer.Api.Models;
using OncoTargetExplorer.Api.Models.UniProt;

namespace OncoTargetExplorer.Api.Services;

public class ProteinService(IUniProtClient uniProtClient) : IProteinService
{
    // Cross-reference databases most relevant to identifying and following up on a
    // cancer target, out of the dozens UniProt returns (EMBL clone accessions, etc.).
    private static readonly HashSet<string> RelevantCrossReferenceDatabases =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "HGNC", "GeneID", "RefSeq", "Ensembl", "PDB", "DrugBank", "ChEMBL", "MIM",
        };

    public async Task<IReadOnlyList<ProteinSummaryDto>> SearchAsync(string query, CancellationToken ct = default)
    {
        var entries = await uniProtClient.SearchAsync(query, ct);
        return entries.Select(MapSummary).ToList();
    }

    public async Task<ProteinDetailDto?> GetDetailAsync(string accession, CancellationToken ct = default)
    {
        var entry = await uniProtClient.GetByAccessionAsync(accession, ct);
        return entry is null ? null : MapDetail(entry);
    }

    private static ProteinSummaryDto MapSummary(UniProtEntry entry) => new(
        entry.PrimaryAccession,
        GetGeneName(entry),
        GetProteinName(entry),
        entry.Organism?.ScientificName ?? "Unknown");

    private static ProteinDetailDto MapDetail(UniProtEntry entry)
    {
        var comments = entry.Comments ?? [];

        return new ProteinDetailDto(
            entry.PrimaryAccession,
            GetGeneName(entry),
            GetProteinName(entry),
            entry.Organism?.ScientificName ?? "Unknown",
            GetFunctionSummary(comments),
            GetSubcellularLocations(comments),
            entry.Sequence?.Length ?? 0,
            GetDiseaseAssociations(comments),
            GetCrossReferences(entry));
    }

    private static string GetGeneName(UniProtEntry entry) =>
        entry.Genes?.FirstOrDefault()?.GeneName?.Value ?? "Unknown";

    private static string GetProteinName(UniProtEntry entry) =>
        entry.ProteinDescription?.RecommendedName?.FullName?.Value ?? "Unknown";

    private static string? GetFunctionSummary(IEnumerable<UniProtComment> comments) =>
        comments.FirstOrDefault(c => c.CommentType == "FUNCTION")?.Texts?.FirstOrDefault()?.Value;

    private static IReadOnlyList<string> GetSubcellularLocations(IEnumerable<UniProtComment> comments) =>
        comments
            .Where(c => c.CommentType == "SUBCELLULAR LOCATION")
            .SelectMany(c => c.SubcellularLocations ?? [])
            .Select(l => l.Location?.Value)
            .OfType<string>()
            .Distinct()
            .ToList();

    private static IReadOnlyList<string> GetDiseaseAssociations(IEnumerable<UniProtComment> comments) =>
        comments
            .Where(c => c.CommentType == "DISEASE")
            .Select(c => c.Disease?.DiseaseId)
            .OfType<string>()
            .Distinct()
            .ToList();

    private static IReadOnlyList<CrossReferenceDto> GetCrossReferences(UniProtEntry entry) =>
        (entry.UniProtKBCrossReferences ?? [])
            .Where(xref => RelevantCrossReferenceDatabases.Contains(xref.Database))
            .Select(xref => new CrossReferenceDto(xref.Database, xref.Id))
            .ToList();
}
