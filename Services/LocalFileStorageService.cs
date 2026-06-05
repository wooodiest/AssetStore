using AssetStore.Dto.Files;
using AssetStore.Models.Common;
using AssetStore.Services.Interfaces;

namespace AssetStore.Services;

public class LocalFileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".zip", ".glb", ".gltf", ".fbx", ".png", ".jpg", ".jpeg", ".wav", ".mp3", ".ogg"
    ];

    private const long MaxFileSizeBytes = 100 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadRelativePath;

    public LocalFileStorageService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _uploadRelativePath = configuration["FileStorage:UploadPath"] ?? "App_Data/Uploads";
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

        var uploadDirectory = Path.Combine(_environment.ContentRootPath, _uploadRelativePath);
        Directory.CreateDirectory(uploadDirectory);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadDirectory, storedFileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, cancellationToken);

        var relativePath = Path.Combine(_uploadRelativePath, storedFileName).Replace('\\', '/');
        return ServiceResult<string>.Ok(relativePath);
    }

    public Task<ServiceResult<FileDownloadDto>> GetFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.FromResult(ServiceResult<FileDownloadDto>.Fail("File path is missing.", ServiceErrorCode.BadRequest));
        }

        var normalizedPath = fileUrl.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, normalizedPath));
        var rootPath = Path.GetFullPath(_environment.ContentRootPath);

        if (!fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(ServiceResult<FileDownloadDto>.Fail("Invalid file path.", ServiceErrorCode.Forbidden));
        }

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(ServiceResult<FileDownloadDto>.Fail("File not found.", ServiceErrorCode.NotFound));
        }

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = Path.GetFileName(fullPath);

        return Task.FromResult(ServiceResult<FileDownloadDto>.Ok(new FileDownloadDto
        {
            Stream = stream,
            ContentType = "application/octet-stream",
            FileName = fileName
        }));
    }
}

