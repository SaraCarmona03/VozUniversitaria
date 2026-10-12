# Backend Framework – VozUniversitaria

**Framework:** ASP.NET Core 8 (MVC) · **Language:** C# · **Database:** SQL Server via Entity Framework Core · **Auth:** ASP.NET Core Identity

This document shows that the backend runs and explains how to start it, how the project is organized, and how a request is handled.

---

## 1. Why ASP.NET Core

| Reason | Detail |
|--------|--------|
| Batteries included | Routing, dependency injection, model binding, validation, authentication and Razor views come with the framework. |
| Identity built in | ASP.NET Core Identity gives us users, roles, password hashing, lockout and cookie sessions without writing them from scratch. |
| Strong tooling | `dotnet` CLI scaffolds, builds, tests and runs the app; `dotnet ef` manages database migrations. |
| Typed end to end | C# models, view models and EF Core entities catch many mistakes at compile time. |

The app was scaffolded with the framework's own tool (`dotnet new mvc`) and runs on **Kestrel**, the built-in web server.

---

## 2. How to start it

### 2.1 Prerequisites

The project runs inside the **Dev Container** (`.devcontainer/`), which provides the .NET 8 SDK, a SQL Server container and a local mail catcher (MailHog). Open the repository in VS Code and choose **Reopen in Container**.

### 2.2 First-time setup

```bash
# from the repository root
dotnet tool restore            # installs dotnet-ef from .config/dotnet-tools.json
cd VozUniversitaria
dotnet ef database update      # applies the EF Core migrations to SQL Server
```

Configuration lives in `VozUniversitaria/appsettings.json` and `appsettings.Development.json`:

| Setting | Purpose |
|---------|---------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection used by `AppDbContext` |
| `AllowedEmailDomains` | Institutional email domains accepted at registration |

SMTP credentials for the one-time code (OTP) emails are **not** stored in the repository; each developer sets them with `dotnet user-secrets`.

### 2.3 Run

```bash
cd VozUniversitaria
dotnet build      # compile
dotnet run        # start Kestrel
```

Expected output (abridged):

```text
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5032
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

Open `http://localhost:5032`. The default route sends anonymous visitors to the login page.

### 2.4 Tests

```bash
# from the repository root
dotnet test VozUniversitaria.Tests
```

Expected result: `Passed! - Failed: 0, Passed: 4, Skipped: 0, Total: 4`.

---

## 3. Project organization

```text
VozUniversitaria/                  (repository root)
├── VozUniversitaria/              ← the ASP.NET Core MVC web app
│   ├── Program.cs                 entry point: services + middleware pipeline
│   ├── appsettings*.json          configuration
│   ├── Controllers/               handle requests (AccountController, HomeController)
│   ├── Models/                    domain entities (Users, UserRole)
│   ├── ViewModels/                data shapes for forms (Login, Register, VerifyEmail)
│   ├── Views/                     Razor templates (.cshtml): Account/, Home/, Shared/
│   ├── Services/                  business logic (OtpService, SmtpEmailSender, ...)
│   ├── Data/                      EF Core DbContext (AppDbContext)
│   ├── Migrations/                database schema history
│   └── wwwroot/                   static files: css, js, libraries
├── VozUniversitaria.Tests/        automated tests
├── .devcontainer/                 reproducible dev environment
└── architecture.md, PROPOSAL.md   design documents
```

| Folder | Role | MVC letter |
|--------|------|-----------|
| `Models/` + `Data/` | The data and how it is stored | **M**odel |
| `Views/` | What the user sees (HTML generated from C# data) | **V**iew |
| `Controllers/` | Receives the request, coordinates, chooses the response | **C**ontroller |
| `Services/` | Reusable logic kept out of controllers | – |

**Dependency injection.** `Program.cs` registers the services, and the framework hands them to any class that asks for them in its constructor:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Users, IdentityRole<int>>(...)
       .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<OtpService>();
```

`AccountController` simply declares `UserManager<Users>`, `SignInManager<Users>`, `OtpService` and so on in its constructor, and ASP.NET Core supplies them.

---

## 4. How a request is handled

### 4.1 The middleware pipeline

Every request passes through the same ordered pipeline configured in `Program.cs`:

```csharp
app.UseStaticFiles();        // serves /css, /js, /lib directly, without reaching a controller
app.UseRouting();            // decides which controller action matches the URL
app.UseAuthentication();     // reads the cookie: who is this?
app.UseAuthorization();      // checks [Authorize]: may they do this?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
```

The order matters: authentication must run before authorization, and both after routing.

### 4.2 Walkthrough: signing in (`POST /Account/Login`)

```text
Browser
  │  POST /Account/Login   (email, password)
  ▼
Kestrel web server
  ▼
Middleware pipeline ─ static files? no ─ routing ─ authentication ─ authorization
  ▼
Routing: pattern {controller}/{action} → AccountController.Login (POST)
  ▼
Model binding: form fields → LoginViewModel, validated by data annotations
  ▼
AccountController.Login(LoginViewModel model)
  ├─ UserManager.FindByEmailAsync        ← EF Core → SQL Server
  ├─ checks: active account, verified email
  ├─ SignInManager.CheckPasswordSignInAsync (hash check + lockout)
  └─ SignInManager.SignInAsync           → issues the auth cookie
  ▼
Result: RedirectToAction("Index", "Home")  →  HTTP 302 + Set-Cookie
  ▼
Browser follows the redirect: GET /Home/Index (now with the cookie)
  ▼
Authentication middleware reads the cookie → user identified
[Authorize] on HomeController.Index passes → View() is rendered by Razor
  ▼
HTML response (Views/Home/Index.cshtml inside Views/Shared/_Layout.cshtml)
```

### 4.3 The same steps mapped to code

| Step | Where in the code |
|------|-------------------|
| Route matching | `app.MapControllerRoute(...)` in `Program.cs` |
| Controller + action | `AccountController.Login` (`[HttpPost]`) |
| Model binding + validation | `LoginViewModel` parameter, `ModelState.IsValid` |
| Business rules | `UserManager`, `SignInManager`, `OtpService` (injected) |
| Data access | `AppDbContext` through Identity's EF Core stores |
| Response | `View(model)`, `RedirectToAction(...)`, `TempData` messages |
| Rendering | Razor views in `Views/` using `_Layout.cshtml` |

### 4.4 Protected pages

`HomeController.Index` carries `[Authorize]`. A request without a valid cookie never reaches the action: the framework redirects to `LoginPath` (`/Account/Login`), configured with `ConfigureApplicationCookie`.

---

## 5. Evidence that it runs

To reproduce what the team ran:

```bash
cd VozUniversitaria
dotnet build        # → Build succeeded. 0 Warning(s) 0 Error(s)
dotnet run          # → Now listening on: http://localhost:5032
```

Then, in the browser:

1. `http://localhost:5032` redirects to the **Login** page.
2. **Register** with an institutional email: select a role, fill the form, receive the 6-digit code.
3. **Verify** the code, then **sign in**: the home page loads.
4. Open `/Home/Index` in a private window without signing in: it redirects back to login (authorization working).

---

## 6. Quick reference

| Task | Command |
|------|---------|
| Restore tools | `dotnet tool restore` |
| Apply migrations | `dotnet ef database update` |
| Create a migration | `dotnet ef migrations add <Name>` |
| Build | `dotnet build` |
| Run | `dotnet run` |
| Test (from repo root) | `dotnet test VozUniversitaria.Tests` |