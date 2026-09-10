# 📌 Project Proposal: VozUniversitaria

> **Repository:** VozUniversitaria  
> **Course:** Software Engineering / Web Development  
> **Dev Environment:** VS Code Dev Container (.NET 8 SDK)

---

## 1. Project Overview

**VozUniversitaria** es una plataforma web desarrollada en ASP.NET Core MVC diseñada para centralizar, gestionar y difundir noticias, eventos y artículos de interés para la comunidad universitaria.

### Core Objectives
* Proporcionar un portal intuitivo para que los estudiantes y docentes publiquen y consulten artículos.
* Implementar control de acceso basado en roles (Administrador, Redactor, Usuario).
* Estandarizar el entorno de desarrollo mediante Dev Containers para evitar discrepancias de configuración.

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
1. **User Authentication:** Registro, inicio de sesión y manejo de sesiones.
2. **Content Management System (CMS):** Crear, editar, publicar y eliminar artículos/noticias (CRUD).
3. **Category & Tagging System:** Clasificación de contenido por facultades o temas de interés.
4. **Data Persistence:** Mapeo relacional con Entity Framework Core hacia SQL Server.

---

## 5. Dev Container Specification

El entorno de desarrollo estandarizado del proyecto utiliza **Dev Containers** para garantizar idempotencia entre los desarrolladores:

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