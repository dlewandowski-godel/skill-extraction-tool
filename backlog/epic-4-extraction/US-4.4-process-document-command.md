# US-4.4 — ProcessDocumentCommand via MediatR

**Epic:** Epic 4 — Skill Extraction Engine  
**Status:** `[x] Done` <!-- change to [x] Done when complete -->

---

## User Story

> As the system, I want to dispatch a `ProcessDocumentCommand` via MediatR when a document upload is complete so the extraction pipeline is triggered after the upload finishes.

---

## Acceptance Criteria

- [ ] `ProcessDocumentCommand` is a MediatR `IRequest` with `DocumentId` as its payload
- [ ] The command handler (`ProcessDocumentCommandHandler`) orchestrates the full pipeline:
  1. Set document status to `Processing`
  2. Extract text from PDF (via `IPdfTextExtractor`)
  3. Extract skills from text (via `ISkillExtractor`)
  4. Infer proficiency levels
  5. Persist extracted skills (via `IEmployeeSkillRepository`)
  6. Set document status to `Done`
- [ ] On any unhandled exception, the handler sets document status to `Failed`, records the error message, and does NOT rethrow (fail gracefully)
- [ ] The command is dispatched from the upload handler using `IMediator.Send()` (in-process, synchronous for MVP)
- [ ] The upload endpoint returns `202 Accepted` immediately; processing happens synchronously but status is surfaced via polling (US-3.3)
- [ ] All handler steps are wrapped in a database transaction where appropriate

---

## Technical Notes

- For MVP, processing is synchronous (in-process via MediatR). A future enhancement could move this to a background queue (Hangfire / RabbitMQ)
- Log each pipeline step with duration using `ILogger<ProcessDocumentCommandHandler>`
- The pipeline behavior (FluentValidation) runs before the handler

---

## Unit Tests

**Backend (`ProcessDocumentCommandHandlerTests`):**
- [ ] Happy path: document status transitions `Pending → Processing → Done`
- [ ] Handler calls `IPdfTextExtractor.ExtractText()` with the document's file path
- [ ] Handler calls `ISkillExtractor.ExtractSkills()` with the extracted text
- [ ] Extraction failure (extractor throws): status is set to `Failed` with error message, exception is not rethrown
- [ ] Document-not-found: handler returns without crashing and logs a warning
- [ ] All dependencies (`IPdfTextExtractor`, `ISkillExtractor`, repositories) are mocked with NSubstitute

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-4.3](US-4.3-proficiency-inference.md) | **Next:** [US-4.5](US-4.5-persist-skills.md)
