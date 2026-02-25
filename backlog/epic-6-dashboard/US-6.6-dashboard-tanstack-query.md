# US-6.6 — Dashboard Data via TanStack Query

**Epic:** Epic 6 — Admin Dashboard & Analytics  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want all dashboard data loaded via TanStack Query so charts refresh without full page reloads and stale data is shown while revalidating.

---

## Acceptance Criteria

- [ ] Each chart widget uses its own `useQuery` hook with a specific `queryKey`
- [ ] `staleTime` is set to 2 minutes for all analytics queries (data doesn't need to be real-time)
- [ ] While data is revalidating in the background, the previous data is shown (no flash of loading state)
- [ ] Each chart widget shows a MUI `Skeleton` placeholder on initial load only
- [ ] A "Refresh" button on the dashboard triggers `queryClient.invalidateQueries({ queryKey: ['analytics'] })` to refetch all charts
- [ ] Failed analytics queries show an error state within the chart card (not a full-page error)
- [ ] Custom hooks are extracted: `useTopSkillsQuery`, `useSkillsByDepartmentQuery`, `useSkillGapsQuery`, `useUploadActivityQuery`, `useProficiencyDistributionQuery`

---

## Technical Notes

- All analytics query keys should share a common prefix `['analytics']` for bulk invalidation
- Use TanStack Query `placeholderData: keepPreviousData` for filter-change scenarios (e.g., department selector)

---

## Unit Tests

**Frontend (analytics hook tests with `renderHook` + `createTestQueryClient()`):**
- [ ] Each custom hook (`useTopSkillsQuery`, `useSkillsByDepartmentQuery`, `useSkillGapsQuery`, `useUploadActivityQuery`, `useProficiencyDistributionQuery`) returns data on a mocked successful response
- [ ] Each hook returns an error state on API failure
- [ ] `staleTime` is ≥ 2 minutes on all analytics queries
- [ ] `queryClient.invalidateQueries({ queryKey: ['analytics'] })` causes all analytics hooks to refetch
- [ ] `placeholderData: keepPreviousData` is used in hooks that accept a filter parameter

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-6.5](US-6.5-proficiency-distribution.md)
