using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using OncoTargetExplorer.Api.Models.UniProt;

namespace OncoTargetExplorer.Api.Services;

public class UniProtClient(HttpClient httpClient) : IUniProtClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<IReadOnlyList<UniProtEntry>> SearchAsync(string query, CancellationToken ct = default)
    {
        var url = "uniprotkb/search"
            + $"?query={Uri.EscapeDataString(query)}"
            + "&fields=accession,gene_names,protein_name,organism_name"
            + "&format=json&size=25";

        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UniProtSearchResponse>(JsonOptions, ct);
        return result?.Results ?? [];
    }

    public async Task<UniProtEntry?> GetByAccessionAsync(string accession, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"uniprotkb/{Uri.EscapeDataString(accession)}.json", ct);

        // UniProt returns 400 (not 404) for a syntactically invalid accession, and 404 for a
        // well-formed one that doesn't exist. Both mean "no such protein" to our callers.
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UniProtEntry>(JsonOptions, ct);
    }
}
