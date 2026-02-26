# Skill Extraction Tool

A full-stack web application that automatically extracts skills from CVs and IFUs (Instructions for Use) using ML.NET matching, then lets HR admins and managers view employee skill profiles, manage a skill taxonomy, and analyse skill gaps across departments.

> **Project status:** See [BACKLOG.md](BACKLOG.md) for the full story backlog and progress.

---

## Prerequisites

| Tool | Minimum version |
|------|----------------|
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | 24+ |
| [Docker Compose](https://docs.docker.com/compose/install/) | v2.20+ |
| [Node.js](https://nodejs.org/) | 20 LTS |
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0 |

---

## Quick Start (Docker)

```bash
# 1. Clone the repository
git clone https://github.com/dlewandowski-godel/skill-extraction-tool.git
cd skill-extraction-tool

# 2. Copy and review environment variables
cp .env.example .env
# Edit .env and set secure values for passwords and JWT_SECRET

# 3. Start the full stack
docker compose up --build

# 4. Open the app
#   Frontend  → http://localhost:3000
#   API       → http://localhost:5000
#   pgAdmin   → http://localhost:5050
#   OpenAPI   → http://localhost:5000/openapi
```

The API runs EF Core migrations automatically on startup. The first boot takes ~30 s while Docker pulls images.

---

## Default credentials

| Service | Email / User | Password |
|---------|--------------|----------|
| Admin account (seeded) | `admin@example.com` | `Admin1234!` *(set in seed script)* |
| pgAdmin | value of `PGADMIN_EMAIL` in `.env` | value of `PGADMIN_PASSWORD` in `.env` |
| PostgreSQL | value of `POSTGRES_USER` in `.env` | value of `POSTGRES_PASSWORD` in `.env` |

> ⚠️ Change all default credentials before deploying to any non-local environment.

---

## Environment variables

Copy `.env.example` to `.env` and fill in the values below.

| Variable | Description | Example |
|----------|-------------|---------|
| `POSTGRES_DB` | PostgreSQL database name | `skillextractor` |
| `POSTGRES_USER` | PostgreSQL super-user | `skilluser` |
| `POSTGRES_PASSWORD` | PostgreSQL password | *(strong password)* |
| `PGADMIN_EMAIL` | pgAdmin login e-mail | `admin@example.com` |
| `PGADMIN_PASSWORD` | pgAdmin login password | *(strong password)* |
| `JWT_SECRET` | HS256 signing secret (min 32 chars) | *(random 64-char string)* |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` |
| `VITE_API_URL` | API base URL used by the frontend | `http://localhost:5000` |

---

## Running without Docker (local development)

### Backend

```bash
cd backend

# Start a local Postgres (or point to the Docker one on port 5433)
# then set connection string
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5433;Database=skillextractor;Username=skilluser;Password=changeme"
export Jwt__Secret="your-super-secret-jwt-key-min-32-chars"

dotnet run --project src/SkillExtractor.API
```

### Frontend

```bash
cd frontend
cp .env.example .env.local   # set VITE_API_URL=http://localhost:5000
npm install
npm run dev   # → http://localhost:3000
```

---

## Architecture

### Backend layers (Clean Architecture)

```
SkillExtractor.API            — ASP.NET Core controllers, middleware, DI wiring
    ↓ depends on
SkillExtractor.Application    — MediatR commands/queries, DTOs, FluentValidation
    ↓ depends on
SkillExtractor.Domain         — Entities, value objects, IRepository<T> interfaces
                                (no external dependencies)

SkillExtractor.Infrastructure — EF Core, Npgsql, ML.NET, file storage
    ↓ implements contracts from Application + Domain
```

Test project: `SkillExtractor.Tests` (xUnit + NSubstitute + FluentAssertions)

### Frontend

```
frontend/
  src/
    lib/api-client.ts   — Axios instance with JWT interceptor
    theme.ts            — Global MUI v6 theme
    App.tsx             — Root component with React Router
    main.tsx            — Entry point: QueryClientProvider + BrowserRouter + ThemeProvider
```

Stack: **Vite + React 19 + TypeScript**, MUI v6, TanStack Query v5, React Router v6, Vitest

---

## Useful commands

```bash
# Run backend tests
cd backend && dotnet test

# Run frontend tests
cd frontend && npm test

# Add a new EF Core migration
cd backend
dotnet ef migrations add MigrationName \
  --project src/SkillExtractor.Infrastructure \
  --startup-project src/SkillExtractor.API \
  --output-dir Persistence/Migrations

# Remove last migration (if not yet applied)
dotnet ef migrations remove \
  --project src/SkillExtractor.Infrastructure \
  --startup-project src/SkillExtractor.API
```

---

## Troubleshooting

| Symptom | Fix |
|---------|-----|
| `docker compose up` fails with "port already in use" | Stop conflicting services: `sudo lsof -i :5432` / `:5000` / `:3000` and kill the process |
| API exits immediately with "Connection string not configured" | Ensure `.env` exists and `docker compose` is picking it up (`docker compose config`) |
| EF migration error on startup | Check Postgres health: `docker compose ps postgres`. Rebuild: `docker compose down -v && docker compose up --build` |
| pgAdmin shows no servers | Add a server manually: host `postgres`, port `5432`, credentials from `.env` |
| Frontend shows blank page | Check browser console; ensure `VITE_API_URL` is set and the API is running |
