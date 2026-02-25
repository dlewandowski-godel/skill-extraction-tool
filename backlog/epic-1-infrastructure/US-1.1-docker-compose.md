# US-1.1 — Docker Compose Full Stack

**Epic:** Epic 1 — Project Setup & Infrastructure  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a developer, I want a `docker-compose.yml` that starts PostgreSQL, pgAdmin, the .NET API, and the React frontend so I can run the whole stack with `docker-compose up`.

---

## Acceptance Criteria

- [ ] Running `docker-compose up` from the project root starts all four services without manual steps
- [ ] Services included: `postgres`, `pgadmin`, `api` (.NET 10), `frontend` (React/Vite)
- [ ] PostgreSQL is accessible on port `5432` (internal) and `5433` (host)
- [ ] pgAdmin is accessible at `http://localhost:5050`
- [ ] The .NET API is accessible at `http://localhost:5000`
- [ ] The React frontend is accessible at `http://localhost:3000`
- [ ] All services use named volumes so data persists across restarts
- [ ] Environment variables (DB credentials, JWT secret, etc.) are injected via a `.env` file
- [ ] A `.env.example` file is committed with placeholder values
- [ ] The API service depends on and waits for the `postgres` service to be healthy before starting
- [ ] A `docker-compose.override.yml` is provided for local development (hot reload, volume mounts)

---

## Technical Notes

- Use Docker health checks on the `postgres` service so `depends_on: condition: service_healthy` works
- Frontend container uses `node:20-alpine` for the Vite dev server
- API container uses `mcr.microsoft.com/dotnet/sdk:10.0` for development, `aspnet:10.0` for production target
- EF Core migrations should run automatically on API startup via `Database.MigrateAsync()`

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next story:** [US-1.2 — .NET 10 solution scaffold](US-1.2-dotnet-scaffold.md)
