# TeamLink Backend

## .NET SDK version

The author of this structure used .NET SDK 11.0.100.

## Build and run

You can build and run using the following command (in the **Backend** folder):

```bash
dotnet build
dotnet run --project src/TeamLink.Api/TeamLink.Api.csproj
```

or directly from the **TeamLink.Api** folder:

```bash
dotnet build
dotnet run
```

## Migrations and database

Set `ConnectionStrings__DefaultConnection` in the .env file or in the current PowerShell session before running the application.

On startup, the API applies all pending Entity Framework Core migrations automatically. If PostgreSQL is unavailable or migration fails, startup fails with the database error.

To migrate the database manually, run the following commands in the **Backend** folder.

Install (or update) the EF CLI to match the project's EF Core version (11):

```powershell
# Install
dotnet tool install --global dotnet-ef --version 11.0.0
# Update
dotnet tool update --global dotnet-ef --version 11.0.0
```

Create a migration after changing the models or entity configurations:

```powershell
dotnet ef migrations add DescribeChange --project src\TeamLink.Api\TeamLink.Api.csproj --startup-project src\TeamLink.Api\TeamLink.Api.csproj --output-dir Data\Migrations
```

Remove the latest migration (if not applied to the database):

```powershell
dotnet ef migrations remove --project src\TeamLink.Api\TeamLink.Api.csproj --startup-project src\TeamLink.Api\TeamLink.Api.csproj
```

Apply pending migrations manually:

```powershell
dotnet ef database update --project src\TeamLink.Api\TeamLink.Api.csproj --startup-project src\TeamLink.Api\TeamLink.Api.csproj
```

To roll back to a specific earlier migration, pass its migration name instead of leaving the target implicit:

```powershell
dotnet ef database update PreviousMigrationName --project src\TeamLink.Api\TeamLink.Api.csproj --startup-project src\TeamLink.Api\TeamLink.Api.csproj
```

## Documentation

Follow official Backend documentation in the [Software Design Document](https://github.com/PatricijaBt/TeamLink/blob/main/1_Laboratory_work.pdf).