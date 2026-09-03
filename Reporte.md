#GitFlow

##ExecutedCommands

#1. Installing Git Flow
sudo apt update -o Acquire::AllowInsecureRepositories=true -o Acquire::AllowDowngradeToInsecureRepositories=true || true && sudo apt install -y git-flow

#2. Resolving Git safe directory permissions
git config --global --add safe.directory /workspaces/VozUniversitaria-master

#3. Configuring user email
git config --global user.email "23020220066@ues.mx"

#4. Configuring user name
git config --global user.name "reyna vanesa isaac vivanco"

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