using Microsoft.AspNetCore.Identity;

namespace AssetStore.Models;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public ICollection<Asset> CreatedAssets { get; set; } = [];

    public ICollection<Review> Reviews { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];
}

