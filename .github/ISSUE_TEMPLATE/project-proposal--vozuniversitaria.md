---
name: 'Project Proposal: VozUniversitaria'
about: "> Repository: VozUniversitaria   >Course: Software Engineering / Web Development
  \  >Dev Environment:VS Code Dev Container (.NET 8 SDK)"
title: ''
labels: ''
assignees: SaraCarmona03

---

### 1. Project Overview & Objectives

| Objective | Description |
| --- | --- |
| **Central Portal** | Provide an intuitive portal for students and faculty to publish and consult articles. |
| **Access Control** | Implement role-based access control (Admin, Editor, User). |
| **Environment Standardization** | Standardize the development environment using Dev Containers to prevent configuration discrepancies. |

---

### 2. Technical Stack

| Component | Technology | Version / Details |
| --- | --- | --- |
| **Framework** | ASP.NET Core MVC | .NET 8.0 |
| **Language** | C# | 12.0 |
| **Database** | SQL Server (MSSQL) | Entity Framework Core 8 |
| **Frontend** | Razor Views, HTML5, CSS3 | Bootstrap / Custom Styles |
| **Environment** | VS Code Dev Containers | Debian-based Docker Container |

---

### 3. Team & Collaborators

| Name / GitHub User | Role | Main Responsibilities |
| --- | --- | --- |
| **SaraCarmona03** | Repository Owner / Lead | Project Setup, Architecture & MVC Core |
| **Vanesa03333** | Collaborator | Database Design & Entity Framework |
| **JesusManuelNoriegaVelazquez** | Collaborator | Views, UI Layouts & Razor Logic |
| **chhah1020** | Collaborator | Business Logic & Controllers |
| **dianabazaneUES** | Collaborator | Authentication, Testing & QA |

---

### 4. System Architecture & Features

| Key Module | Description |
| --- | --- |
| **User Authentication** | Registration, login, and session management. |
| **Content Management System (CMS)** | Create, read, update, and delete articles/news (CRUD). |
| **Category & Tagging System** | Content classification by faculties or topics of interest. |
| **Data Persistence** | Relational mapping with Entity Framework Core to SQL Server. |

---

### 5. Dev Container Specification

| Feature | Configuration |
| --- | --- |
| **Base Image** | `[mcr.microsoft.com/devcontainers/dotnet:1-8.0](https://mcr.microsoft.com/devcontainers/dotnet:1-8.0)` |
| **Pre-installed Tools** | .NET SDK 8.0, EF Core CLI Tools, Git |
| **VS Code Extensions** | C# Dev Kit, Razor Tooling, GitLens |

---

### 6. Project Roadmap

| Phase | Milestone | Status |
| --- | --- | --- |
| **Phase 1** | Repository Setup & Dev Container Config | ✅ Completed |
| **Phase 2** | Database Modeling & EF Core Migrations | ⏳ In Progress |
| **Phase 3** | Controller & View Development (CRUD) | 📅 Scheduled |
| **Phase 4** | Testing, Authentication & Final Release | 📅 Scheduled |
