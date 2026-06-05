using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AssetStore.Dto.Files;
using AssetStore.Models.Common;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class AzureBlobStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".zip", ".glb", ".gltf", ".fbx", ".png", ".jpg", ".jpeg", ".wav", ".mp3", ".ogg"
    ];

    private const long MaxFileSizeBytes = 100 * 1024 * 1024;

    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["FileStorage:Azure:ConnectionString"];
        var containerName = configuration["FileStorage:Azure:ContainerName"] ?? "assetstore-files";

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Azure storage connection string is not configured.");
        }

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.None);
    }

    public async Task<ServiceResult<string>> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            return ServiceResult<string>.Fail("File is empty.", ServiceErrorCode.BadRequest);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return ServiceResult<string>.Fail("File exceeds the maximum allowed size of 100 MB.", ServiceErrorCode.BadRequest);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return ServiceResult<string>.Fail("File type is not allowed.", ServiceErrorCode.BadRequest);
        }

        var blobName = $"{Guid.NewGuid():N}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders
        {
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType
        }, cancellationToken: cancellationToken);

        return ServiceResult<string>.Ok(blobName);
    }

    public async Task<ServiceResult<FileDownloadDto>> GetFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return ServiceResult<FileDownloadDto>.Fail("File path is missing.", ServiceErrorCode.BadRequest);
        }

        var blobClient = _containerClient.GetBlobClient(fileUrl);
        var exists = await blobClient.ExistsAsync(cancellationToken);
        if (!exists)
        {
            return ServiceResult<FileDownloadDto>.Fail("File not found.", ServiceErrorCode.NotFound);
        }

        var response = await blobClient.DownloadAsync(cancellationToken);
        var contentType = response.Value.Details.ContentType ?? "application/octet-stream";
        var fileName = Path.GetFileName(fileUrl);

        return ServiceResult<FileDownloadDto>.Ok(new FileDownloadDto
        {
            Stream = response.Value.Content,
            ContentType = contentType,
            FileName = fileName
        });
    }
}
