# US-2.2 — Roles: Admin and User

**Epic:** Epic 2 — Authentication & Authorization  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a developer, I want ASP.NET Core Identity configured with two roles (`Admin`, `User`) so role-based access control is enforced across all endpoints.

---

## Acceptance Criteria

- [ ] Two roles seeded in the database on startup: `Admin` and `User`
- [ ] All API endpoints are decorated with `[Authorize]` by default (global policy)
- [ ] Admin-only endpoints use `[Authorize(Roles = "Admin")]`
- [ ] Attempting to access an admin endpoint as a `User` returns `403 Forbidden`
- [ ] Attempting to access a protected endpoint without a token returns `401 Unauthorized`
- [ ] Role is included in the JWT claims and used for frontend route guarding
- [ ] A new user's default role is `User` unless explicitly set to `Admin`

---

## Technical Notes

- Register a global `[Authorize]` filter in `Program.cs` so endpoints are secured by default
- Use `[AllowAnonymous]` on `AuthController` endpoints only
- Role claims in JWT: `role` claim with value `"Admin"` or `"User"`
- Frontend route guard checks role from decoded JWT (or auth context)

---

## Unit Tests

**Backend (authorization policy tests):**
- [ ] A request without a JWT to a protected endpoint returns `401`
- [ ] A `User`-role JWT to an `[Authorize(Roles = "Admin")]` endpoint returns `403`
- [ ] An `Admin`-role JWT to an admin endpoint returns `200`
- [ ] A `User`-role JWT to a non-admin protected endpoint returns `200`

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-2.1](US-2.1-login-jwt.md) | **Next:** [US-2.3](US-2.3-seed-admin.md)
