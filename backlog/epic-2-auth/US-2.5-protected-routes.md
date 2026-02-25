# US-2.5 — Protected Frontend Routes

**Epic:** Epic 2 — Authentication & Authorization  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want protected frontend routes that redirect to `/login` when I'm not authenticated so I can't access pages without logging in.

---

## Acceptance Criteria

- [ ] A `<ProtectedRoute>` component wraps all authenticated routes
- [ ] Unauthenticated users accessing any protected route are redirected to `/login`
- [ ] After login, the user is redirected back to the originally requested URL
- [ ] A `<AdminRoute>` component wraps admin-only routes
- [ ] Non-admin users accessing `/admin/*` routes are redirected to `/dashboard` with a "Not authorized" message
- [ ] The `/login` route is not accessible to already-authenticated users (redirects to `/dashboard`)
- [ ] Auth state is persisted across page refreshes via the silent refresh mechanism (see US-2.6)

---

## Technical Notes

- Use React Router v6 `<Outlet>` pattern for layout routes
- `<ProtectedRoute>` reads auth state from a `useAuth()` hook (custom hook over auth context)
- Role check: `useAuth().role === 'Admin'` for admin gate

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-2.4](US-2.4-change-role.md) | **Next:** [US-2.6](US-2.6-token-storage.md)
