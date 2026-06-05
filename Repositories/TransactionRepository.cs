using AssetStore.Data;
using AssetStore.Models;
using AssetStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task<IReadOnlyList<Transaction>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Asset)
                .ThenInclude(a => a.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.PurchaseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPurchasedAsync(string userId, int assetId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AnyAsync(t => t.UserId == userId && t.AssetId == assetId, cancellationToken);
    }
}
