# TaskManager BackEnd

TaskManager BackEnd is an ASP.NET Core Web API for managing users, projects, sprints, work items, authentication, OTP-based password recovery, email notifications, and scheduled sprint automation.

The project is built with a clean layered structure that separates API controllers, domain models, DTOs, services, repositories, and database infrastructure.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Run the API](#run-the-api)
- [API Modules](#api-modules)
- [Security Notes](#security-notes)
- [Useful Commands](#useful-commands)

## Features

- JWT authentication and authorization
- User login with OTP verification
- Forgot password and reset password flow
- User management
- Project management
- Sprint management
- Work item management
- Assign users to projects and work items
- Track work item status, type, estimated time, and actual time
- Parent and child work item relations
- Email notifications with MailKit SMTP
- Hangfire background processing
- Daily sprint closing job based on Jordan time
- Entity Framework Core migrations with SQL Server
- Swagger UI for API exploration in development

## Tech Stack

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Hangfire
- MailKit
- BCrypt.Net
- Swagger / Swashbuckle

## Architecture

The solution follows a layered architecture:

- `TaskManager-p`: API layer, controllers, dependency injection, authentication, CORS, Swagger, and application startup.
- `TaskManager.core`: Core layer, entities, DTOs, enums, helpers, interfaces, and mappers.
- `TaskManager.infrastructrue`: Infrastructure layer, EF Core DbContext, configurations, migrations, repositories, and service implementations.

## Project Structure

```text
TaskManager-p/
├── TaskManager-p/                  # ASP.NET Core API project
│   ├── Controllers/                # API controllers
│   ├── Program.cs                  # App startup and service registration
│   ├── appsettings.json            # Public-safe configuration template
│   └── appsettings.Development.json # Local secrets, ignored by Git
├── TaskManager.core/               # Domain, DTOs, interfaces, helpers
├── TaskManager.infrastructrue/     # EF Core, repositories, services, migrations
├── .gitignore
└── TaskManager-p.slnx
```

## Getting Started

### Requirements

- .NET 10 SDK
- SQL Server or SQL Server LocalDB
- Git
- Optional: Visual Studio 2026 or VS Code
- Optional: `dotnet-ef` CLI tool

Install EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

Clone the repository:

```bash
git clone https://github.com/Mohammadsukhni/TaskManager-BackEnd.git
cd TaskManager-BackEnd
```

Restore dependencies:

```bash
dotnet restore
```

## Configuration

The committed `appsettings.json` is intentionally safe and does not contain real secrets.

Create this file locally:

```text
TaskManager-p/appsettings.Development.json
```

Use this template and replace the placeholder values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManager;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "replace-with-a-long-random-secret-key",
    "Issuer": "TaskManager.Api",
    "Audience": "TaskManager.Client",
    "ExpireMinutes": 60
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "UseSsl": true,
    "From": "your-email@example.com",
    "SmtpUserName": "your-email@example.com",
    "SmtpPassword": "your-app-password"
  }
}
```

`appsettings.Development.json` is ignored by Git and must never be committed.

You can also use environment variables instead of local files:

```bash
ConnectionStrings__DefaultConnection="your-connection-string"
Jwt__Key="your-secret-key"
Email__SmtpPassword="your-smtp-password"
```

## Database Setup

Apply the Entity Framework Core migrations:

```bash
dotnet ef database update --project TaskManager.infrastructrue/TaskManager.Infrastructure.csproj --startup-project TaskManager-p/TaskManager.Api.csproj
```

The database connection is read from:

```text
ConnectionStrings:DefaultConnection
```

## Run the API

Run from the repository root:

```bash
dotnet run --project TaskManager-p/TaskManager.Api.csproj
```

Default development URLs:

```text
HTTP:  http://localhost:5295
HTTPS: https://localhost:7195
Swagger: https://localhost:7195/swagger
Hangfire Dashboard: https://localhost:7195/hangfire
```

The API currently allows frontend requests from:

```text
http://localhost:4200
```

## API Modules

### Auth

Base route:

```text
/api/Auths
```

Endpoints:

- `POST /api/Auths/login`
- `POST /api/Auths/verify-otp`
- `POST /api/Auths/forgot-password`
- `POST /api/Auths/reset-password`

### Users

Base route:

```text
/api/Users
```

Endpoints:

- `GET /api/Users`
- `GET /api/Users/{id}`
- `POST /api/Users`
- `PUT /api/Users`
- `PUT /api/Users/change-status`
- `GET /api/Users/my-account`
- `PUT /api/Users/my-account`
- `DELETE /api/Users/{id}`

### Projects

Base route:

```text
/api/Projects
```

Endpoints:

- `GET /api/Projects`
- `GET /api/Projects/{id}`
- `GET /api/Projects/my-projects`
- `POST /api/Projects`
- `PUT /api/Projects`
- `DELETE /api/Projects/{id}`
- `POST /api/Projects/{projectId}/assign-user/{userId}`

### Sprints

Base route:

```text
/api/Sprints
```

Endpoints:

- `GET /api/Sprints`
- `GET /api/Sprints/{id}`
- `POST /api/Sprints`
- `PUT /api/Sprints`
- `DELETE /api/Sprints/{id}`
- `GET /api/Sprints/my-sprints`

### Work Items

Base route:

```text
/api/WorkItems
```

Endpoints:

- `GET /api/WorkItems`
- `GET /api/WorkItems/{id}`
- `POST /api/WorkItems`
- `PUT /api/WorkItems`
- `DELETE /api/WorkItems/{id}`
- `POST /api/WorkItems/{workItemId}/assign-user/{userId}`
- `POST /api/WorkItems/relation`
- `PUT /api/WorkItems/{workItemId}/my-status`
- `PUT /api/WorkItems/my-workitem`
- `GET /api/WorkItems/my-workitems`

## Background Jobs

Hangfire is configured to use SQL Server storage.

The application registers a recurring job:

```text
close-ended-sprints
```

It runs daily at 01:00 using Jordan time when available, and closes ended sprints through `ISprintBackgroundJobService`.

## Security Notes

- Do not commit `appsettings.Development.json`.
- Do not commit SMTP passwords, JWT keys, API keys, or production connection strings.
- Use a strong JWT key in local and production environments.
- Use Gmail App Passwords instead of your real account password when using Gmail SMTP.
- Rotate any secret immediately if it is ever pushed to a public repository.
- Keep `bin/`, `obj/`, `.vs/`, and generated local artifacts out of Git.

## Useful Commands

Check Git status:

```bash
git status
```

Build the solution:

```bash
dotnet build
```

Run the API:

```bash
dotnet run --project TaskManager-p/TaskManager.Api.csproj
```

Apply migrations:

```bash
dotnet ef database update --project TaskManager.infrastructrue/TaskManager.Infrastructure.csproj --startup-project TaskManager-p/TaskManager.Api.csproj
```

Create a new migration:

```bash
dotnet ef migrations add MigrationName --project TaskManager.infrastructrue/TaskManager.Infrastructure.csproj --startup-project TaskManager-p/TaskManager.Api.csproj
```

Commit and push changes:

```bash
git add .
git commit -m "Describe your change"
git push
```
