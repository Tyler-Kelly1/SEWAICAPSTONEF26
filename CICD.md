# Overload CI/CD Pipeline & Development Guide

> [!IMPORTANT]
> **Production Live URL:** The Overload application is live and accessible at **[http://overload.quest](http://overload.quest)**.
> All code merged into the `main` branch automatically triggers the end-to-end testing, containerization, and production deployment pipeline on AWS EC2.

---

## 🏛️ System & Pipeline Architecture

The application is deployed as a multi-container Docker stack hosted on an Amazon Web Services (AWS) EC2 instance running Amazon Linux 2023, automated via GitHub Actions:

```
[ Developer / Feature Branch ]
               │
               ▼  (PR / Push to main)
      [ GitHub Actions CI/CD ]
         ├── 1. Test Suite (55 Backend MSTests + 62 Frontend Vitests)
         ├── 2. Build & Verify Docker Images (Buildx)
         └── 3. Deploy to AWS EC2 (SSH + rsync + Docker Compose)
               │
               ▼
      [ AWS EC2 Instance (Amazon Linux 2023 + Elastic IP) ]
         ├── Reverse Proxy: Nginx Container (Port 80) ──▶ http://overload.quest
         ├── Frontend: Vue 3 + Vuetify SPA
         ├── Backend: .NET 10.0 Web API Container (Port 5245 / Kestrel:8080)
         ├── Database: PostgreSQL 16 Container (Port 5432)
         └── Admin: pgAdmin 4 Container (Port 5050)
```

---

## 💻 1. Local Development Setup

### 1.1 Prerequisites & Required Tools

Ensure the following tools are installed on your workstation:

| Technology | Recommended Version | Download Link |
| :--- | :--- | :--- |
| **Docker Desktop / Engine** | Docker 20.10+ & Compose v2+ | [docker.com](https://www.docker.com/products/docker-desktop/) |
| **.NET SDK** | .NET 10.0 SDK | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| **Node.js** | Node.js v22 LTS (v18+ supported) | [nodejs.org](https://nodejs.org/) |
| **Git** | Latest Stable | [git-scm.com](https://git-scm.com/) |

### 1.2 Environment Configuration

Create a local `.env` file in the root of the project by copying [.env.example](file:///C:/Users/tyler/Overload/.env.example):
```bash
cp .env.example .env
```

Ensure the database settings match your local environment:
```dotenv
DB_NAME=overloaddb
DB_USER=postgres
DB_PASSWORD=postgrespassword
DB_PORT=5432
PGADMIN_PORT=5050
PGADMIN_DEFAULT_EMAIL=admin@example.com
PGADMIN_DEFAULT_PASSWORD=admin
```

### 1.3 Quick Start (Automated Scripts)

The repository provides one-command startup scripts to launch Docker, the Backend, and the Frontend concurrently:

- **On Windows:**
  ```cmd
  .\start.bat
  ```
- **On Linux / macOS:**
  ```bash
  chmod +x start.sh
  ./start.sh
  ```

### 1.4 Manual Local Service Startup

To run services individually for targeted debugging:

1. **Start PostgreSQL & pgAdmin in Docker:**
   ```bash
   docker compose up -d
   ```
2. **Run Backend Web API (.NET 10):**
   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```
   API Swagger is accessible at: [http://localhost:5245/swagger](http://localhost:5245/swagger)  
   Health check endpoint: [http://localhost:5245/health](http://localhost:5245/health)
3. **Run Frontend Dev Server (Vue 3 + Vite):**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
   Local UI is accessible at: [http://localhost:5173](http://localhost:5173)

### 1.5 Local Test Suite Execution

All features must pass tests before committing code (see [`rules.md`](file:///C:/Users/tyler/Overload/rules.md)):

```bash
# Backend Controller & Engine Tests (MSTest)
dotnet test tests/Backend.Tests/Backend.Tests.csproj

# Frontend Unit & Component Tests (Vitest)
npm test --prefix frontend

# Full-Stack Interactive UI Tests (Selenium + ChromeDriver)
dotnet test tests/Selenium.Tests/Selenium.Tests.csproj
```

---

## 🚀 2. CI/CD Pipeline (`.github/workflows/deploy.yml`)

The production pipeline is managed with GitHub Actions in [.github/workflows/deploy.yml](file:///C:/Users/tyler/Overload/.github/workflows/deploy.yml).

### 2.1 Trigger Events
- **Push to `main`:** Automatically runs tests, builds containers, and deploys live to EC2.
- **Pull Request to `main`:** Runs tests and container validation checks without deploying.
- **Manual Trigger (`workflow_dispatch`):** Allows authorized maintainers to trigger deployment manually from GitHub's **Actions** tab.
- **Branch Isolation:** Pushing to non-main branches (such as `staging` or `feature/*`) does **not** trigger production deployment, protecting live users.

### 2.2 Pipeline Jobs & Stages

#### Stage 1: `test` (Automated Test Suites)
Runs on `ubuntu-latest`:
1. Checks out repository code.
2. Configures .NET 10.0 SDK and restores dependencies.
3. Executes backend MSTests (55 unit & integration tests).
4. Configures Node 22 LTS with npm caching.
5. Installs dependencies via `npm ci` and runs frontend Vitests (62 unit tests).
6. **Gate:** If any test fails, the entire workflow stops immediately.

#### Stage 2: `build-containers` (Container Image Verification)
Runs after `test` passes:
1. Configures Docker Buildx.
2. Builds `backend/Dockerfile` and `frontend/Dockerfile`.
3. Verifies container compilation without syntax or packaging errors before touching production.

#### Stage 3: `deploy-ec2` (Production Deployment)
Runs only on pushes to `main`:
1. Connects to the AWS EC2 instance over SSH using the GitHub Repository Secrets:
   - `EC2_HOST`: Target connection string (e.g., `ec2-user@<Elastic-IP>` or `ec2-user@overload.quest`).
   - `EC2_SSH_KEY`: The private `.pem` key pair for the EC2 instance.
2. Normalizes line endings on the private key to prevent Windows CRLF syntax errors.
3. Synchronizes repository source files to `~/overload/` on the instance using `rsync` over SSH, excluding development artifacts (`.git`, `node_modules`, `bin`, `obj`).
4. Executes the production multi-container stack on the EC2 instance:
   ```bash
   # Build sequentially to conserve RAM on micro instances
   docker compose -f docker-compose.prod.yml build --progress=plain backend
   docker compose -f docker-compose.prod.yml build --progress=plain frontend

   # Recreate and run containers in background
   docker compose -f docker-compose.prod.yml up -d

   # Prune dangling build images to save disk space
   docker system prune -f

   # Print live container status
   docker compose -f docker-compose.prod.yml ps
   ```

---

## 🌐 3. Production Infrastructure & AWS Setup

### 3.1 Live Production Endpoint
- **Domain:** **[http://overload.quest](http://overload.quest)**
- **Reverse Proxy:** Nginx container routing port `80` to the Vue 3 static distribution and proxying `/api/*` requests to the .NET 10 Kestrel backend.

### 3.2 AWS EC2 Instance Specs & Memory Optimization
- **Operating System:** Amazon Linux 2023
- **Instance Type:** `t3.micro` / `t2.micro` (1 vCPU, 1 GB RAM)
- **Swap Space Requirement:** Because compiling .NET 10 SDK and Node/Vite in Docker requires >1 GB of RAM during container builds, the instance is configured with a **2 GB swap file** to prevent Out-Of-Memory (OOM) freezing:
  ```bash
  sudo dd if=/dev/zero of=/swapfile bs=128M count=16
  sudo chmod 600 /swapfile
  sudo mkswap /swapfile
  sudo swapon /swapfile
  echo '/swapfile swap swap defaults 0 0' | sudo tee -a /etc/fstab
  ```

### 3.3 AWS Networking & Security Group Rules
- **Elastic IP (Static IPv4):** An AWS Elastic IP is associated with the EC2 instance, providing a permanent static IPv4 address that persists across instance restarts.
- **DNS `A` Record:** The domain `overload.quest` points to the instance's Elastic IP.
- **Inbound Security Group Rules:**
  - `HTTP` (Port 80) ➔ `0.0.0.0/0` (Public web traffic to Nginx)
  - `SSH` (Port 22) ➔ `0.0.0.0/0` (Protected by RSA/ED25519 `.pem` key pair for administrator and GitHub Actions runner access)

### 3.4 GitHub Secrets Configuration
Under **Repository Settings > Secrets and variables > Actions**:
- `EC2_HOST`: `ec2-user@<YOUR-ELASTIC-IP>` (or `ec2-user@overload.quest`)
- `EC2_SSH_KEY`: The full contents of the `.pem` private key file (including header and footer).

---

## 🛠️ 4. Maintenance & Operations Cheatsheet

### 4.1 Checking Production Logs on EC2
```bash
ssh -i /path/to/key.pem ec2-user@overload.quest

# View running containers
docker ps

# Follow live backend logs
docker logs -f overload_backend

# Follow live frontend / nginx proxy logs
docker logs -f overload_frontend

# View Postgres logs
docker logs -f overload_postgres
```

### 4.2 Restarting Services Manually on EC2
```bash
cd ~/overload
docker compose -f docker-compose.prod.yml restart
```

### 4.3 Database Backups
```bash
# Dump database from the container
docker exec -t overload_postgres pg_dump -U postgres overloaddb > backup_$(date +%F).sql
```
