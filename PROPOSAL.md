# 📌 Project Proposal: VozUniversitaria

> **Repository:** VozUniversitaria  
> **Course:** Software Engineering / Web Development  
> **Dev Environment:** VS Code Dev Container (.NET 8 SDK)

---

## 1. Project Overview

**VozUniversitaria** is a web application developed in ASP.NET Core MVC designed to centralize, manage, and distribute news, events, and articles of interest to the university community.

### Core Objectives
* Provide an intuitive portal for students and faculty to publish and consult articles.
* Implement role-based access control (Admin, Editor, User).
* Standardize the development environment using Dev Containers to prevent configuration discrepancies.

---

## 2. Technical Stack

| Component | Technology | Version / Details |
| :--- | :--- | :--- |
| **Framework** | ASP.NET Core MVC | .NET 8.0 |
| **Language** | C# | 12.0 |
| **Database** | SQL Server (MSSQL) | Entity Framework Core 8 |
| **Frontend** | Razor Views, HTML5, CSS3 | Bootstrap / Custom Styles |
| **Environment** | VS Code Dev Containers | Debian-based Docker Container |

---

## 3. Team & Collaborators

| Name / GitHub User | Role | Main Responsibilities |
| :--- | :--- | :--- |
| **SaraCarmona03** | Repository Owner / Lead | Project Setup, Architecture & MVC Core |
| **Vanesa03333** | Collaborator | Database Design & Entity Framework |
| **JesusManuelNoriegaVelazquez** | Collaborator | Views, UI Layouts & Razor Logic |
| **chhah1020** | Collaborator | Business Logic & Controllers |
| **dianabazaneUES** | Collaborator | Authentication, Testing & QA |

---

## 4. System Architecture & Features

### Key Modules
1. **User Authentication:** Registration, login, and session management.
2. **Content Management System (CMS):** Create, read, update, and delete articles/news (CRUD).
3. **Category & Tagging System:** Content classification by faculties or topics of interest.
4. **Data Persistence:** Relational mapping with Entity Framework Core to SQL Server.

---

## 5. Dev Container Specification

The standardized development environment for this project uses **Dev Containers** to ensure idempotency across all developers:

| Feature | Configuration |
| :--- | :--- |
| **Base Image** | `mcr.microsoft.com/devcontainers/dotnet:1-8.0` |
| **Pre-installed Tools** | .NET SDK 8.0, EF Core CLI Tools, Git |
| **VS Code Extensions** | C# Dev Kit, Razor Tooling, GitLens |

---

## 6. Project Roadmap

| Phase | Milestone | Status |
| :--- | :--- | :--- |
| **Phase 1** | Repository Setup & Dev Container Config | ✅ Completed |
| **Phase 2** | Database Modeling & EF Core Migrations | ⏳ In Progress |
| **Phase 3** | Controller & View Development (CRUD) | 📅 Scheduled |
| **Phase 4** | Testing, Authentication & Final Release | 📅 Scheduled |