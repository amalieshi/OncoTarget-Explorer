namespace OncoTargetExplorer.Api.Models;

public record ShortlistItemDto(
    string Accession,
    string GeneName,
    string ProteinName,
    DateTime AddedAtUtc);

public record ShortlistCreateRequest(
    string Accession,
    string GeneName,
    string ProteinName);
