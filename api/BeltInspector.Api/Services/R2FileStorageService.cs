using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using BeltInspector.Api.Settings;
using Microsoft.Extensions.Options;

namespace BeltInspector.Api.Services;

public class R2FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly R2Options _options;
    private readonly ILogger<R2FileStorageService> _logger;

    public R2FileStorageService(IAmazonS3 s3, IOptions<R2Options> options, ILogger<R2FileStorageService> logger)
    {
        _s3 = s3;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(IFormFile file, string? prefix, CancellationToken cancellationToken = default)
    {
        var normalizedPrefix = string.IsNullOrWhiteSpace(prefix) ? null : prefix.Trim('/');
        var key = string.IsNullOrEmpty(normalizedPrefix)
            ? $"{Guid.NewGuid()}_{file.FileName}"
            : $"{normalizedPrefix}/{Guid.NewGuid()}_{file.FileName}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            InputStream = stream,
            AutoCloseStream = true,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
        };

        _logger.LogInformation("Uploading {FileName} to R2 with key {Key}", file.FileName, key);
        await _s3.PutObjectAsync(request, cancellationToken);

        return key;
    }

    public async Task<Stream?> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _s3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _options.Bucket,
                Key = storageKey,
            }, cancellationToken);

            var memory = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memory, cancellationToken);
            memory.Position = 0;
            return memory;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning(ex, "File not found for key {Key}", storageKey);
            return null;
        }
    }
}
