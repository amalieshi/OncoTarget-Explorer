namespace OncoTargetExplorer.Api.Models.UniProt;

public record UniProtSearchResponse(List<UniProtEntry>? Results);

public record UniProtEntry(
    string PrimaryAccession,
    UniProtOrganism? Organism,
    UniProtProteinDescription? ProteinDescription,
    List<UniProtGene>? Genes,
    List<UniProtComment>? Comments,
    UniProtSequence? Sequence,
    List<UniProtCrossReference>? UniProtKBCrossReferences);

public record UniProtOrganism(string? ScientificName, string? CommonName);

public record UniProtProteinDescription(UniProtRecommendedName? RecommendedName);

public record UniProtRecommendedName(UniProtValue? FullName);

public record UniProtValue(string Value);

public record UniProtGene(UniProtValue? GeneName);

public record UniProtSequence(int Length);

public record UniProtCrossReference(string Database, string Id);

public record UniProtComment(
    string CommentType,
    List<UniProtValue>? Texts,
    UniProtDisease? Disease,
    List<UniProtSubcellularLocationEntry>? SubcellularLocations);

public record UniProtDisease(string? DiseaseId);

public record UniProtSubcellularLocationEntry(UniProtValue? Location);
