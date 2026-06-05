using AssetStore.Data;
using AssetStore.Models;
using AssetStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetByAssetAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.AssetId == assetId)
            .OrderByDescending(r => r.PostedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);
        return review;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FindAsync([id], cancellationToken);
        if (review is null)
        {
            return false;
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(int assetId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AnyAsync(r => r.AssetId == assetId && r.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(r => r.Asset)
            .Include(r => r.User)
            .OrderByDescending(r => r.PostedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reviews.CountAsync(cancellationToken);
    }
}

