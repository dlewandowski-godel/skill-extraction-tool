# US-3.3 — Document Processing Status

**Epic:** Epic 3 — Document Upload  
**Status:** `[x] Done`

---

## User Story

> As a user, I want to see the upload and processing status (Pending / Processing / Done / Failed) for each of my documents so I know the current state.

---

## Acceptance Criteria

- [ ] `GET /api/documents/my` returns a list of the authenticated user's documents with their current status
- [ ] Possible statuses: `Pending`, `Processing`, `Done`, `Failed`
- [ ] Each document record includes: `documentId`, `documentType`, `fileName`, `status`, `uploadedAt`, `processedAt` (nullable), `errorMessage` (nullable, shown on `Failed`)
- [ ] The frontend polls this endpoint every 5 seconds while any document has `Pending` or `Processing` status (using TanStack Query `refetchInterval`)
- [ ] Polling stops automatically when all documents reach `Done` or `Failed`
- [ ] Status is shown with a MUI `Chip` (color-coded: grey=Pending, blue=Processing, green=Done, red=Failed)
- [ ] A `Failed` status shows a tooltip or expandable message with the error reason
- [ ] Status transitions are: `Pending` → `Processing` → `Done` or `Failed`

---

## Technical Notes

- Status column is updated by the extraction pipeline handler (see US-4.4)
- Use TanStack Query `useQuery` with `refetchInterval: (query) => query.state.data?.some(d => d.status === 'Pending' || d.status === 'Processing') ? 5000 : false`

---

## Unit Tests

**Backend (`GetMyDocumentsQueryHandlerTests`):**
- [ ] Returns only the current user's documents (not other users')
- [ ] Returns documents sorted by `UploadedAt` descending

**Frontend (`useDocumentStatusQuery.test.ts`):**
- [ ] `refetchInterval` returns `5000` when any document is `Pending` or `Processing`
- [ ] `refetchInterval` returns `false` when all documents are `Done` or `Failed`

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-3.2](US-3.2-upload-ifu.md) | **Next:** [US-3.4](US-3.4-file-validation.md)
