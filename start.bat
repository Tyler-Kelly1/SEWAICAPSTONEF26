@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo  Starting Full-Stack Application
echo ==========================================

:: Navigate to script directory
cd /d "%~dp0"

:: 1. Start Docker Container (PostgreSQL)
echo [1/3] Starting containerized PostgreSQL DB...
where docker-compose >nul 2>&1
if %errorlevel% equ 0 (
    docker-compose up -d
) else (
    docker compose up -d
)

echo Waiting for PostgreSQL database to be ready...
timeout /t 3 /nobreak >nul

:: 2. Start .NET Core Backend API in a separate window
echo [2/3] Starting .NET Core Backend API (http://localhost:5245)...
start "Overload Backend (.NET)" /d "%~dp0backend" cmd /k "dotnet run"
echo Backend launched in a separate window.

:: Give backend time to bind to port
timeout /t 4 /nobreak >nul

:: 3. Start Vuetify Frontend in main window
echo [3/3] Starting Vuetify + Vite Frontend (http://localhost:5173)...
cd /d "%~dp0frontend"
npm run dev
