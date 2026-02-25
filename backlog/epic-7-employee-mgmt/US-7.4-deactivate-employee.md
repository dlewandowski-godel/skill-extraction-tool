# US-7.4 — Deactivate Employee Account

**Epic:** Epic 7 — Employee Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to deactivate an employee account so former employees can no longer log in without losing their skill data.

---

## Acceptance Criteria

- [ ] `PUT /api/admin/employees/{id}/deactivate` sets the user's `IsActive = false` (soft delete)
- [ ] Deactivated users cannot log in; login attempt returns `401` with message "Account is deactivated"
- [ ] Skill profile data is retained for reporting purposes
- [ ] Admin can re-activate an account via `PUT /api/admin/employees/{id}/activate`
- [ ] Deactivated employees are shown with a visual badge ("Inactive") in the employee list
- [ ] Admin UI shows "Deactivate" button with a confirmation dialog before proceeding
- [ ] An admin cannot deactivate their own account
- [ ] Deactivating an employee revokes all their active refresh tokens

---

## Technical Notes

- Use ASP.NET Core Identity `LockoutEnabled` + `LockoutEndDateUtc = DateTimeOffset.MaxValue` for deactivation, or a custom `IsActive` column on `ApplicationUser`
- Check `IsActive` in the login handler before issuing a token

---

## Unit Tests

**Backend (`DeactivateEmployeeCommandHandlerTests`):**
- [ ] Sets `IsActive = false` and revokes all refresh tokens for the target user
- [ ] Returns `NotFound` for a non-existent employee
- [ ] Returns a validation error when an admin attempts to deactivate their own account

**Backend (`LoginCommandHandlerTests` — regression):**
- [ ] Deactivated user login attempt returns `401` with "Account is deactivated"

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-7.3](US-7.3-edit-employee.md) | **Next:** [US-7.5](US-7.5-manage-departments.md)
