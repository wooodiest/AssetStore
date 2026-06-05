# AssetStore

AssetStore is an ASP.NET Core 8.0 MVC application designed as a demonstration and small storefront for selling digital assets. It implements typical e-commerce flows for digital goods including asset upload and download, creators, purchases, reviews, and user/role management using ASP.NET Identity and EF Core.

**Project Highlights**

- Clean service-repository-controller architecture with DTOs and view models.
- EF Core Code First migrations and seed logic.
- Pluggable file storage abstraction (`IFileStorageService`) with both local and Azure Blob implementations.
- Demo data seeding for quick local evaluation.

**Tech Stack**

- .NET 8 (C# 12)
- ASP.NET Core MVC
- Entity Framework Core (SQL Server / LocalDB)
- ASP.NET Core Identity
- Azure Blob Storage (optional)

**Repository Structure (high level)**

- `Controllers/` — MVC controllers
- `Services/` — business logic and file storage implementations
- `Data/` — `ApplicationDbContext`, migrations and `DbInitializer`
- `Models/`, `Dto/`, `ViewModels/` — domain types and transfer objects
- `Views/`, `wwwroot/` — UI and static assets

Prerequisites

- .NET 8 SDK installed
- SQL Server or LocalDB for local development
- (Optional) Azure Storage account if using Azure provider

Configuration
Configuration lives in `appsettings.json` and `appsettings.Development.json`.

- By default the project is configured for local development. `appsettings.json` contains a `FileStorage` section with the following relevant keys:
  - `FileStorage:Provider` — `Local` or `Azure`. Default: `Local`.
  - `FileStorage:UploadPath` — used by the local provider (e.g. `App_Data/Uploads`).
  - `FileStorage:Azure:ConnectionString` — Azure Storage connection string (when `Provider` is `Azure`).
  - `FileStorage:Azure:ContainerName` — blob container name.

- Database connection string is read from `ConnectionStrings:DefaultConnection`. For local development this typically points to LocalDB. For Azure or production, change `DefaultConnection` to your Azure SQL connection string.

Seeded demo data
The project includes a `DbInitializer` that seeds roles, demo users (assigned the `User` role), demo creators, categories, assets, and example transactions/reviews. The initializer will run when the application starts (if configured) or you can invoke the seeding routines manually for a fresh database.

Storage providers

- Local: saves uploaded files under the configured `UploadPath` and is recommended for local development.
- Azure Blob: uploads assets and thumbnails to an Azure Storage container. Use this provider for cloud deployments and set `FileStorage:Provider` to `Azure` and the connection string accordingly.
