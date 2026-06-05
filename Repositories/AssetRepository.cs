using AssetStore.Data;
using AssetStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly ApplicationDbContext _context;

    public AssetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetCreatorIdAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AsNoTracking()
            .Where(a => a.Id == assetId && !a.IsDeleted)
            .Select(a => a.CreatorId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
