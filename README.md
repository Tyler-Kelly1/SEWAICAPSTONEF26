# SEWAICAPSTONEF26
Fall 2026, Software Engineering with AI Capstone Project

*Touched by Antigravity*

## Setup & Development
For complete instructions on setting up your local environment, required software versions, and running the project, please see [`setup.md`](file:///C:/Users/tyler/Overload/setup.md).

### Quick Launch Entry Points
- **Development (Full App):** Run `.\start.bat` (Windows) or `./start.sh` (Linux/macOS)
- **Production Container Stack (Docker Compose):** `docker compose -f docker-compose.prod.yml up -d --build` (Web App on [http://localhost](http://localhost), Backend on [http://localhost:5245](http://localhost:5245))
- **PostgreSQL Admin Dashboard (pgAdmin 4):** Run `.\start-db-admin.bat` (Windows) or `./start-db-admin.sh` (Linux/macOS) — accessible at [http://localhost:5050](http://localhost:5050)
- **Backend API:** [http://localhost:5245](http://localhost:5245) (Swagger: [http://localhost:5245/swagger](http://localhost:5245/swagger), Health: [http://localhost:5245/health](http://localhost:5245/health))
- **Frontend App:** [http://localhost:5173](http://localhost:5173) (Dev) or [http://localhost](http://localhost) (Containerized Prod)



