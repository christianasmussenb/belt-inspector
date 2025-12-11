using BeltInspector.Api.Data;
using BeltInspector.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeltInspector.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InspectionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inspection>>> GetInspections(CancellationToken cancellationToken)
    {
        var inspections = await _context.Inspections
            .Include(x => x.Files)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(inspections);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Inspection>> GetInspection(Guid id, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(x => x.Files)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (inspection is null)
        {
            return NotFound();
        }

        return Ok(inspection);
    }

    [HttpPost]
    public async Task<ActionResult<Inspection>> CreateInspection([FromBody] CreateInspectionRequest request, CancellationToken cancellationToken)
    {
        var inspection = new Inspection
        {
            Title = request.Title,
            Description = request.Description,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "pending" : request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetInspection), new { id = inspection.Id }, inspection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInspection(Guid id, [FromBody] UpdateInspectionRequest request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (inspection is null)
        {
            return NotFound();
        }

        inspection.Title = request.Title ?? inspection.Title;
        inspection.Description = request.Description ?? inspection.Description;
        inspection.Status = request.Status ?? inspection.Status;
        inspection.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public record CreateInspectionRequest(string Title, string? Description, string? Status);
public record UpdateInspectionRequest(string? Title, string? Description, string? Status);
