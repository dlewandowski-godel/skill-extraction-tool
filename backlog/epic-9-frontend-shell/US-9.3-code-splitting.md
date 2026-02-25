# US-9.3 — Route-Level Code Splitting

**Epic:** Epic 9 — Frontend Shell & UX  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a developer, I want route-level code splitting (lazy loading) so the initial bundle is small and the app loads fast.

---

## Acceptance Criteria

- [ ] All page-level components are loaded via `React.lazy()` + `import()`
- [ ] Each route is wrapped in `<Suspense fallback={<PageSkeleton />}>`
- [ ] `npm run build` shows that the bundle is split into multiple chunks (one per lazy-loaded route)
- [ ] The admin chunk is loaded only when an admin navigates to an admin route (not shipped to non-admin users on initial load)
- [ ] Build output checked: no single chunk exceeds 500 KB (gzipped target: < 150 KB initial)

---

## Technical Notes

- Use `React.lazy(() => import('@/pages/admin/AdminDashboard'))` pattern
- Vite handles code splitting automatically when using dynamic `import()`
- `<PageSkeleton>` is a full-page loading placeholder (MUI `Skeleton` bars)

---

## Unit Tests

> Code splitting is a build-time concern and is best verified by inspecting Vite's bundle output. No meaningful unit tests apply here; verify via `vite build` that each lazily imported page produces a separate chunk.

**Frontend (`PageSkeleton.test.tsx`):**
- [ ] `<PageSkeleton>` renders the expected number of MUI `Skeleton` bars
- [ ] `<Suspense fallback={<PageSkeleton />}>` renders the fallback while the lazy component loads

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-9.2](US-9.2-admin-sidebar.md) | **Next:** [US-9.4](US-9.4-loading-skeletons.md)
