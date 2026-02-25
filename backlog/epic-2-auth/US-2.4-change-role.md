# US-2.4 — Admin: Change User Role

**Epic:** Epic 2 — Authentication & Authorization  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to assign or change a user's role so I can grant or revoke admin access.

---

## Acceptance Criteria

- [ ] `PUT /api/admin/users/{id}/role` endpoint accepts `{ role: "Admin" | "User" }` and updates the user's role
- [ ] Only `Admin` users can call this endpoint; others receive `403 Forbidden`
- [ ] An admin cannot demote their own account (returns `400 Bad Request` with a clear message)
- [ ] The role change takes effect on the user's next login (existing tokens are not invalidated mid-session by default)
- [ ] A success response returns the updated user summary
- [ ] Admin UI shows a role selector on the employee detail page
- [ ] Role change triggers a confirmation dialog before submitting

---

## Technical Notes

- Use `UserManager<ApplicationUser>.RemoveFromRolesAsync` + `AddToRoleAsync` for the change
- A future enhancement could invalidate the user's refresh tokens on role change for immediate effect

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-2.3](US-2.3-seed-admin.md) | **Next:** [US-2.5](US-2.5-protected-routes.md)
