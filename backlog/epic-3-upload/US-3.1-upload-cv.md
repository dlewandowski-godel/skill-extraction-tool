# US-3.1 — Upload CV (PDF)

**Epic:** Epic 3 — Document Upload  
**Status:** `[x] Done`

---

## User Story

> As a user, I want to upload my CV (PDF, max 10 MB) to the system so it can be processed for skill extraction.

---

## Acceptance Criteria

- [ ] `POST /api/documents/upload` accepts a `multipart/form-data` request with a `file` field and `documentType: "CV"`
- [ ] Only authenticated users can upload; unauthenticated requests return `401`
- [ ] Uploaded file is validated: must be `application/pdf`, max size 10 MB
- [ ] Invalid files return `400 Bad Request` with a descriptive error message
- [ ] On success, the endpoint returns `{ documentId, status: "Pending" }`
- [ ] A record is created in the `Documents` table with `UserId`, `FileName`, `FilePath`, `DocumentType`, `Status`, `UploadedAt`
- [ ] A `ProcessDocumentCommand` is dispatched via MediatR after the document is saved
- [ ] A user can upload a new CV to replace the previous one (the old document is retained for audit, new one becomes active)
- [ ] Frontend upload form uses drag-and-drop or file picker, shows file name and size after selection
- [ ] Upload progress is shown via a MUI `LinearProgress` bar during the upload

---

## Technical Notes

- Store file with a GUID-based name to avoid collisions: `{userId}/{guid}.pdf`
- File storage path is configurable via environment variable `FILE_STORAGE_PATH`
- Max file size enforced both on the frontend (before upload) and the backend (Kestrel request size limit)

---

## Unit Tests

**Backend (`UploadDocumentCommandHandlerTests`):**
- [ ] Valid upload command saves a document record with status `Pending` and dispatches `ProcessDocumentCommand`
- [ ] Handler calls `IFileStorageService.SaveAsync()` with the correct path pattern
- [ ] Handler returns the new `documentId` and `status: "Pending"` on success

**Frontend (`UploadForm.test.tsx`):**
- [ ] Upload area renders for both CV sections
- [ ] Selecting a valid PDF file shows the file name and size
- [ ] Upload progress bar renders while the mutation is pending
- [ ] Success state shows after the mutation resolves

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-3.2](US-3.2-upload-ifu.md)
