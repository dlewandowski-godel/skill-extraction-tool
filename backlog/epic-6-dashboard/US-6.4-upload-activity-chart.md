# US-6.4 — Upload Activity Over Time Chart

**Epic:** Epic 6 — Admin Dashboard & Analytics  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a line chart of document upload activity over time so I can monitor employee onboarding pace.

---

## Acceptance Criteria

- [ ] `GET /api/admin/analytics/upload-activity?period=30d` returns daily upload counts for the past N days
- [ ] Period options: `7d`, `30d`, `90d` (configurable via a toggle in the UI)
- [ ] Frontend renders a line chart (recharts `LineChart`) with date on X-axis and upload count on Y-axis
- [ ] Two lines: CV uploads and IFU uploads (different colors)
- [ ] Hovering a data point shows a tooltip with date, CV count, and IFU count
- [ ] Chart title: "Document Upload Activity"
- [ ] Data is loaded via TanStack Query; period change triggers a refetch

---

## Technical Notes

- API groups `Documents.UploadedAt` by date and `DocumentType`
- Return format: `{ date: string, cvCount: number, ifuCount: number }[]`
- Zero-fill days with no uploads so the chart line is continuous

---

## Unit Tests

**Backend (`GetUploadActivityQueryHandlerTests`):**
- [ ] Returns one entry per day for the requested period
- [ ] Days with no uploads are zero-filled (not missing from the response)
- [ ] CV and IFU counts are reported separately per day
- [ ] Period is correctly bounded (e.g., `30d` returns exactly 30 entries)

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-6.3](US-6.3-skill-gap-analysis.md) | **Next:** [US-6.5](US-6.5-proficiency-distribution.md)
