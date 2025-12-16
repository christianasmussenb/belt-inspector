using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BeltInspector.Api.Models;

public class FileRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string FileName { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    [Required]
    public string StorageKey { get; set; } = string.Empty;

    [MaxLength(500)]
    public string DownloadUrl { get; set; } = string.Empty;

    [Required]
    public Guid InspectionId { get; set; }

    [ForeignKey(nameof(InspectionId))]
    [JsonIgnore] // evita ciclos al serializar
    public Inspection? Inspection { get; set; }
}
