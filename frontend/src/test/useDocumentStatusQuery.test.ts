import { getRefetchInterval } from "@/hooks/useDocumentStatusQuery";
import type { DocumentDto } from "@/lib/documents-client";
import { describe, expect, it } from "vitest";

function makeDoc(status: DocumentDto["status"]): DocumentDto {
  return {
    documentId: crypto.randomUUID(),
    documentType: "CV",
    fileName: "test.pdf",
    status,
    uploadedAt: new Date().toISOString(),
    processedAt: null,
    errorMessage: null,
  };
}

describe("getRefetchInterval", () => {
  it("returns 5000 when any document is Pending", () => {
    expect(getRefetchInterval([makeDoc("Pending")])).toBe(5000);
  });

  it("returns 5000 when any document is Processing", () => {
    expect(getRefetchInterval([makeDoc("Processing")])).toBe(5000);
  });

  it("returns 5000 when mix of Done and Pending", () => {
    expect(getRefetchInterval([makeDoc("Done"), makeDoc("Pending")])).toBe(
      5000,
    );
  });

  it("returns 5000 when mix of Done and Processing", () => {
    expect(getRefetchInterval([makeDoc("Done"), makeDoc("Processing")])).toBe(
      5000,
    );
  });

  it("returns false when all documents are Done", () => {
    expect(getRefetchInterval([makeDoc("Done"), makeDoc("Done")])).toBe(false);
  });

  it("returns false when all documents are Failed", () => {
    expect(getRefetchInterval([makeDoc("Failed"), makeDoc("Failed")])).toBe(
      false,
    );
  });

  it("returns false when documents are mix of Done and Failed", () => {
    expect(getRefetchInterval([makeDoc("Done"), makeDoc("Failed")])).toBe(
      false,
    );
  });

  it("returns false when data is undefined", () => {
    expect(getRefetchInterval(undefined)).toBe(false);
  });

  it("returns false when data is an empty array", () => {
    expect(getRefetchInterval([])).toBe(false);
  });
});
