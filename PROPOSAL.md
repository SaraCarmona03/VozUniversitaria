1. Project Overview

VozUniversitaria is a web application developed in ASP.NET Core MVC designed to centralize, manage, and distribute news, events, and articles of interest to the university community.

Per README.md, the implementation already includes user registration, cookie-based login, role-based authorization, and SQL Server persistence through EF Core. The objectives below describe what's still ahead: content management, courses, and academic tools.

Core Objectives
Provide an intuitive portal for students and faculty to publish and consult articles.
Extend role-based access control (Admin, Editor, User) beyond authentication into content permissions.
Standardize the development environment using Dev Containers to prevent configuration discrepancies (already in place — see README.md → "Local SQL Server connection").
2. Technical Stack
Component	Technology	Version / Details
Framework	ASP.NET Core MVC	.NET 8.0
Language	C#	12.0
Database	SQL Server (MSSQL)	Entity Framework Core 8 — implemented (3 migrations, seed data)
Frontend	Razor Views, HTML5, CSS3	Bootstrap / Custom Styles, jQuery
Security	BCrypt.Net-Next	Password hashing — implemented
Version control	Git / GitFlow	Feature-branch workflow
Environment	VS Code Dev Containers	Debian-based Docker Container (SQL Server 2019 container included)

Per README.md ("Development Notes"), what's still not implemented is content-domain functionality: student/teacher profile entities, courses, enrollments, announcements, messaging, CMS CRUD, an API layer, and the admin dashboard.

3. Team & Collaborators
Name / GitHub User	Role	Main Responsibilities
SaraCarmona03	Repository Owner / Lead	Project Setup, Architecture & MVC Core
Vanesa03333	Collaborator	Database Design & Entity Framework
JesusManuelNoriegaVelazquez	Collaborator	Views, UI Layouts & Razor Logic
chhah1020	Collaborator	Business Logic & Controllers
dianabazaneUES	Collaborator	Authentication, Testing & QA
4. Dev Container Specification
Feature	Configuration
Base Image	mcr.microsoft.com/devcontainers/dotnet:1-8.0
Pre-installed Tools	.NET SDK 8.0, EF Core CLI Tools, Git
VS Code Extensions	C# Dev Kit, Razor Tooling, GitLens

SQL Server connection details (server, port, credentials, database name VozUniversitariaDb) and migration commands are documented in README.md → "Database and Entity Framework Core" and "Local SQL Server connection" — not repeated here.

5. Project Roadmap (product-level)
Phase	Milestone	Status
Phase 1	Repository Setup & Dev Container Config	Completed
Phase 2	Database Modeling & EF Core Migrations (Users)	Completed
Phase 3	Authentication (registration, login, roles)	Completed
Phase 4	Academic domain: courses, enrollments, announcements, messaging (CRUD)	Scheduled
Phase 5	Testing & final release	Scheduled
