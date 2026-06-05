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
        await SeedDemoCreatorsAsync(userManager, configuration, logger);
        await SeedCategoriesAsync(context, logger);
        await SeedDemoAssetsAsync(context, userManager, configuration, logger);
        await SeedDemoUsersAndActivityAsync(context, userManager, configuration, logger);
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

    private static async Task SeedDemoCreatorsAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var defaults = new[]
        {
            new { Email = configuration["Seed:DemoCreatorEmail1"] ?? "creator1@assetstore.local", Password = configuration["Seed:DemoCreatorPassword1"] ?? "Creator123!" },
            new { Email = configuration["Seed:DemoCreatorEmail2"] ?? "creator2@assetstore.local", Password = configuration["Seed:DemoCreatorPassword2"] ?? "Creator123!" },
            new { Email = configuration["Seed:DemoCreatorEmail3"] ?? "creator3@assetstore.local", Password = configuration["Seed:DemoCreatorPassword3"] ?? "Creator123!" }
        };

        foreach (var entry in defaults)
        {
            var creator = await userManager.FindByEmailAsync(entry.Email);
            if (creator is not null)
            {
                if (!await userManager.IsInRoleAsync(creator, AppRoles.Creator))
                {
                    await userManager.AddToRoleAsync(creator, AppRoles.Creator);
                }

                if (!await userManager.IsInRoleAsync(creator, AppRoles.User))
                {
                    await userManager.AddToRoleAsync(creator, AppRoles.User);
                }

                continue;
            }

            creator = new ApplicationUser
            {
                UserName = entry.Email,
                Email = entry.Email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await userManager.CreateAsync(creator, entry.Password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create demo creator {Email}: {Errors}", entry.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                continue;
            }

            await userManager.AddToRoleAsync(creator, AppRoles.Creator);
            await userManager.AddToRoleAsync(creator, AppRoles.User);
        }
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

        var creators = await userManager.GetUsersInRoleAsync(AppRoles.Creator);
        if (creators is null || creators.Count == 0)
        {
            logger.LogWarning("No creators found. Skipping demo asset seed.");
            return;
        }

        var selectedCreators = creators.Take(3).ToList();

        var categories = await context.Categories.ToListAsync();
        if (categories.Count == 0)
        {
            return;
        }

        var categoryByName = categories.ToDictionary(c => c.Name, c => c.Id);

        var templates = new[]
        {
            new { Title = "Low Poly Hero Pack", Category = "3D Models", Description = "Set of stylized low-poly hero characters with LODs and game-ready rigs.", Price = 0m, File = "demo/low-poly-hero-pack.zip" },
            new { Title = "Sci-Fi Interior Kit", Category = "3D Models", Description = "Modular sci-fi interior pieces for fast level assembly.", Price = 12.99m, File = "demo/scifi-interior-kit.zip" },
            new { Title = "Modular Building Kit", Category = "3D Models", Description = "Modular walls, doors, and windows for urban environments.", Price = 9.49m, File = "demo/modular-building-kit.zip" },
            new { Title = "Character Animation Pack", Category = "3D Models", Description = "Baked animation clips: walk, run, jump, attack, idle variations.", Price = 7.99m, File = "demo/character-animation-pack.zip" },
            new { Title = "Weapon Models Pack", Category = "3D Models", Description = "High-quality weapon models with PBR materials.", Price = 14.99m, File = "demo/weapon-models-pack.zip" },
            new { Title = "PBR Stone Textures 2K", Category = "Textures", Description = "12 stone textures with albedo, normal and roughness maps (2K).", Price = 4.99m, File = "demo/pbr-stone-textures-2k.zip" },
            new { Title = "Hand-Painted Tileset", Category = "Textures", Description = "Seamless hand-painted tiles for stylized games.", Price = 3.99m, File = "demo/hand-painted-tileset.zip" },
            new { Title = "Water Shader Pack", Category = "Textures", Description = "Efficient water shader with foam and reflections for URP/HDRP.", Price = 8.99m, File = "demo/water-shader-pack.zip" },
            new { Title = "Vehicle Textures 4K", Category = "Textures", Description = "4K texture set for vehicles including masks and emissive maps.", Price = 11.50m, File = "demo/vehicle-textures-4k.zip" },
            new { Title = "Forest Ambience Pack", Category = "Audio", Description = "Long ambience loops with birds, wind and distant water.", Price = 5.99m, File = "demo/forest-ambience-pack.zip" },
            new { Title = "UI SFX Bundle", Category = "Audio", Description = "Short UI sound effects: clicks, notifications, success/fail.", Price = 2.99m, File = "demo/ui-sfx-bundle.zip" },
            new { Title = "Orchestral Percussion Kit", Category = "Audio", Description = "Percussion one-shots and loops for cinematic scoring.", Price = 6.49m, File = "demo/orchestral-percussion-kit.zip" },
            new { Title = "Footstep SFX Collection", Category = "Audio", Description = "Varied footstep samples across surfaces and speeds.", Price = 1.99m, File = "demo/footstep-sfx.zip" },
            new { Title = "AI Dialogue Manager", Category = "Scripts", Description = "Lightweight dialogue system with branching and localization support.", Price = 9.99m, File = "demo/ai-dialogue-manager.zip" },
            new { Title = "VR Interaction Utilities", Category = "Scripts", Description = "Common VR interaction scripts: grab, throw, UI pointing.", Price = 7.49m, File = "demo/vr-interaction-utilities.zip" },
            new { Title = "Procedural Terrain Tools", Category = "Scripts", Description = "Procedural terrain generator and brush-based editors.", Price = 12.00m, File = "demo/procedural-terrain-tools.zip" },
            new { Title = "HUD & UI Kit", Category = "Other", Description = "Modular HUD elements and UI layouts for games.", Price = 3.50m, File = "demo/hud-ui-kit.zip" },
            new { Title = "Top-down Tileset", Category = "Other", Description = "Top-down game tileset with animated elements.", Price = 2.99m, File = "demo/topdown-tileset.zip" },
            new { Title = "Cartoon VFX Pack", Category = "Other", Description = "Particle and sprite VFX for cartoon-style games.", Price = 4.25m, File = "demo/cartoon-vfx-pack.zip" },
            new { Title = "Optimization Utilities", Category = "Scripts", Description = "Tools for LOD generation and mesh optimization.", Price = 0m, File = "demo/optimization-utilities.zip" }
        };

        var demoAssets = new List<Asset>();
        for (int i = 0; i < templates.Length; i++)
        {
            var tpl = templates[i];
            var creator = selectedCreators[i % selectedCreators.Count];
            var categoryId = categoryByName.ContainsKey(tpl.Category) ? categoryByName[tpl.Category] : categories[i % categories.Count].Id;

            demoAssets.Add(new Asset
            {
                CreatorId = creator.Id,
                Title = tpl.Title,
                Description = tpl.Description,
                Price = tpl.Price,
                CategoryId = categoryId,
                FileUrl = tpl.File,
                UploadDate = DateTime.UtcNow.AddDays(-i),
                IsDeleted = false
            });
        }

        context.Assets.AddRange(demoAssets);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} demo assets.", demoAssets.Count);
    }

    private static async Task SeedDemoUsersAndActivityAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var usersDefaults = new[]
        {
            new { Email = configuration["Seed:DemoUserEmail1"] ?? "user1@assetstore.local", Password = configuration["Seed:DemoUserPassword1"] ?? "User123!" },
            new { Email = configuration["Seed:DemoUserEmail2"] ?? "user2@assetstore.local", Password = configuration["Seed:DemoUserPassword2"] ?? "User123!" },
            new { Email = configuration["Seed:DemoUserEmail3"] ?? "user3@assetstore.local", Password = configuration["Seed:DemoUserPassword3"] ?? "User123!" }
        };

        var demoUsers = new List<ApplicationUser>();
        foreach (var u in usersDefaults)
        {
            var user = await userManager.FindByEmailAsync(u.Email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var res = await userManager.CreateAsync(user, u.Password);
                if (!res.Succeeded)
                {
                    logger.LogError("Failed to create demo user {Email}: {Errors}", u.Email, string.Join(", ", res.Errors.Select(e => e.Description)));
                    continue;
                }
            }

            if (await userManager.IsInRoleAsync(user, AppRoles.Creator))
            {
                await userManager.RemoveFromRoleAsync(user, AppRoles.Creator);
            }
            if (await userManager.IsInRoleAsync(user, AppRoles.Administrator))
            {
                await userManager.RemoveFromRoleAsync(user, AppRoles.Administrator);
            }

            if (!await userManager.IsInRoleAsync(user, AppRoles.User))
            {
                await userManager.AddToRoleAsync(user, AppRoles.User);
            }

            demoUsers.Add(user);
        }

        if (demoUsers.Count == 0)
        {
            logger.LogWarning("No demo users available for activity seeding.");
            return;
        }

        var assets = await context.Assets.Where(a => !a.IsDeleted).ToListAsync();
        if (assets.Count == 0)
        {
            logger.LogWarning("No assets available for purchase seeding.");
            return;
        }


        bool hasDemoActivity = await context.Transactions.AnyAsync(t => demoUsers.Select(d => d.Id).Contains(t.UserId));
        if (hasDemoActivity)
        {
            logger.LogInformation("Demo user activity already exists, skipping activity seeding.");
            return;
        }

        var transactions = new List<Transaction>();
        var reviews = new List<Review>();

        for (int i = 0; i < demoUsers.Count; i++)
        {
            var user = demoUsers[i];
            for (int j = 0; j < 4; j++)
            {
                var asset = assets[(i * 4 + j) % assets.Count];
                transactions.Add(new Transaction
                {
                    AssetId = asset.Id,
                    UserId = user.Id,
                    Amount = asset.Price,
                    PurchaseDate = DateTime.UtcNow.AddDays(-(i * 4 + j))
                });

                if (j < 2 && asset.CreatorId != user.Id)
                {
                    if (!await context.Reviews.AnyAsync(r => r.AssetId == asset.Id && r.UserId == user.Id))
                    {
                        var rating = ((i + j) % 5) + 1;
                        reviews.Add(new Review
                        {
                            AssetId = asset.Id,
                            UserId = user.Id,
                            Rating = rating,
                            Comment = $"Great asset: {asset.Title} - useful in many scenes.",
                            PostedAt = DateTime.UtcNow.AddDays(-(i * 4 + j))
                        });
                    }
                }
            }
        }

        if (transactions.Count > 0)
        {
            await context.Transactions.AddRangeAsync(transactions);
        }
        if (reviews.Count > 0)
        {
            await context.Reviews.AddRangeAsync(reviews);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {TxCount} demo transactions and {RvCount} demo reviews.", transactions.Count, reviews.Count);
    }
}

