# US-3.5 — File Storage Persistence

**Epic:** Epic 3 — Document Upload  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As the system, I want uploaded files stored on the server filesystem (or a named Docker volume) and referenced by path in the database so files are persisted across container restarts.

---

## Acceptance Criteria

- [ ] Uploaded files are stored in a directory configured by `FILE_STORAGE_PATH` environment variable
- [ ] Directory structure: `{FILE_STORAGE_PATH}/{userId}/{guid}-{documentType}.pdf`
- [ ] The full file path is stored in the `Documents.FilePath` database column
- [ ] In Docker, `FILE_STORAGE_PATH` maps to a named volume so files survive `docker-compose down` + `docker-compose up`
- [ ] The API container has write permissions to the storage volume
- [ ] If `FILE_STORAGE_PATH` does not exist on startup, it is created automatically
- [ ] An `IFileStorageService` interface in `Application` abstracts the storage implementation (filesystem today, blob tomorrow)
- [ ] `FileStorageService` implementation lives in `Infrastructure`

---

## Technical Notes

- Never serve files directly from the API via a static file URL (security risk); serve via an authenticated endpoint if needed
- Include the named volume in `docker-compose.yml` under `volumes:` at the root level

---

## Unit Tests

**Backend (`FileStorageServiceTests`):**
- [ ] `SaveAsync()` writes the file to the expected path (`{userId}/{guid}-{documentType}.pdf`)
- [ ] `SaveAsync()` creates the target directory if it does not exist
- [ ] `DeleteAsync()` removes the file if it exists and returns `true`
- [ ] `DeleteAsync()` returns `false` (no exception) if the file does not exist

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-3.4](US-3.4-file-validation.md)
