# US-2.1 — Login with JWT

**Epic:** Epic 2 — Authentication & Authorization  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want to log in with my email and password and receive a JWT token so I can access protected features.

---

## Acceptance Criteria

- [ ] `POST /api/auth/login` endpoint accepts `{ email, password }` and returns `{ accessToken, expiresIn, role }`
- [ ] Incorrect credentials return `401 Unauthorized` with a generic error message (no user enumeration)
- [ ] The access token is a signed JWT containing `sub` (user ID), `email`, `role`, and `exp` claims
- [ ] The JWT is signed with a secret from environment config (not hardcoded)
- [ ] Access token lifetime is configurable (default: 15 minutes)
- [ ] A `POST /api/auth/refresh` endpoint issues a new access token using an HTTP-only refresh cookie
- [ ] A `POST /api/auth/logout` endpoint clears the refresh cookie
- [ ] Frontend login form validates email format and non-empty password before submitting
- [ ] Frontend stores the access token in memory (React context), not localStorage
- [ ] On successful login, the user is redirected to `/dashboard`
- [ ] On failed login, a clear error message is shown in the form

---

## Technical Notes

- Use `Microsoft.AspNetCore.Authentication.JwtBearer`
- Use `Microsoft.AspNetCore.Identity.EntityFrameworkCore` for the user store
- Refresh token stored in the database, linked to the user, with expiry and revocation flag
- Refresh cookie should be `HttpOnly`, `Secure`, `SameSite=Strict`

---

## Unit Tests

**Backend (`LoginCommandHandlerTests`):**
- [ ] Valid credentials return an access token and set a refresh cookie
- [ ] Invalid password returns `401` result (no exception thrown)
- [ ] Non-existent email returns `401` result (no user enumeration)
- [ ] Deactivated account returns `401` with correct message

**Frontend (`LoginForm.test.tsx`):**
- [ ] Form renders email and password fields and a submit button
- [ ] Submitting with an empty email shows a validation error
- [ ] Submitting with an empty password shows a validation error
- [ ] Successful login mutation result redirects to `/dashboard`
- [ ] Failed login mutation shows the error message in the form

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-2.2](US-2.2-roles.md)
