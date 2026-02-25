# US-7.3 — Edit Employee Details

**Epic:** Epic 7 — Employee Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to edit an employee's details (name, department, role) so records stay current.

---

## Acceptance Criteria

- [ ] `PUT /api/admin/employees/{id}` accepts `{ firstName, lastName, departmentId, role }` and updates the employee
- [ ] Only `Admin` role can call this endpoint
- [ ] Non-existent employee returns `404 Not Found`
- [ ] Admin UI shows an "Edit" button on the employee detail page that opens an edit form (inline or dialog)
- [ ] Form pre-populates with the employee's current values
- [ ] On success, the employee detail page updates without a full page reload
- [ ] Role change follows the same rules as US-2.4 (admin cannot demote themselves)

---

## Technical Notes

- Email is not editable (identity uniqueness constraint; would require re-verification in a real system)
- Department change does not affect existing skill data

---

## Unit Tests

**Backend (`EditEmployeeCommandHandlerTests`):**
- [ ] Successfully updates name and department for an existing employee
- [ ] Returns `NotFound` for a non-existent employee ID
- [ ] Admin cannot demote themselves (returns a validation error)

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-7.2](US-7.2-create-employee.md) | **Next:** [US-7.4](US-7.4-deactivate-employee.md)
