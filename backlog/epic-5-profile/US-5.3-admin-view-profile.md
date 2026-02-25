# US-5.3 — Admin: View Any Employee Profile

**Epic:** Epic 5 — Employee Skill Profile  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to view any employee's full skill profile so I can assess their capabilities.

---

## Acceptance Criteria

- [ ] `GET /api/admin/employees/{id}/profile` returns the specified employee's skill profile
- [ ] Endpoint is restricted to `Admin` role; others receive `403 Forbidden`
- [ ] Non-existent employee ID returns `404 Not Found`
- [ ] The admin can navigate to an employee's profile from the employee list page (`/admin/employees/:id`)
- [ ] The employee profile page (`/admin/employees/:id`) shows the same grouped skill layout as the user's own profile (US-5.1), plus admin action buttons (add / edit / remove skill)
- [ ] Page title shows the employee's name and department
- [ ] A breadcrumb navigation: `Admin > Employees > {Employee Name} > Profile`

---

## Technical Notes

- Reuse the skill profile layout component — data shape is identical between own-profile and admin-view
- MediatR query: `GetEmployeeProfileByIdQuery(EmployeeId)`

---

## Unit Tests

**Backend (`GetEmployeeProfileByIdQueryHandlerTests`):**
- [ ] Returns the correct employee's skill profile by ID
- [ ] Returns a `NotFound` result for a non-existent employee ID
- [ ] Non-admin callers receive an authorization failure (enforced at the controller level)

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-5.2](US-5.2-proficiency-indicators.md) | **Next:** [US-5.4](US-5.4-admin-add-skill.md)
