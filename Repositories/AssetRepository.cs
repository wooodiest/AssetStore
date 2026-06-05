using AssetStore.Data;
using AssetStore.Models;
using AssetStore.Models.Common;
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

    public async Task<PagedResult<Asset>> GetPagedAsync(
        int page,
        int pageSize,
        int? categoryId = null,
        decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assets
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Creator)
            .Where(a => !a.IsDeleted);

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(a => a.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.UploadDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Asset>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Creator)
            .Include(a => a.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<Asset?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetByCreatorAsync(string creatorId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => a.CreatorId == creatorId && !a.IsDeleted)
            .OrderByDescending(a => a.UploadDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetCreatorIdAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AsNoTracking()
            .Where(a => a.Id == assetId && !a.IsDeleted)
            .Select(a => a.CreatorId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Asset> CreateAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync(cancellationToken);
        return asset;
    }

    public async Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

        if (asset is null)
        {
            return false;
        }

        asset.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assets.CountAsync(a => !a.IsDeleted, cancellationToken);
    }
}

