# US-7.2 — Create New Employee Account

**Epic:** Epic 7 — Employee Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to create a new employee account (name, email, role, department) so new hires can log in.

---

## Acceptance Criteria

- [ ] `POST /api/admin/employees` accepts `{ firstName, lastName, email, role, departmentId }` and creates the user
- [ ] Email must be unique; duplicate email returns `400 Bad Request` with message "Email already in use"
- [ ] A temporary password is auto-generated and returned in the response (admin shares it with the employee)
- [ ] New user is assigned the specified role (`Admin` or `User`)
- [ ] Frontend shows a "New Employee" button on the employee list page that opens a dialog with the creation form
- [ ] Form validates: required fields, valid email format, role selected
- [ ] On success, the employee list is invalidated and the new employee appears at the top
- [ ] A success snackbar shows: "Employee account created. Temporary password: {password}"

---

## Technical Notes

- Use `UserManager.CreateAsync(user, password)` from ASP.NET Core Identity
- Temporary password generation: `Guid.NewGuid().ToString("N").Substring(0, 12) + "A1!"` to satisfy Identity password policy
- `departmentId` is optional — employees can be created without a department

---

## Unit Tests

**Backend (`CreateEmployeeCommandHandlerTests`):**
- [ ] Successfully creates a user with `User` role when all required fields are valid
- [ ] Returns a conflict error when the email is already in use
- [ ] Created user has the specified role (`Admin` or `User`)
- [ ] Temporary password satisfies ASP.NET Identity complexity requirements

**Frontend (`CreateEmployeeDialog.test.tsx`):**
- [ ] Form renders all required fields (first name, last name, email, role, department)
- [ ] Submit with missing required field shows a validation error
- [ ] Submit with an invalid email format shows a validation error

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-7.1](US-7.1-employee-list.md) | **Next:** [US-7.3](US-7.3-edit-employee.md)
