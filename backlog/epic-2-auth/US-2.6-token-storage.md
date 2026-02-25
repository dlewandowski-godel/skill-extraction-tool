# US-2.6 — Secure Token Storage

**Epic:** Epic 2 — Authentication & Authorization  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a developer, I want the JWT token stored in memory (not localStorage) and refreshed via a silent refresh mechanism so the app is reasonably secure against XSS token theft.

---

## Acceptance Criteria

- [ ] The access token is stored only in React state / context (in-memory), never written to `localStorage` or `sessionStorage`
- [ ] The refresh token is stored exclusively in an HTTP-only cookie set by the server
- [ ] On app load, the frontend calls `POST /api/auth/refresh` silently to restore the session if a valid refresh cookie exists
- [ ] If the refresh call fails (cookie expired or revoked), the user is redirected to `/login`
- [ ] A background timer triggers a silent refresh ~1 minute before the access token expires
- [ ] If the browser tab is closed and reopened, the session is restored from the refresh cookie (persistent session)
- [ ] The Axios instance automatically retries a failed `401` response once after a silent refresh before redirecting to login

---

## Technical Notes

- `AuthContext` exposes: `{ user, accessToken, login, logout, isAuthenticated }`
- Silent refresh calls must not be intercepted by the JWT Axios interceptor (avoid infinite loop — use a separate Axios instance for auth calls)
- Refresh token rotation: each refresh issues a new refresh token and invalidates the old one

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-2.5](US-2.5-protected-routes.md)
