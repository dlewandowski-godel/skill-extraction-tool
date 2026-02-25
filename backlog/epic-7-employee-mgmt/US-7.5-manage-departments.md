# US-7.5 — Manage Departments

**Epic:** Epic 7 — Employee Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to create and manage departments so employees can be organized into teams.

---

## Acceptance Criteria

- [ ] `POST /api/admin/departments` creates a new department with `{ name }`
- [ ] `GET /api/admin/departments` returns all departments with employee count
- [ ] `PUT /api/admin/departments/{id}` renames a department
- [ ] `DELETE /api/admin/departments/{id}` deletes a department only if no employees are assigned to it; otherwise returns `400`
- [ ] Department management UI is a page at `/admin/departments` or a section within settings
- [ ] Department list shows: name, number of employees, required skills count (from US-8.5)
- [ ] Department name must be unique; duplicate names return `400 Bad Request`

---

## Technical Notes

- `Department` entity: `Id (Guid)`, `Name (string)`, `CreatedAt`
- Employees have a nullable `DepartmentId` foreign key

---

## Unit Tests

**Backend (Department command/query handler tests):**
- [ ] `CreateDepartmentCommandHandler` creates a department with a unique name
- [ ] `CreateDepartmentCommandHandler` returns a conflict error for a duplicate name
- [ ] `DeleteDepartmentCommandHandler` deletes a department with no employees assigned
- [ ] `DeleteDepartmentCommandHandler` returns a validation error when employees are assigned to the department
- [ ] `GetDepartmentsQueryHandler` returns all departments with correct employee counts

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-7.4](US-7.4-deactivate-employee.md)
