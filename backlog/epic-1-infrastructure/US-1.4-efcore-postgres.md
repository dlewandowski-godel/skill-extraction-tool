# US-1.4 — EF Core + PostgreSQL Setup

**Epic:** Epic 1 — Project Setup & Infrastructure  
**Status:** `[x] Done`

---

## User Story

> As a developer, I want EF Core + Npgsql configured with an initial migration and a working connection to the Dockerized PostgreSQL so the database is ready for feature work.

---

## Acceptance Criteria

- [ ] `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, and `Microsoft.EntityFrameworkCore.Design` installed in `Infrastructure`
- [ ] `AppDbContext` created in `Infrastructure` with `DbSet<>` properties for initial entities
- [ ] Connection string is read from environment variable / `appsettings.json` (no hardcoded credentials)
- [ ] An initial EF Core migration exists (`dotnet ef migrations add InitialCreate`)
- [ ] `Database.MigrateAsync()` is called on startup so migrations run automatically in Docker
- [ ] Running `docker-compose up` results in a healthy PostgreSQL instance with the schema applied
- [ ] pgAdmin can connect to the database and show the created tables

---

## Technical Notes

- Use `snake_case` naming convention for PostgreSQL column names via `UseSnakeCaseNamingConvention()`
- Configure EF from `Infrastructure` project using `IEntityTypeConfiguration<T>` classes (one per entity)
- Store migration files in `Infrastructure/Persistence/Migrations/`
- Add `UseNpgsqlDataSource` for connection pooling support

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-1.3](US-1.3-frontend-scaffold.md) | **Next:** [US-1.5](US-1.5-readme.md)
