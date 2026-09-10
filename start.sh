#!/usr/bin/env bash

# Exit immediately if a command exits with a non-zero status
set -e

echo "=========================================="
echo " Starting Full-Stack Application"
echo "=========================================="

# Cleanup function to kill background processes when Ctrl+C or script exits
cleanup() {
    echo ""
    echo "Shutting down application services..."
    kill 0 2>/dev/null || true
}
trap cleanup EXIT INT TERM

# 1. Start Docker Container (PostgreSQL)
echo "[1/3] Starting containerized PostgreSQL DB..."
if command -v docker-compose &> /dev/null; then
    docker-compose up -d
else
    docker compose up -d
fi

echo "Waiting for PostgreSQL database to be ready..."
sleep 3

# 2. Start .NET Core Backend
echo "[2/3] Starting .NET Core Backend API (http://localhost:5245)..."
(cd backend && dotnet run) &
BE_PID=$!
echo "Backend running with PID: $BE_PID"

# Give backend a moment to bind to http://localhost:5245
sleep 4

# 3. Start Vuetify Frontend
echo "[3/3] Starting Vuetify + Vite Frontend..."
echo "Frontend running at http://localhost:5173"
(cd frontend && npm run dev)
