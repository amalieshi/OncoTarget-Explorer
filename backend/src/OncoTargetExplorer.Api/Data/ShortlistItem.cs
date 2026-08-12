namespace OncoTargetExplorer.Api.Data;

public class ShortlistItem
{
    public int Id { get; set; }

    public required string Accession { get; set; }

    public required string GeneName { get; set; }

    public required string ProteinName { get; set; }

    public DateTime AddedAtUtc { get; set; }
}
