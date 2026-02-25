# US-9.5 — Toast / Snackbar Notifications

**Epic:** Epic 9 — Frontend Shell & UX  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want toast/snackbar notifications for success and error states so I always know what happened after an action.

---

## Acceptance Criteria

- [ ] Success notifications: green MUI `Snackbar` with `Alert severity="success"`, auto-dismiss after 4 seconds
- [ ] Error notifications: red MUI `Snackbar` with `Alert severity="error"`, requires manual dismiss (no auto-dismiss)
- [ ] Warning notifications: orange MUI `Snackbar` with `Alert severity="warning"`, auto-dismiss after 6 seconds
- [ ] Notifications triggered from: upload success/failure, skill added/removed, login error, role change, account deactivation
- [ ] A notification context/hook (`useNotify`) is available globally: `notify.success('Upload complete')`, `notify.error('Processing failed')`
- [ ] Maximum 3 notifications visible at once; older ones are queued
- [ ] Notifications appear in the bottom-right corner of the screen

---

## Technical Notes

- Implement a `NotificationProvider` context that manages a queue of notifications and renders them
- TanStack Query `useMutation` `onSuccess` and `onError` callbacks call `useNotify()` to trigger notifications
- MUI `Snackbar` + `Alert` is the component pair to use

---

## Unit Tests

**Frontend (`useNotify.test.ts`):**
- [ ] `notify.success(message)` adds a success notification to the queue
- [ ] `notify.error(message)` adds an error notification to the queue
- [ ] Success notifications auto-dismiss after the configured duration
- [ ] Error notifications require explicit user dismissal (no auto-dismiss)

**Frontend (`NotificationProvider.test.tsx`):**
- [ ] Renders a `Snackbar` for each notification in the queue
- [ ] Dismissing a notification removes it from the queue
- [ ] Multiple queued notifications display in FIFO order

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-9.4](US-9.4-loading-skeletons.md)
