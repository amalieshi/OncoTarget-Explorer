using Microsoft.AspNetCore.Mvc;
using OncoTargetExplorer.Api.Data;
using OncoTargetExplorer.Api.Models;

namespace OncoTargetExplorer.Api.Controllers;

[ApiController]
[Route("api/shortlist")]
public class ShortlistController(IShortlistRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ShortlistItemDto>>> GetAll(CancellationToken ct)
    {
        var items = await repository.GetAllAsync(ct);
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ShortlistItemDto>> Add(ShortlistCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Accession))
        {
            return BadRequest("Accession must not be empty.");
        }

        if (await repository.ExistsAsync(request.Accession, ct))
        {
            return Conflict("This protein is already on the shortlist.");
        }

        var item = new ShortlistItem
        {
            Accession = request.Accession,
            GeneName = request.GeneName,
            ProteinName = request.ProteinName,
            AddedAtUtc = DateTime.UtcNow,
        };
        await repository.AddAsync(item, ct);

        return CreatedAtAction(nameof(GetAll), null, ToDto(item));
    }

    [HttpDelete("{accession}")]
    public async Task<IActionResult> Remove(string accession, CancellationToken ct)
    {
        var removed = await repository.RemoveAsync(accession, ct);
        return removed ? NoContent() : NotFound();
    }

    private static ShortlistItemDto ToDto(ShortlistItem item) =>
        new(item.Accession, item.GeneName, item.ProteinName, item.AddedAtUtc);
}
