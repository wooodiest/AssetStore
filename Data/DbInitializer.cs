using AssetStore.Models;
using AssetStore.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetStore.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager, logger);
        await SeedAdminUserAsync(userManager, configuration, logger);
        await SeedDemoCreatorAsync(userManager, configuration, logger);
        await SeedCategoriesAsync(context, logger);
        await SeedDemoAssetsAsync(context, userManager, configuration, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var roleName in AppRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var email = configuration["Seed:AdminEmail"] ?? "admin@assetstore.local";
        var password = configuration["Seed:AdminPassword"] ?? "Admin123!";

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is not null)
        {
            return;
        }

        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, AppRoles.Administrator);
    }

    private static async Task SeedDemoCreatorAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var email = configuration["Seed:DemoCreatorEmail"] ?? "creator@assetstore.local";
        var password = configuration["Seed:DemoCreatorPassword"] ?? "Creator123!";

        var creator = await userManager.FindByEmailAsync(email);
        if (creator is not null)
        {
            if (!await userManager.IsInRoleAsync(creator, AppRoles.Creator))
            {
                await userManager.AddToRoleAsync(creator, AppRoles.Creator);
            }

            return;
        }

        creator = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(creator, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create demo creator: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(creator, AppRoles.Creator);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var categories = new[]
        {
            new Category { Name = "3D Models", Description = "3D characters, environments, and props." },
            new Category { Name = "Textures", Description = "Diffuse, normal, roughness maps, and other PBR textures." },
            new Category { Name = "Audio", Description = "Sound effects, music, and audio samples." },
            new Category { Name = "Scripts", Description = "Scripts and tools for game engines." },
            new Category { Name = "Other", Description = "Other digital assets." }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} categories.", categories.Length);
    }

    private static async Task SeedDemoAssetsAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        if (await context.Assets.AnyAsync())
        {
            return;
        }

        var creatorEmail = configuration["Seed:DemoCreatorEmail"] ?? "creator@assetstore.local";
        var creator = await userManager.FindByEmailAsync(creatorEmail);
        if (creator is null)
        {
            logger.LogWarning("Demo creator not found. Skipping demo asset seed.");
            return;
        }

        var categories = await context.Categories.ToListAsync();
        if (categories.Count == 0)
        {
            return;
        }

        var categoryByName = categories.ToDictionary(c => c.Name, c => c.Id);

        var demoAssets = new[]
        {
            new Asset
            {
                CreatorId = creator.Id,
                Title = "Low Poly Character Pack",
                Description = "A set of 5 low-poly characters ready to import into Unity or Unreal.",
                Price = 0m,
                CategoryId = categoryByName["3D Models"],
                FileUrl = "demo/low-poly-characters.zip",
                UploadDate = DateTime.UtcNow,
                IsDeleted = false
            },
            new Asset
            {
                CreatorId = creator.Id,
                Title = "Forest Ambience Audio",
                Description = "A forest ambience loop with birds and wind, ideal for adventure games.",
                Price = 9.99m,
                CategoryId = categoryByName["Audio"],
                FileUrl = "demo/forest-ambience.zip",
                UploadDate = DateTime.UtcNow,
                IsDeleted = false
            },
            new Asset
            {
                CreatorId = creator.Id,
                Title = "PBR Stone Textures",
                Description = "A pack of 12 2K stone textures with PBR maps.",
                Price = 4.99m,
                CategoryId = categoryByName["Textures"],
                FileUrl = "demo/stone-textures.zip",
                UploadDate = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        context.Assets.AddRange(demoAssets);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} demo assets.", demoAssets.Length);
    }
}

