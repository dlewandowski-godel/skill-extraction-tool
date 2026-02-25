# US-4.1 — PDF Text Extraction (PdfPig)

**Epic:** Epic 4 — Skill Extraction Engine  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As the system, I want to extract plain text from uploaded PDF files using PdfPig so the content is available for downstream skill analysis.

---

## Acceptance Criteria

- [ ] `PdfPig` NuGet package is installed in `Infrastructure`
- [ ] `IPdfTextExtractor` interface is defined in `Application` with method `string ExtractText(string filePath)`
- [ ] `PdfPigTextExtractor` implementation in `Infrastructure` reads the file from the given path and returns all text
- [ ] Text extraction uses `ContentOrderTextExtractor.GetText(page)` (correct reading order, not `page.Text`)
- [ ] Multi-page PDFs are fully extracted (all pages concatenated)
- [ ] Empty or unreadable PDFs return an empty string and log a warning (do not throw)
- [ ] Password-protected PDFs return an empty string and set document status to `Failed` with message "PDF is password protected"
- [ ] Extracted text is stored transiently in memory during processing (not persisted to DB)

---

## Technical Notes

- PdfPig v0.1.x is the target; use `UglyToad.PdfPig` namespace
- `ContentOrderTextExtractor` is in `UglyToad.PdfPig.Content`
- Wrap `PdfDocument.Open(filePath)` in a `try/catch` to handle corrupt/encrypted files gracefully

---

## Unit Tests

**Backend (`PdfPigTextExtractorTests`):**
- [ ] Returns a non-empty string for a valid fixture PDF (small PDF checked into `Tests/Fixtures/sample.pdf`)
- [ ] Concatenates text from all pages of a multi-page PDF
- [ ] Returns an empty string for a 0-byte / empty PDF (no exception thrown)
- [ ] Returns an empty string and does not throw for a corrupt/unreadable file
- [ ] Returns an empty string for a password-protected PDF

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-4.2](US-4.2-mlnet-matching.md)
