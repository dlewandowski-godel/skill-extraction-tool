# US-3.2 — Upload IFU (PDF)

**Epic:** Epic 3 — Document Upload  
**Status:** `[x] Done`

---

## User Story

> As a user, I want to upload my IFU (Instructions for Use) document (PDF, max 10 MB) to the system so it can complement my skill profile.

---

## Acceptance Criteria

- [ ] The same `POST /api/documents/upload` endpoint supports `documentType: "IFU"` in addition to `"CV"`
- [ ] Both CV and IFU uploads are shown in the same upload UI section, clearly labelled
- [ ] An IFU document is optional — users without an IFU still get a skill profile from their CV alone
- [ ] A user can have at most one active CV and one active IFU at any time
- [ ] The upload form shows separate upload areas for CV and IFU, each with their own status indicator
- [ ] All other acceptance criteria from US-3.1 apply (validation, storage, MediatR dispatch, progress bar)

---

## Technical Notes

- `DocumentType` is an enum: `CV = 0`, `IFU = 1`
- When both documents are uploaded, the extraction pipeline merges skill results from both (see Epic 4)

---

## Unit Tests

**Backend:**
- [ ] Uploading with `DocumentType.IFU` creates a document record with the correct type
- [ ] A user can have at most one active document per type (second upload replaces, not duplicates)

**Frontend (`UploadForm.test.tsx`):**
- [ ] Both CV and IFU upload areas render with distinct labels
- [ ] Each area maintains its own status independently

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-3.1](US-3.1-upload-cv.md) | **Next:** [US-3.3](US-3.3-processing-status.md)
