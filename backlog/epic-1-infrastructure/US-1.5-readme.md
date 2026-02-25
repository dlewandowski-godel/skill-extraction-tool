# US-1.5 — README with Setup Instructions

**Epic:** Epic 1 — Project Setup & Infrastructure  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a developer, I want a `README.md` with setup instructions so any new developer can run the project in under 5 minutes.

---

## Acceptance Criteria

- [ ] `README.md` at the repo root explains what the project does in 2–3 sentences
- [ ] Prerequisites section lists required tools (Docker, Docker Compose, Node 20+, .NET SDK 10)
- [ ] Step-by-step "Quick Start" section shows how to run with `docker-compose up`
- [ ] Instructions for running without Docker (manual local setup) are included
- [ ] Default credentials for the seeded admin account are documented
- [ ] Environment variables table documents every variable in `.env.example`
- [ ] Architecture overview (brief) describes the four backend layers and frontend structure
- [ ] Links to the [BACKLOG.md](../../BACKLOG.md) for project status

---

## Technical Notes

- Keep the README scannable — use headers, bullet points, and code blocks
- Include a "Troubleshooting" section for the most common issues (port conflicts, migration errors)

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-1.4](US-1.4-efcore-postgres.md)
