# VozUniversitaria

VozUniversitaria is a web application developed with ASP.NET Core MVC on .NET 8. The project follows a clean MVC structure and uses the default ASP.NET Core web template with Razor views, Bootstrap styling, and modern server-side rendering practices.

## Overview

VozUniversitaria is a university communication and academic information portal. The
current implementation provides an ASP.NET Core MVC application with user registration,
cookie-based login, role-based authorization, and SQL Server persistence through Entity
Framework Core.

## Tech Stack

- .NET 8
- ASP.NET Core MVC
- C#
- Razor Views
- HTML5
- CSS3
- JavaScript
- Bootstrap
- jQuery
- Entity Framework Core 8
- SQL Server
- BCrypt.Net-Next
- Git / GitFlow
- Visual Studio Code dev container

## Features

- ASP.NET Core MVC architecture
- Razor-based view rendering
- Bootstrap-powered responsive layout
- Static assets served from `wwwroot`
- Home and privacy pages
- User registration and login
- Cookie-based authentication
- Role claims and authorization policies for administrators and teaching staff
- User validation with ViewModels and DataAnnotations
- BCrypt password hashing
- Unique email and `CaseNumber` constraints
- EF Core migrations and seed data for development

## Project Structure

```text
VozUniversitaria/
├── Controllers/
│   ├── AccountController.cs
│   └── HomeController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbContextFactory.cs
├── Migrations/
│   ├── 20260908045623_InitialCreate.cs
│   ├── 20260918021207_AddUserConstraintsAndSeeding.cs
│   └── 20260918023553_FixSeedUserPasswordHash.cs
├── Models/
│   ├── ErrorViewModel.cs
│   └── Users.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   └── RegisterViewModel.cs
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Layout.cshtml.css
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── VozUniversitaria.csproj
└── Properties/
    └── launchSettings.json
```

## Architecture Diagram

```mermaid
flowchart LR
    A[Browser] --> B[ASP.NET Core MVC App]
    B --> C[Controllers]
    C --> D[Models]
    C --> E[Razor Views]
    E --> F[Bootstrap / CSS / JS]
    B --> G[Static Files - wwwroot]
    B --> H[Routing + Middleware]
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server available at `localhost:1433` with credentials matching the connection string in `VozUniversitaria/appsettings.json`
- A code editor such as Visual Studio Code
- Git

### Installation

1. Clone the repository:

```bash
git clone <repository-url>
cd VozUniversitaria
```

2. Restore the repository's local .NET tools and NuGet packages:

```bash
dotnet tool restore
dotnet restore
```

3. Create or update the local database by applying the EF Core migrations:

```bash
dotnet ef database update --project VozUniversitaria/VozUniversitaria.csproj
```

4. Build the application:

```bash
dotnet build
```

5. Run the application:

```bash
dotnet run --project VozUniversitaria/VozUniversitaria.csproj
```

5. Open the browser to the local URL shown in the terminal, usually:

```text
https://localhost:5001
```

If the app is running in development mode, the local port may vary depending on your machine configuration.

## Configuration

The project includes the default ASP.NET Core configuration files:

- `appsettings.json` — application settings
- `appsettings.Development.json` — development-specific settings
- `Properties/launchSettings.json` — local launch profiles

## Database and Entity Framework Core

The application uses SQL Server through Entity Framework Core. The connection string is
configured as `ConnectionStrings:DefaultConnection` in
`VozUniversitaria/appsettings.json` and points to the local development database
`VozUniversitariaDb` on `localhost:1433`.

The current data model contains the `Users` entity with the following roles:

- `Student`
- `Teacher`
- `Admin`
- `Staff`

The database includes unique indexes for `Email` and `CaseNumber`, and development seed
data for an administrator and a teacher. Passwords are stored as BCrypt hashes.

### Apply migrations

From the repository root, restore the local EF Core tool once, then apply migrations:

```bash
dotnet tool restore
dotnet ef database update --project VozUniversitaria/VozUniversitaria.csproj
```

To list the available migrations:

```bash
dotnet ef migrations list --project VozUniversitaria/VozUniversitaria.csproj
```

The project currently includes the following migrations:

1. `20260908045623_InitialCreate`
2. `20260918021207_AddUserConstraintsAndSeeding`
3. `20260918023553_FixSeedUserPasswordHash`
4. `20260922013733_AddIdentitySupport`

### Local SQL Server connection

When using the development container, configure a SQL Server instance at `localhost:1433`
and make sure the credentials match the connection string in `appsettings.json`. The
development password must not be reused in production.

To connect from VS Code (using the **SQL Server (mssql)** extension):

1. Open the SQL Server panel in the sidebar (database icon).
2. Create a new connection with the following details:
   - **Server name:** `localhost`
   - **Port:** `1433`
   - **Trust server certificate:** enabled (the container uses a self-signed certificate)
   - **Authentication type:** SQL Login
   - **User name:** `sa`
    - **Password:** use the password configured in `appsettings.json`
    - **Database name:** `VozUniversitariaDb`
3. Click "Connect".

## Development Notes

The current implementation is intentionally focused on authentication and the initial
user data model. It does not yet include:

- student and teacher profile entities
- courses and enrollments
- announcements and internal messages
- content management CRUD
- API layer
- external service integrations
- admin dashboard

The planned data model and future entities are documented in `architecture.md` and
`DATA-MODEL.md`.

## Contribution Guidelines

Contributions are welcome. To keep the project consistent and maintainable, please follow these guidelines:

1. Create a feature branch before making changes:

```bash
git checkout -b feature/your-feature-name
```

2. Keep changes focused and aligned with the project purpose.

3. Follow C# and ASP.NET Core coding conventions.

4. Add or update documentation when behavior or setup changes.

5. Validate the project before submitting a pull request:

```bash
dotnet build
```

6. Submit a pull request with a clear description of the change and its purpose.

## License

This project does not currently specify a license file. If you plan to share or distribute it publicly, consider adding an appropriate open-source license such as MIT or Apache 2.0.

## Maintainers

This project is currently in an early development stage and is best suited for continued extension by the project owner or development team.


