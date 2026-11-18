#!/usr/bin/env bash

# Exit immediately if a command exits with a non-zero status
set -e

echo "=========================================="
echo " Starting PostgreSQL Admin Dashboard"
echo "=========================================="

cd "$(dirname "$0")"

# 1. Start Docker Containers (PostgreSQL & pgAdmin)
echo "[1/2] Ensuring PostgreSQL and pgAdmin containers are running..."
if command -v docker-compose &> /dev/null; then
    docker-compose up -d postgres pgadmin
else
    docker compose up -d postgres pgadmin
fi

# 2. Provide connection info
echo ""
echo "[2/2] PostgreSQL Admin Dashboard is ready!"
echo "------------------------------------------"
echo "URL:            http://localhost:5050"
echo "Mode:           Desktop (No login required)"
echo "Pre-configured: Overload DB (Local)"
echo "  Host:         postgres"
echo "  Port:         5432"
echo "  Database:     overloaddb"
echo "  Username:     postgres"
echo "  Password:     postgrespassword"
echo "------------------------------------------"
echo ""

# Open in browser if possible
if command -v xdg-open &> /dev/null; then
    xdg-open "http://localhost:5050"
elif command -v open &> /dev/null; then
    open "http://localhost:5050"
fi
