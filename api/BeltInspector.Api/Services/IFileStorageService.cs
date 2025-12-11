using Microsoft.AspNetCore.Http;

namespace BeltInspector.Api.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(IFormFile file, string? prefix, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string storageKey, CancellationToken cancellationToken = default);
}
