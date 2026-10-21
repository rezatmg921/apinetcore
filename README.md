# ApiNetCore

A starter ASP.NET Core Web API built with .NET 9, Clean Architecture, SQL Server, JWT bearer authentication, Swagger/OpenAPI, and centralized exception handling.

> **Runtime note:** .NET 9 reached end of support in May 2026. This repository targets .NET 9 as requested; for a new production system, consider upgrading to a supported LTS release.

## Solution structure

```text
src/
  ApiNetCore.Domain/          # Entities and domain rules
  ApiNetCore.Application/     # Use cases, DTOs, abstractions, validation
  ApiNetCore.Infrastructure/  # EF Core, SQL Server, persistence, JWT services
  ApiNetCore.Api/             # HTTP endpoints, middleware, Swagger, composition root
```

## Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB, SQL Server Express, or a server instance)

## Configure

Set `ConnectionStrings:DefaultConnection` and `Jwt:Key` using user secrets or environment variables. Never commit production secrets.

```bash
dotnet user-secrets init --project src/ApiNetCore.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=ApiNetCoreDb;Trusted_Connection=True;TrustServerCertificate=True" --project src/ApiNetCore.Api
dotnet user-secrets set "Jwt:Key" "replace-with-a-random-secret-at-least-32-characters" --project src/ApiNetCore.Api
```

## Build and run

```bash
dotnet restore
dotnet build
dotnet run --project src/ApiNetCore.Api
```

Open `/swagger` in Development to explore the API. Apply database migrations with the EF Core tools after adding migrations to the Infrastructure project.

## Included starter features

- Clean Architecture project separation
- EF Core SQL Server registration and DbContext
- JWT bearer authentication configuration
- Swagger/OpenAPI with Bearer security definition
- Centralized JSON exception handling and Problem Details responses
- Health endpoint and sample protected endpoint

## Security notes

Use a strong randomly generated JWT signing key, configure HTTPS, keep secrets out of source control, and replace demo authentication/token issuance with your real identity provider or user store before production use.
