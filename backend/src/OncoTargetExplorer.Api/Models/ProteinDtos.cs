namespace OncoTargetExplorer.Api.Models;

public record ProteinSummaryDto(
    string Accession,
    string GeneName,
    string ProteinName,
    string Organism);

public record ProteinDetailDto(
    string Accession,
    string GeneName,
    string ProteinName,
    string Organism,
    string? FunctionSummary,
    IReadOnlyList<string> SubcellularLocations,
    int SequenceLength,
    IReadOnlyList<string> DiseaseAssociations,
    IReadOnlyList<CrossReferenceDto> CrossReferences);

public record CrossReferenceDto(string Database, string Id);
