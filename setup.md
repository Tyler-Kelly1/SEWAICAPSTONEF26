# Development Setup Guide

This document provides instructions for setting up and running the **Overload** full-stack development environment locally.

---

## 🛠️ Required Technologies & Prerequisites

Before setting up the project, ensure you have installed the following required software:

| Technology | Recommended / Required Version | Download / Documentation Link |
| :--- | :--- | :--- |
| **Docker / Docker Desktop** | Docker 20.10+ / Docker Compose v2+ | [Download Docker Desktop](https://www.docker.com/products/docker-desktop/) |
| **.NET SDK** | .NET 10.0 SDK (or .NET 8.0+) | [Download .NET SDK](https://dotnet.microsoft.com/download) |
| **Node.js** | Node.js v18+ or v20+ LTS (Node v24 supported) | [Download Node.js](https://nodejs.org/) |
| **Git** | Latest Stable | [Download Git](https://git-scm.com/) |

---

## 🏗️ Tech Stack Overview

- **Database:** PostgreSQL 16 (Containerized via Docker)
- **Backend:** .NET 10.0 Core Web API with Entity Framework Core & Npgsql
- **Frontend:** Vue 3 + Vuetify 3 built with Vite

---

## 🚀 Quick Start (Automated Scripts)

The repository provides automated startup scripts to launch Docker, the Backend, and the Frontend simultaneously.

### On Windows
Run the Windows batch file in Command Prompt or PowerShell:
```cmd
.\start.bat
```

### On Linux / macOS / Git Bash
Make the script executable (if needed) and run:
```bash
chmod +x start.sh
./start.sh
```

---

## 🔧 Manual Step-by-Step Setup Guide

If you prefer to start each service manually for development or debugging, follow these steps:

### 1. Start the Containerized PostgreSQL Database
From the root directory of the project, run:
```bash
docker compose up -d
```
*(Alternatively, use `docker-compose up -d` if using older Docker Compose versions).*

This launches a PostgreSQL 16 container running on port `5432` with database `overloaddb`.

### 2. Set Up & Run the .NET Core Backend
Navigate to the `backend/` directory:
```bash
cd backend
```

Restore dependencies and run the API:
```bash
dotnet restore
dotnet run
```
The API backend will start listening at:
- **HTTP:** [http://localhost:5245](http://localhost:5245)
- **Swagger / OpenAPI Documentation:** [http://localhost:5245/swagger](http://localhost:5245/swagger)

*(Note: Database migrations will run or be applied automatically via EF Core on startup or via `dotnet ef database update`).*

### 3. Set Up & Run the Vuetify Frontend
Navigate to the `frontend/` directory in a new terminal:
```bash
cd frontend
```

Install npm package dependencies:
```bash
npm install
```

Start the Vite development server:
```bash
npm run dev
```
The frontend UI will be available at:
- **Vite Dev Server:** [http://localhost:5173](http://localhost:5173)

## 🧪 Test Suite Execution

The repository contains test suites for endpoint testing, frontend unit testing, and full-stack UI testing:

### 1. Backend Endpoint Tests (MSTest)
Run the .NET MSTest suite for backend controllers and engines:
```bash
dotnet test tests/Backend.Tests/Backend.Tests.csproj
```

### 2. Frontend JS Tests (Vitest)
Run the Vitest suite for frontend API services and components:
```bash
cd frontend
npm test
```
To run Vitest in interactive watch mode:
```bash
npm run test:watch
```

### 3. Full Stack Interactive UI Tests (Selenium + MSTest)
Run the Selenium UI tests (requires Chrome installed; defaults to headless mode):
```bash
dotnet test tests/Selenium.Tests/Selenium.Tests.csproj
```

### 4. Run All .NET Test Suites
To run all .NET test suites (Backend + Selenium UI tests):
```bash
dotnet test Overload.slnx
```

---

## 🔍 Verification & Troubleshooting

1. **Verify Database Container:**
   Run `docker ps` to ensure `overload_postgres` is running.
2. **Verify Backend API:**
   Navigate to `http://localhost:5245/api/TestTable` to check the GET endpoint response.
3. **Verify Frontend UI:**
   Navigate to `http://localhost:5173` to test adding and displaying items in `TEST_TABLE`.

