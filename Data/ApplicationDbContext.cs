using AssetStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);
        });

        builder.Entity<Asset>(entity =>
        {
            entity.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(4000);

            entity.Property(a => a.Price)
                .HasPrecision(18, 2);

            entity.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(a => a.ThumbnailUrl)
                .IsRequired(false)
                .HasMaxLength(500);

            entity.HasOne(a => a.Creator)
                .WithMany(u => u.CreatedAssets)
                .HasForeignKey(a => a.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(a => a.IsDeleted);
            entity.HasIndex(a => a.CategoryId);
        });

        builder.Entity<Review>(entity =>
        {
            entity.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(2000);

            entity.HasOne(r => r.Asset)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.AssetId, r.UserId })
                .IsUnique();
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.Amount)
                .HasPrecision(18, 2);

            entity.HasOne(t => t.Asset)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(t => new { t.UserId, t.AssetId })
                .IsUnique();
        });
    }
}

