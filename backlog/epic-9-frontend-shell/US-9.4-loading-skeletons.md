# US-9.4 — Loading Skeletons

**Epic:** Epic 9 — Frontend Shell & UX  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want a loading skeleton shown when data is fetching so the UI doesn't feel broken or blank while waiting for content.

---

## Acceptance Criteria

- [ ] All data-loading pages show a skeleton layout while TanStack Query status is `'pending'`
- [ ] Skeleton shapes match the actual content layout (e.g., a skill card skeleton looks like a skill card outline)
- [ ] Skeletons are implemented using MUI `Skeleton` with `variant="rectangular"` or `"text"`
- [ ] Skeleton components are co-located with their parent page/component (`SkillCardSkeleton`, `EmployeeListSkeleton`, etc.)
- [ ] Re-fetches in the background (stale-while-revalidate) do NOT show skeletons — only initial loads do
- [ ] Chart widgets show a skeleton placeholder sized to match the chart area on initial load

---

## Technical Notes

- Use TanStack Query's `isLoading` (= `isPending && isFetching`) to conditionally show skeletons
- Avoid showing skeletons on background refetches — use `isFetching && !isLoading` to show a subtle top loading bar instead (MUI `LinearProgress` at the top of the page)

---

## Unit Tests

**Frontend (`EmployeeList.test.tsx` / `SkillsTable.test.tsx`):**
- [ ] Skeleton component is shown when `isLoading === true`
- [ ] Skeleton is NOT shown when `isFetching === true` but `isLoading === false` (background refetch)
- [ ] `LinearProgress` bar is shown during background refetch (`isFetching && !isLoading`)
- [ ] Data renders correctly once loading completes

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-9.3](US-9.3-code-splitting.md) | **Next:** [US-9.5](US-9.5-notifications.md)
