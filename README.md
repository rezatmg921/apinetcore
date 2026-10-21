# ApiNetCore

A starter ASP.NET Core Web API using .NET 9, Clean Architecture, SQL Server, JWT bearer authentication, Swagger/OpenAPI, and centralized exception handling.

> **Runtime note:** .NET 9 support ended in May 2026. This repository targets .NET 9 as requested; for a new production system, prefer a currently supported LTS release.

## Architecture

- **Domain** — entities and domain rules
- **Application** — abstractions and application contracts
- **Infrastructure** — EF Core, SQL Server, persistence
- **Api** — HTTP endpoints, JWT validation, Swagger, exception handling

## Requirements

- .NET 9 SDK
- SQL Server or SQL Server LocalDB
- Optional: `dotnet-ef` CLI tool for migrations

## Configure secrets

The app intentionally refuses to start with the placeholder JWT key. From the repository root:

```bash
dotnet user-secrets init --project src/ApiNetCore.Api
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)" --project src/ApiNetCore.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=ApiNetCoreDb;Trusted_Connection=True;TrustServerCertificate=True" --project src/ApiNetCore.Api
dotnet user-secrets set "Auth:DemoUsername" "demo" --project src/ApiNetCore.Api
dotnet user-secrets set "Auth:DemoPassword" "replace-with-a-long-random-password" --project src/ApiNetCore.Api
```

User Secrets are for local development only. Use a managed secret store or environment variables in deployed environments. The demo credentials are not a production identity system.

## Build and run

```bash
dotnet restore ApiNetCore.sln
dotnet build ApiNetCore.sln
dotnet test ApiNetCore.sln
dotnet run --project src/ApiNetCore.Api
```

When running in Development, open the `/swagger` page shown by the launch profile.

## Database migrations

The initial `TodoItems` migration is included. Install the EF CLI if needed, then apply it:

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
dotnet ef database update --project src/ApiNetCore.Infrastructure --startup-project src/ApiNetCore.Api
```

Review generated migrations before applying them to shared or production databases.

## Endpoints

| Method | Path | Access | Purpose |
| --- | --- | --- | --- |
| GET | `/health` | Anonymous | Health check |
| POST | `/auth/token` | Development only | Demo credentials to JWT |
| GET | `/api/todos` | Bearer token | List todo items |
| POST | `/api/todos` | Bearer token | Create a todo item |

### Request a development token

```http
POST /auth/token
Content-Type: application/json

{
  "username": "demo",
  "password": "your-configured-demo-password"
}
```

Use the returned `accessToken` as a Bearer token in Swagger's **Authorize** dialog. The demo token endpoint is deliberately registered only in the Development environment. Replace it with a real identity provider and user store before production.

### Create a todo

```http
POST /api/todos
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "title": "First task" }
```

## Included

- Layered solution and sample Todo entity/repository
- SQL Server DbContext, design-time factory, and initial EF Core migration
- JWT validation and development-only demo token issuance
- Swagger/OpenAPI Bearer configuration
- Centralized exception handling with Problem Details
- Basic request validation and health endpoint

## Security and limitations

Never commit credentials or production secrets. Use HTTPS, rate-limit authentication endpoints, and adopt a production identity provider. GitHub Actions is configured to restore, build, and run unit tests on pushes and pull requests. The first CI run should be checked before treating the starter as verified. Run the database migration command above in an environment with SQL Server available.
