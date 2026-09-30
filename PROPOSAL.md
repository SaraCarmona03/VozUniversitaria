## 1. Project Overview

**VozUniversitaria** is a web application developed using ASP.NET Core MVC, designed to
centralize, manage, and distribute news, events, and articles of interest to the university
community.

According to the `README.md`, the implementation already includes user registration, cookie-based
login, role-based authorization, and SQL Server persistence via EF Core. The objectives
listed below outline the remaining work: management of content, courses, and academic tools.

### Main Objectives

* Provide an intuitive portal for students and faculty to post and view complaints, suggestions, and events.
* Expand role-based access control (Administrator, Editor, User) beyond authentication
to include content-level permissions.
* Standardize the development environment using Dev Containers
to avoid configuration discrepancies (already implemented; see `README.md` →
"Local connection to SQL Server").

---

## 2. Technology Stack

| Component | Technology | Version / Details |
| :--- | :--- | :--- |
| **Framework** | ASP.NET Core MVC | .NET 8.0 |
| **Language** | C# | 12.0 |
| **Database** | SQL Server (MSSQL) | Entity Framework Core 8 — implemented (3 migrations, seed data) |
| **Frontend** | Razor Views, HTML5, CSS3 | Bootstrap / Custom styles, jQuery |
| **Security** | BCrypt.Net-Next | Password hashing — implemented |
| **Version Control** | Git / GitFlow | Feature-branch workflow |
| **Environment** | VS Code Dev Containers | Debian-based Docker container (includes SQL Server 2019 container) |
---

## 3. Team and Contributors

| Name / GitHub Username | Role | Key Responsibilities |
| :--- | :--- | :--- |
| **SaraCarmona03** | Repository Owner / Lead | Project setup, architecture, and MVC core |
| **Vanesa03333** | Contributor | Database design and Entity Framework |
| **JesusManuelNoriegaVelazquez** | Contributor | Views, UI designs, and Razor logic |
| **chhah1020** | Contributor | Business logic and controllers |
| **dianabazaneUES** | Contributor | Authentication, testing, and QA |

---

## 4. Development Container Specification (*Dev Container*)

| Feature | Configuration |
| :--- | :--- |
| **Base image** | `mcr.microsoft.com/devcontainers/dotnet:1-8.0` |
| **Pre-installed tools** | .NET SDK 8.0, EF Core CLI tools, Git |
| **VS Code extensions** | C# Dev Kit, Razor Tooling, GitLens |

> SQL Server connection details (server, port, credentials, database name
> `VozUniversitariaDb`) and migration commands are documented in the `README.md` file
> —sections "Database and Entity Framework Core" and "Local SQL Server connection"—; they are not repeated here.

---

## 5. Project Roadmap (Product Level)

| Phase | Milestone | Status |
| :--- | :--- | :--- | | **Phase 1** | Repository and dev container setup | Completed |
| **Phase 2** | Database modeling and EF Core migrations (`Users`) | Completed |
| **Phase 3** | Authentication (registration, login, roles) | Completed |
| **Phase 4** | Academic domain: courses, enrollments, announcements, messaging (CRUD) | Scheduled |
| **Phase 5** | Testing and final launch | Scheduled |

> For the technical sequence of functional development (feature by feature), see `architecture.md` 3
> ("Recommendation on project evolution").