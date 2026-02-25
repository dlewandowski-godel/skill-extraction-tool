# US-7.1 — Paginated Employee List with Search

**Epic:** Epic 7 — Employee Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a paginated, searchable list of all employees so I can find and manage any user.

---

## Acceptance Criteria

- [ ] `GET /api/admin/employees?page=1&pageSize=20&search=john&department=engineering` returns a paginated list of employees
- [ ] Response includes: `{ items: Employee[], totalCount, page, pageSize }`
- [ ] Each employee in the list shows: name, email, department, role, account status (active/inactive), last upload date
- [ ] Search filters by name or email (case-insensitive, partial match)
- [ ] Department filter is a dropdown populated from available departments
- [ ] Table uses MUI `DataGrid` (from `@mui/x-data-grid`) or a custom MUI `Table` with pagination controls
- [ ] Clicking an employee row navigates to `/admin/employees/:id`
- [ ] Data is fetched via TanStack Query with `queryKey: ['admin', 'employees', { page, pageSize, search, department }]`
- [ ] Page is accessible at `/admin/employees`

---

## Technical Notes

- Server-side pagination preferred (not client-side) for scalability
- Use MediatR `GetEmployeesQuery(page, pageSize, search, departmentId)`

---

## Unit Tests

**Backend (`GetEmployeesQueryHandlerTests`):**
- [ ] Returns a paginated list respecting `page` and `pageSize` parameters
- [ ] Search filter matches by name and email (case-insensitive)
- [ ] Department filter returns only employees in the specified department
- [ ] Total count reflects the full unfiltered result set
- [ ] Deactivated employees are included in the list (with inactive status)

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-7.2](US-7.2-create-employee.md)
