# University Voice
> **"Your voice drives change."**

## 1. Project Description
**University Voice** is a web platform designed for the entire university community. The system enables students, faculty, administrative staff, and other institutional members to submit and track:
* Complaints
* Suggestions
* Reports
* Commendations

The primary goal is to establish an organized, transparent communication channel between the community and the institution's various departments, providing mechanisms for tracking, managing, and analyzing information.

## 2. Technologies and Tools
* **Frontend:** React
* **Backend:** .NET (C#) and Node.js (TypeScript)
* **Database:** MS SQL Server
* **ORM:** Entity Framework Core (for .NET) / Corresponding tool for Node.js
* **Authentication:** JWT
* **Version Control:** Git + GitHub
* **Editor:** Visual Studio Code
* **AI (Optional):** Ollama + Qwen

## 3. System Architecture
The updated architecture operates as follows:
1. The **User** interacts with the **Frontend (React)**.
2. The Frontend communicates via **HTTP/JSON** with the **Backend ecosystem (.NET with C# and Node.js with TypeScript)**, enabling a microservices-oriented architecture or a separation of concerns.
3. The Backend queries or modifies data centralized in **MS SQL Server**.
4. (Optional) The Backend queries the AI ​​(**Ollama/Qwen**) for report classification.

## 4. User Roles
* **Student and Faculty:** Can create posts, view content, comment, vote, attach evidence, and check the status of their reports.
* **Administrative Staff:** Can view reports relevant to their area, track progress, and provide responses.
* **Moderator:** Reviews, classifies, and hides inappropriate content.
* **Administrator:** Has full control. Manages users, categories, roles, and departments, and reviews global statistics via a dashboard. ## 5. Post Types and Privacy
The system handles different privacy levels to protect identity when necessary:
* **Public:** Visible to all users.
* **Anonymous:** Content is visible, but the author remains hidden.
* **Private:** Visible only to the creator and the responsible department.

## 6. Minimum Viable Product (MVP)
For the first phase of the project, development will focus strictly on:
1. Registration and login (JWT-based authentication).
2. Role management.
3. Creating and viewing posts.
4. Viewing details, editing, and deleting one's own posts.
5. Commenting functionality.
6. Management of categories and report statuses (New, Under Review, Assigned, In Progress, Resolved).
7. Functional database (MS SQL Server) and REST API.

## 7. Suggested Development Phases
1. **Planning and Setup:** Requirement definition, VS Code setup, GitHub repository creation.
2. **Database:** Creation of main tables (Users, Posts, Comments, etc.) in MS SQL Server.
3. **Backend:** API creation using .NET (C#) and Node.js (TypeScript), post CRUD operations, and authentication system.
4. **Frontend:** Interface development in React; login, feed, and dashboard screens.
5. **Integration and Testing:** Connecting React to the APIs, role testing, and MVP validation.


#GitFlow

##ExecutedCommands

#1. Installing Git Flow
sudo apt update -o Acquire::AllowInsecureRepositories=true -o Acquire::AllowDowngradeToInsecureRepositories=true || true && sudo apt install -y git-flow

#2. Resolving Git safe directory permissions
git config --global --add safe.directory /workspaces/VozUniversitaria-master

#3. Configuring user email
git config --global user.email "UniversityVoice@ues.mx"

#4. Configuring user name
git config --global user.name "University Voice"

#5. Staging files for the first commit
git add .

#6. Creating the initial commit on the base branch
git commit -m "Initial commit"

#7. Force-initializing Git Flow (accepting default values ​​with Enter)
git flow init -f

#8. Creating and starting the feature branch
git flow feature start login

# 9. Staging changes (clean working tree)
git add .

#10. Finishing the feature and automatically merging into develop
git flow feature finish login
```

* Commits: I created this when saving the initial state of my project before starting the flow, using `git commit -m "Initial commit"`.

* Branches: The main branches (`master` and `develop`) were created automatically when running `git flow init -f`, and I created the `feature/login` branch by executing `git flow feature start login`.

* Merges: This happened automatically when I closed the feature using `git flow feature finish login`, which merged all the code from the `feature/login` branch into `develop`.

* Pull Requests: These are not performed via terminal commands; they are simulated locally when finishing the feature (in a real project, they would be handled via the GitHub website after pushing the branch with `git push`).