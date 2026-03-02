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

## Prompts
1.I want to create new project.
Skill extraction tool. An app that reads a new employee's CV and IFU, extracts skills and saves them in a structured format.
For frontend use React, typescript, mui and tanstack query. Try to use composition and tanstack query for state management.
For backend use .net 10, MediatR,ef core, clean architecture with Commands and use DDD. Ask clarifying questions. Create docker-compose that allows to run everything with a single command docker-compose up. For db use postgres. I want unit tests for both frontend and backend. I want an admin panel. I want simple login and 2 roles: Admin and User. Add some pretty graphs for admin view.
I want to have at least several pages, routing. I want to first create a backlog of epics and user stories. Then I will work on them 1 by 1 with you.

2.I want you generate this backlog as folders and files in this project. I also want a way of ticking them off so I could see what is done

3. start working on epic-1. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 1.
4. start working on epic-1. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 1.
5. we need a register functoinality for frontend and backend as well for normal users. Also there is a lot of errors. fix them.
6. How to manually test this epic?
7. start working on epic-4. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 4. after that make sure you've implemented everything.
8. start working on epic-5. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 5. after that make sure you've implemented everything.
9.how do I test manually everything up till epic 5? I will docker compose up and start clicking around the frontend app
10.start working on epic-6. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 6. after that make sure you've implemented everything.
11.start working on epic-7. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 7. after that make sure you've implemented everything.
12. start working on epic-8. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 8. after that make sure you've implemented everything.
13. start working on epic-8. After you're done with a single user story, build and run the tests. If that fails, fix them. if everything works, move on to the next user story. finish this way entire epic 8. after that make sure you've implemented everything.
14. create new folder with example cvs and  ifu files. clear database. create seed data that runs automatically on start like with admin account.

## Tools
Claude Sonnet 4.6 using copilot and vscode.
I started from Plan to create a backlog, then agent all the way through.
In normal project I always start a user story with a Plan mode to review it thoroughly. Then move on to agent mode to make smaller changes / ask questions. Ask mode feels unnecesary. I didn't use any MCP server, because for the scope of this project it seemed unnecesary.