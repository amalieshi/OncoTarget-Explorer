using Microsoft.AspNetCore.Mvc;
using OncoTargetExplorer.Api.Models;
using OncoTargetExplorer.Api.Services;

namespace OncoTargetExplorer.Api.Controllers;

[ApiController]
[Route("api/proteins")]
public class ProteinsController(IProteinService proteinService, ILogger<ProteinsController> logger) : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<ProteinSummaryDto>>> Search([FromQuery] string? query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query must not be empty.");
        }

        try
        {
            var results = await proteinService.SearchAsync(query, ct);
            return Ok(results);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "UniProt search failed for query {Query}", query);
            return StatusCode(StatusCodes.Status502BadGateway, "The upstream data provider is unavailable.");
        }
    }

    [HttpGet("{accession}")]
    public async Task<ActionResult<ProteinDetailDto>> GetByAccession(string accession, CancellationToken ct)
    {
        try
        {
            var detail = await proteinService.GetDetailAsync(accession, ct);
            return detail is null ? NotFound() : Ok(detail);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "UniProt lookup failed for accession {Accession}", accession);
            return StatusCode(StatusCodes.Status502BadGateway, "The upstream data provider is unavailable.");
        }
    }
}
