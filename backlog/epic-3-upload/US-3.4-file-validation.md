# US-3.4 — File Validation

**Epic:** Epic 3 — Document Upload  
**Status:** `[x] Done`

---

## User Story

> As the system, I want uploaded PDFs validated (type, size) before storage so invalid files are rejected early with a clear error message.

---

## Acceptance Criteria

- [ ] File MIME type must be `application/pdf` — other types return `400` with message "Only PDF files are accepted"
- [ ] File size must not exceed 10 MB — larger files return `400` with message "File size must not exceed 10 MB"
- [ ] An empty file (0 bytes) is rejected with message "Uploaded file is empty"
- [ ] Frontend validates file type and size before initiating the upload (no wasted network request)
- [ ] Backend validation is independent of frontend validation (never trust client-side only)
- [ ] Validation is implemented as a MediatR `IPipelineBehavior<UploadDocumentCommand>` using FluentValidation
- [ ] MIME type is validated by magic bytes (first 4 bytes = `%PDF`) in addition to the Content-Type header

---

## Technical Notes

- Magic byte check: PDF starts with `0x25 0x50 0x44 0x46` (`%PDF`)
- Use `IFormFile.OpenReadStream()` with a small buffer to check magic bytes without reading the whole file
- FluentValidation `AbstractValidator<UploadDocumentCommand>` handles all rules

---

## Unit Tests

**Backend (`UploadDocumentCommandValidatorTests`):**
- [ ] File with correct MIME type and magic bytes (`%PDF`) passes validation
- [ ] File with MIME type `application/pdf` but wrong magic bytes fails with "Only PDF files are accepted"
- [ ] File with size > 10 MB fails with "File size must not exceed 10 MB"
- [ ] File with 0 bytes fails with "Uploaded file is empty"
- [ ] File with non-PDF MIME type fails with "Only PDF files are accepted"

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-3.3](US-3.3-processing-status.md) | **Next:** [US-3.5](US-3.5-file-storage.md)
