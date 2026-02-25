# US-9.2 — Admin-Only Sidebar Section

**Epic:** Epic 9 — Frontend Shell & UX  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want admin-only navigation items hidden from my sidebar so the UI isn't cluttered with features I can't access.

---

## Acceptance Criteria

- [ ] Admin navigation section (Admin Dashboard, Employees, Taxonomy, Departments) is only rendered when `role === 'Admin'`
- [ ] Non-admin users see no visual trace of admin routes in the sidebar
- [ ] The admin section in the sidebar has a visual separator and label ("Administration")
- [ ] If a non-admin manually navigates to `/admin/*`, they are redirected to `/dashboard` with an "Access denied" snackbar
- [ ] Admin users see both regular and admin sections in the sidebar

---

## Technical Notes

- Role check comes from `useAuth().role` from auth context
- Conditionally render the admin `<List>` section based on role

---

## Unit Tests

**Frontend (`AdminSidebar.test.tsx`):**
- [ ] Admin navigation section is visible when `role === 'Admin'`
- [ ] Admin navigation section is not rendered when `role === 'User'`
- [ ] Non-admin users cannot navigate to `/admin/*` routes directly (protected route redirects to `/`)

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-9.1](US-9.1-navigation-bar.md) | **Next:** [US-9.3](US-9.3-code-splitting.md)
