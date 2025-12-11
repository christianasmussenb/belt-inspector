using BeltInspector.Api.Data;
using BeltInspector.Api.Models;
using BeltInspector.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeltInspector.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public FilesController(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    [HttpPost("{inspectionId:guid}")]
    public async Task<ActionResult<FileRecord>> Upload(Guid inspectionId, IFormFile file, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections.FirstOrDefaultAsync(x => x.Id == inspectionId, cancellationToken);
        if (inspection is null)
        {
            return NotFound(new { message = "Inspection not found" });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "File is required" });
        }

        var storageKey = await _fileStorageService.UploadAsync(file, $"inspections/{inspectionId}", cancellationToken);

        var record = new FileRecord
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            StorageKey = storageKey,
            InspectionId = inspectionId,
        };

        _context.Files.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(Download), new { id = record.Id }, record);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var record = await _context.Files.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        var stream = await _fileStorageService.DownloadAsync(record.StorageKey, cancellationToken);
        if (stream is null)
        {
            return NotFound();
        }

        return File(stream, record.ContentType ?? "application/octet-stream", record.FileName);
    }
}
