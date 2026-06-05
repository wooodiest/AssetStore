# Project Documentation: Asset Store

## 1. Project Overview

The "Asset Store" is a web-based marketplace application designed for independent game developers, 3D graphic designers, and sound engineers. The platform allows users to share, discover, and acquire digital assets (such as 3D models, textures, audio files, and scripts) necessary for game development.

The project is developed as the final assignment for the "Modern Web Applications Development" course. It strictly adheres to the course requirements, implementing a robust backend architecture, relational database management, secure authorization, and complete CRUD operations.

## 2. Technology Stack

- **Backend Framework:** ASP.NET Core (Version 8.0), C# - utilizing a recent release as encouraged by the course instructor.
- **Architecture Pattern:** ASP.NET Core MVC (Model-View-Controller). _Note: MVC was explicitly chosen to fulfill the instructor's recommendation for better visual testing and to utilize HTML/JavaScript for an enhanced frontend presentation._
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Authentication & Identity:** ASP.NET Core Identity (Cookie-based for MVC)
- **Cloud Hosting & Storage (Bonus):** Microsoft Azure App Service (hosting), Azure Blob Storage (asset file storage)

## 3. Architecture and Design Patterns

The application follows a standard N-Tier architecture to ensure separation of concerns, scalability, and maintainability, strictly following the instructor's structural requirements.

- **Core Layers:** The project is explicitly divided into the Model layer, Repository layer, and MVC layer.
- **Service Layer (Bonus):** Implemented to handle complex business logic, separating it from the controllers for extra grading points as suggested.
- **Clean Code & Conventions:** The project strictly adheres to ASP.NET Core naming conventions, proper routing nomenclature, and clean code principles as discussed during the lectures.
- **Inversion of Control (IoC) & Dependency Injection:** Extensively used for injecting services, database contexts, and repositories.

## 4. User Roles and Authorization

The system implements role-based access control (RBAC) to ensure security and proper data isolation, fulfilling the requirement to handle different user access levels effectively.

- **Guest:** Can browse the marketplace, search for assets, and view asset details, but cannot download files or leave reviews.
- **User (Buyer):** Registered account. Can download free assets, purchase premium assets, view transaction history, and post reviews.
- **Creator (Seller):** Has access to a dedicated dashboard. Can upload new assets, manage their portfolio, and track download statistics.
- **Administrator:** Possesses global access. Can manage categories, moderate reviews, and ban or suspend users violating the terms of service.

## 5. Core Functionalities (CRUD Operations)

The application fulfills the mandatory requirement of executing full CRUD operations across multiple related entities.

### Create

- **User Registration:** Creating a new account with hashed passwords.
- **Asset Upload:** Creators can upload new asset packages, define titles, descriptions, categories, and prices.
- **Reviews:** Users can add ratings and written reviews to assets they have downloaded.

### Read

- **Catalog Browsing:** Fetching a paginated list of assets available in the store.
- **Search and Filtering:** Retrieving specific assets based on keywords, category filters, or price range.
- **Asset Details:** Viewing detailed information about a single asset, including creator details and user reviews.
- **Dashboard Analytics:** Creators can read their specific statistics and uploaded items.

### Update

- **Profile Management:** Users can update their display name, bio, or avatar.
- **Asset Modification:** Creators can update the description, price, or tags of their existing assets.
- **Role Escalation:** Administrators can promote a standard User to a Creator role.

### Delete

- **Asset Removal:** Creators can delete their assets (implemented as a soft-delete to preserve transaction history for previous buyers).
- **Content Moderation:** Administrators can hard-delete inappropriate reviews or assets.

## 6. Database Schema (Main Entities)

The database structure is managed via Entity Framework Core utilizing Code-First Migrations. It comfortably exceeds the minimum requirement of 2-3 tables.

- **User:** Id, Username, Email, PasswordHash, Role, CreatedAt.
- **Asset:** Id, CreatorId (Foreign Key), Title, Description, Price, CategoryId, FileUrl, UploadDate, IsDeleted.
- **Category:** Id, Name, Description.
- **Review:** Id, AssetId (Foreign Key), UserId (Foreign Key), Rating, Comment, PostedAt.
- **Transaction:** Id, AssetId (Foreign Key), UserId (Foreign Key), Amount, PurchaseDate.

## 7. Deployment and Hosting Strategy

To maximize the project grade and utilize modern deployment practices, the application will be hosted in the cloud.

- The application backend and frontend will be deployed using **Azure App Service**.
- The SQL database will be hosted on **Azure SQL Database**.
- Heavy binary files (e.g., .zip archives containing 3D models, high-resolution textures) will NOT be stored in the relational database. Instead, they will be uploaded securely to **Azure Blob Storage**, and the database will only store the reference URL.
