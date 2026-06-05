using AssetStore.Dto.Files;
using AssetStore.Models.Common;

namespace AssetStore.Services.Interfaces;

public interface IFileStorageService
{
    Task<ServiceResult<string>> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default);

    Task<ServiceResult<FileDownloadDto>> GetFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}
