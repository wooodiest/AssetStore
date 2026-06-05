using AssetStore.Dto.Creator;

namespace AssetStore.Services.Interfaces;

public interface ICreatorDashboardService
{
    Task<CreatorDashboardDto> GetDashboardAsync(string creatorId, CancellationToken cancellationToken = default);
}

