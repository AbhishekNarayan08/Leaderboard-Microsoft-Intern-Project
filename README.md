# 🏆 Leaderboard-Microsoft-Intern-Project

This repository contains the **Leaderboards** solution developed for the Microsoft Internship Project. It provides a full-stack implementation, including backend APIs, frontend web app, automated testing, and CI/CD pipelines.

---

## 📁 Repository Structure

```
.
├── AzureDevopsPipelines/     # YAML pipeline definitions for CI/CD
├── LeaderboardAPIs/          # ASP.NET Core Web API project for leaderboard services
├── WebAppLeaderboard/        # Front-end web application for displaying leaderboards
├── LeaderboardTest/          # Integration tests for API endpoints
├── Src/                      # Primary source code (shared libraries or common modules)
├── Tests/                    # Unit test projects for backend logic
├── Reputation.sln            # Visual Studio solution file
├── .editorconfig             # Editor configuration
├── .gitattributes            # Git attributes
├── .gitignore                # Git ignore rules
└── NuGet.config              # NuGet package source configuration
```

---

## 🚀 Getting Started

### Prerequisites

- **.NET SDK 6.0+** (Download: https://dotnet.microsoft.com/download)  
- **Visual Studio 2022** or **Visual Studio Code** with C# extensions  
- **Node.js** (if front-end uses npm/yarn in `WebAppLeaderboard`)  

### Building the Solution

1. Clone the repository  
   ```bash
   git clone https://github.com/AbhishekNarayan08/Leaderboard-Microsoft-Intern-Project.git
   cd Leaderboard-Microsoft-Intern-Project
   ```

2. Open the solution  
   - Launch `Reputation.sln` in Visual Studio **or**  
   - Run from CLI:
     ```bash
     dotnet build Reputation.sln
     ```

### Running the API

1. Navigate to the API project  
   ```bash
   cd LeaderboardAPIs
   ```
2. Restore packages and run  
   ```bash
   dotnet restore
   dotnet run
   ```
3. API will be available at `https://localhost:5001` (Swagger UI at `/swagger`).

### Running the Front-end

1. Navigate to front-end folder  
   ```bash
   cd WebAppLeaderboard
   ```
2. Install dependencies and start  
   ```bash
   npm install
   npm start
   ```
3. Visit `http://localhost:3000` to view the leaderboard UI.

---

## 🔍 Testing

### Unit Tests

Run unit tests from CLI:
```bash
cd Tests
dotnet test
```

### Integration Tests

Execute API integration tests:
```bash
cd LeaderboardTest
dotnet test
```

---

## ⚙️ CI/CD Pipelines

Configured under `AzureDevopsPipelines/`:
- **Build**: Compiles solution, runs tests  
- **Release**: Deploys API and front-end to Azure App Service (configure your service connections)

---

## 📈 Features & Components

- **LeaderboardAPIs**: RESTful endpoints for retrieving and updating user scores  
- **WebAppLeaderboard**: Responsive web UI for displaying top performers  
- **Tests**: Ensures code quality via unit and integration tests  
- **CI/CD**: Automated build and deploy pipelines using Azure DevOps

---

## 📄 API Documentation

Visit `/swagger` after running the API to explore all endpoints, example requests, and responses.

---

## 👤 Author

**Abhishek Narayan**  
Microsoft Intern Project – IIT Delhi  

---

*For questions or contributions, please open an issue or submit a pull request.*  
