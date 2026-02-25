# US-9.1 — Persistent Navigation Bar

**Epic:** Epic 9 — Frontend Shell & UX  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want a persistent top/side navigation bar so I can move between pages without losing context.

---

## Acceptance Criteria

- [ ] A persistent sidebar (or top bar on mobile) is rendered on all authenticated pages
- [ ] Sidebar shows: app logo/name, navigation links, user name, logout button
- [ ] Active route is visually highlighted in the sidebar
- [ ] The sidebar uses MUI `Drawer` component (permanent on desktop, toggleable on mobile)
- [ ] Navigation links for all users: Dashboard, My Profile, Upload Documents
- [ ] Navigation links for admins only: Admin Dashboard, Employees, Taxonomy, Departments (see US-9.2)
- [ ] Logout button calls `POST /api/auth/logout` and clears auth state + refreshes to `/login`
- [ ] App layout component wraps all authenticated routes with the sidebar

---

## Technical Notes

- Create a `<AppLayout>` component that renders `<Sidebar>` + `<Outlet>` (React Router nested route)
- Sidebar navigation uses React Router `<NavLink>` for active state detection

---

## Unit Tests

**Frontend (`Sidebar.test.tsx`):**
- [ ] Renders all common navigation links (Dashboard, Profile, Employees, Taxonomy)
- [ ] Active route link receives the `aria-current="page"` attribute
- [ ] Navigating to a route updates the active link correctly

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-9.2](US-9.2-admin-sidebar.md)
