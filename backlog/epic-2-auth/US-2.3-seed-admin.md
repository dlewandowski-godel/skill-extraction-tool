# US-2.3 — Seed Default Admin Account

**Epic:** Epic 2 — Authentication & Authorization  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a default admin account to exist after first startup so I can log in immediately after running `docker-compose up` without manual database setup.

---

## Acceptance Criteria

- [ ] On API startup, a database seeder runs and creates an admin user if none exists
- [ ] Default admin credentials are configurable via environment variables (not hardcoded in source)
- [ ] Default values in `.env.example`: `SEED_ADMIN_EMAIL=admin@skillextractor.local`, `SEED_ADMIN_PASSWORD=Admin@123!`
- [ ] The seeded user has the `Admin` role assigned
- [ ] Seeder is idempotent — running it multiple times does not create duplicate accounts
- [ ] The seeder logs a message when a new admin is created (`[Seeder] Admin account created`)
- [ ] The seeder logs a message when the admin already exists (`[Seeder] Admin account already exists, skipping`)
- [ ] Seeded credentials are documented in `README.md`

---

## Technical Notes

- Seeder is a scoped service called from `Program.cs` inside `await using var scope = app.Services.CreateAsyncScope()`
- Seeder should also create the `Admin` and `User` roles if they don't exist (combines with US-2.2)
- Password must satisfy ASP.NET Core Identity default password policy

---

## Unit Tests

**Backend (`DatabaseSeederTests`):**
- [ ] When no admin exists, seeder creates a user with `Admin` role
- [ ] When the admin already exists, seeder does not create a duplicate (idempotent)
- [ ] When neither `Admin` nor `User` role exists, both are created
- [ ] Seeder logs `[Seeder] Admin account created` when creating, and `[Seeder] Admin account already exists, skipping` when not

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-2.2](US-2.2-roles.md) | **Next:** [US-2.4](US-2.4-change-role.md)
