# Skill Extraction Tool — Backlog

> **How to use:** Check the box next to a story when it is complete.  
> Each story title links to its detail file with full acceptance criteria.  
> Status at a glance: count the `[x]` boxes below.

---

## Progress

| Epic | Stories | Done |
|------|---------|------|
| [Epic 1 — Project Setup & Infrastructure](#epic-1--project-setup--infrastructure) | 5 | 0 |
| [Epic 2 — Authentication & Authorization](#epic-2--authentication--authorization) | 6 | 0 |
| [Epic 3 — Document Upload](#epic-3--document-upload) | 5 | 0 |
| [Epic 4 — Skill Extraction Engine](#epic-4--skill-extraction-engine) | 5 | 5 |
| [Epic 5 — Employee Skill Profile](#epic-5--employee-skill-profile) | 6 | 0 |
| [Epic 6 — Admin Dashboard & Analytics](#epic-6--admin-dashboard--analytics) | 6 | 0 |
| [Epic 7 — Employee Management (Admin)](#epic-7--employee-management-admin) | 5 | 0 |
| [Epic 8 — Skill Taxonomy Management (Admin)](#epic-8--skill-taxonomy-management-admin) | 5 | 0 |
| [Epic 9 — Frontend Shell & UX](#epic-9--frontend-shell--ux) | 5 | 0 |
| **Total** | **48** | **5** |

---

## Epic 1 — Project Setup & Infrastructure

> Scaffold the entire project, Docker-compose, CI-ready structure.

- [ ] [US-1.1 — Docker Compose full stack](backlog/epic-1-infrastructure/US-1.1-docker-compose.md)
- [ ] [US-1.2 — .NET 10 solution scaffold](backlog/epic-1-infrastructure/US-1.2-dotnet-scaffold.md)
- [ ] [US-1.3 — React frontend scaffold](backlog/epic-1-infrastructure/US-1.3-frontend-scaffold.md)
- [ ] [US-1.4 — EF Core + PostgreSQL setup](backlog/epic-1-infrastructure/US-1.4-efcore-postgres.md)
- [ ] [US-1.5 — README with setup instructions](backlog/epic-1-infrastructure/US-1.5-readme.md)

---

## Epic 2 — Authentication & Authorization

> JWT-based login, role assignment, protected routes.

- [ ] [US-2.1 — Login with JWT](backlog/epic-2-auth/US-2.1-login-jwt.md)
- [ ] [US-2.2 — Roles: Admin and User](backlog/epic-2-auth/US-2.2-roles.md)
- [ ] [US-2.3 — Seed default admin account](backlog/epic-2-auth/US-2.3-seed-admin.md)
- [ ] [US-2.4 — Admin: change user role](backlog/epic-2-auth/US-2.4-change-role.md)
- [ ] [US-2.5 — Protected frontend routes](backlog/epic-2-auth/US-2.5-protected-routes.md)
- [ ] [US-2.6 — Secure token storage](backlog/epic-2-auth/US-2.6-token-storage.md)

---

## Epic 3 — Document Upload

> Upload CV and IFU PDFs, store them, trigger processing.

- [x] [US-3.1 — Upload CV (PDF)](backlog/epic-3-upload/US-3.1-upload-cv.md)
- [x] [US-3.2 — Upload IFU (PDF)](backlog/epic-3-upload/US-3.2-upload-ifu.md)
- [x] [US-3.3 — Document processing status](backlog/epic-3-upload/US-3.3-processing-status.md)
- [x] [US-3.4 — File validation](backlog/epic-3-upload/US-3.4-file-validation.md)
- [x] [US-3.5 — File storage persistence](backlog/epic-3-upload/US-3.5-file-storage.md)

---

## Epic 4 — Skill Extraction Engine

> PDF → text → keyword matching → taxonomy mapping → skill records.

- [x] [US-4.1 — PDF text extraction (PdfPig)](backlog/epic-4-extraction/US-4.1-pdf-text-extraction.md)
- [x] [US-4.2 — ML.NET tokenization + taxonomy matching](backlog/epic-4-extraction/US-4.2-mlnet-matching.md)
- [x] [US-4.3 — Proficiency level inference](backlog/epic-4-extraction/US-4.3-proficiency-inference.md)
- [x] [US-4.4 — ProcessDocumentCommand via MediatR](backlog/epic-4-extraction/US-4.4-process-document-command.md)
- [x] [US-4.5 — Persist extracted skills](backlog/epic-4-extraction/US-4.5-persist-skills.md)

---

## Epic 5 — Employee Skill Profile

> View, edit, and manage extracted skill profiles.

- [ ] [US-5.1 — View own skill profile](backlog/epic-5-profile/US-5.1-view-own-profile.md)
- [ ] [US-5.2 — Proficiency indicators on skills](backlog/epic-5-profile/US-5.2-proficiency-indicators.md)
- [ ] [US-5.3 — Admin: view any employee profile](backlog/epic-5-profile/US-5.3-admin-view-profile.md)
- [ ] [US-5.4 — Admin: manually add skill](backlog/epic-5-profile/US-5.4-admin-add-skill.md)
- [ ] [US-5.5 — Admin: remove skill](backlog/epic-5-profile/US-5.5-admin-remove-skill.md)
- [ ] [US-5.6 — Admin: change proficiency level](backlog/epic-5-profile/US-5.6-admin-change-proficiency.md)

---

## Epic 6 — Admin Dashboard & Analytics

> Charts and graphs for organizational skill visibility.

- [ ] [US-6.1 — Top 10 skills bar chart](backlog/epic-6-dashboard/US-6.1-top-skills-chart.md)
- [ ] [US-6.2 — Skills per department chart](backlog/epic-6-dashboard/US-6.2-skills-per-department.md)
- [ ] [US-6.3 — Skill gap analysis view](backlog/epic-6-dashboard/US-6.3-skill-gap-analysis.md)
- [ ] [US-6.4 — Upload activity over time chart](backlog/epic-6-dashboard/US-6.4-upload-activity-chart.md)
- [ ] [US-6.5 — Proficiency distribution chart](backlog/epic-6-dashboard/US-6.5-proficiency-distribution.md)
- [ ] [US-6.6 — Dashboard data via TanStack Query](backlog/epic-6-dashboard/US-6.6-dashboard-tanstack-query.md)

---

## Epic 7 — Employee Management (Admin)

> CRUD employees, manage departments, assign roles.

- [ ] [US-7.1 — Paginated employee list with search](backlog/epic-7-employee-mgmt/US-7.1-employee-list.md)
- [ ] [US-7.2 — Create new employee account](backlog/epic-7-employee-mgmt/US-7.2-create-employee.md)
- [ ] [US-7.3 — Edit employee details](backlog/epic-7-employee-mgmt/US-7.3-edit-employee.md)
- [ ] [US-7.4 — Deactivate employee account](backlog/epic-7-employee-mgmt/US-7.4-deactivate-employee.md)
- [ ] [US-7.5 — Manage departments](backlog/epic-7-employee-mgmt/US-7.5-manage-departments.md)

---

## Epic 8 — Skill Taxonomy Management (Admin)

> Manage the predefined skills list that drives extraction.

- [ ] [US-8.1 — View skill taxonomy by category](backlog/epic-8-taxonomy/US-8.1-view-taxonomy.md)
- [ ] [US-8.2 — Add skill to taxonomy](backlog/epic-8-taxonomy/US-8.2-add-skill.md)
- [ ] [US-8.3 — Edit skill in taxonomy](backlog/epic-8-taxonomy/US-8.3-edit-skill.md)
- [ ] [US-8.4 — Deactivate skill in taxonomy](backlog/epic-8-taxonomy/US-8.4-deactivate-skill.md)
- [ ] [US-8.5 — Define required skills per department](backlog/epic-8-taxonomy/US-8.5-required-skills-per-dept.md)

---

## Epic 9 — Frontend Shell & UX

> App shell, navigation, routing, layout, protected routes.

- [ ] [US-9.1 — Persistent navigation bar](backlog/epic-9-frontend-shell/US-9.1-navigation-bar.md)
- [ ] [US-9.2 — Admin-only sidebar section](backlog/epic-9-frontend-shell/US-9.2-admin-sidebar.md)
- [ ] [US-9.3 — Route-level code splitting](backlog/epic-9-frontend-shell/US-9.3-code-splitting.md)
- [ ] [US-9.4 — Loading skeletons](backlog/epic-9-frontend-shell/US-9.4-loading-skeletons.md)
- [ ] [US-9.5 — Toast/snackbar notifications](backlog/epic-9-frontend-shell/US-9.5-notifications.md)

---

## Key Technical Decisions

| Decision | Choice | Reason |
|----------|--------|--------|
| PDF extraction | PdfPig | Open-source, no AGPL concerns, .NET 10 compatible |
| Skill extraction | ML.NET keyword matching | No external API keys needed |
| Auth | ASP.NET Core Identity + JWT | Self-contained, no external IdP |
| Token storage | In-memory + HTTP-only refresh cookie | Avoid localStorage XSS risk |
| React Router | v6 (not v7) | v7 has Remix-style breaking changes |
| TanStack Query | v5 — single object signature everywhere | Per v5 API |
| Proficiency levels | Inferred from context, admin-overridable | Flexible but auditable |
