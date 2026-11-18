@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo  Starting PostgreSQL Admin Dashboard
echo ==========================================

:: Navigate to script directory
cd /d "%~dp0"

:: 1. Start Docker Containers (PostgreSQL & pgAdmin)
echo [1/2] Ensuring PostgreSQL and pgAdmin containers are running...
where docker-compose >nul 2>&1
if %errorlevel% equ 0 (
    docker-compose up -d postgres pgadmin
) else (
    docker compose up -d postgres pgadmin
)

:: 2. Provide connection information
echo.
echo [2/2] PostgreSQL Admin Dashboard is ready!
echo ------------------------------------------
echo URL:            http://localhost:5050
echo Mode:           Desktop (No login required)
echo Pre-configured: Overload DB (Local)
echo   Host:         postgres
echo   Port:         5432
echo   Database:     overloaddb
echo   Username:     postgres
echo   Password:     postgrespassword
echo ------------------------------------------
echo.

:: Open default browser to pgAdmin
echo Opening dashboard in your default browser...
start http://localhost:5050
