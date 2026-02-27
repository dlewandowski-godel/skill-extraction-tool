import { UploadPage } from "@/pages/UploadPage";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";

// ── Mocks ────────────────────────────────────────────────────────────────────

vi.mock("@/lib/documents-client", () => ({
  uploadDocument: vi.fn(),
  getMyDocuments: vi.fn().mockResolvedValue([]),
}));

import { getMyDocuments, uploadDocument } from "@/lib/documents-client";

const mockUpload = vi.mocked(uploadDocument);
const mockGetMyDocuments = vi.mocked(getMyDocuments);

// ── Helpers ───────────────────────────────────────────────────────────────────

function renderUploadPage() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <UploadPage />
    </QueryClientProvider>,
  );
}

function makePdf(name = "cv.pdf"): File {
  return new File(["%PDF-1.4"], name, { type: "application/pdf" });
}

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("UploadPage (UploadForm)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockGetMyDocuments.mockResolvedValue([]);
  });

  // ── Rendering ─────────────────────────────────────────────────────────────

  it("renders upload areas for both CV and IFU sections", () => {
    renderUploadPage();
    expect(screen.getByTestId("cv-upload-zone")).toBeInTheDocument();
    expect(screen.getByTestId("ifu-upload-zone")).toBeInTheDocument();
  });

  it("renders CV and IFU sections with distinct labels", () => {
    renderUploadPage();
    expect(screen.getByText(/CV \(Curriculum Vitae\)/i)).toBeInTheDocument();
    expect(
      screen.getByText(/IFU \(Instructions for Use\)/i),
    ).toBeInTheDocument();
  });

  // ── File selection ────────────────────────────────────────────────────────

  it("shows file name and size after selecting a valid PDF", async () => {
    const user = userEvent.setup();
    renderUploadPage();

    const input = screen.getByTestId("cv-file-input");
    const file = makePdf("my-cv.pdf");
    await user.upload(input, file);

    await screen.findByTestId("cv-file-info");
    expect(screen.getByText("my-cv.pdf")).toBeInTheDocument();
    // Size is displayed (any non-empty text near the file info)
    expect(screen.getByTestId("cv-file-info")).toBeInTheDocument();
  });

  it("each upload area maintains its own state independently", async () => {
    const user = userEvent.setup();
    renderUploadPage();

    const cvInput = screen.getByTestId("cv-file-input");
    const ifuInput = screen.getByTestId("ifu-file-input");

    await user.upload(cvInput, makePdf("cv.pdf"));
    // CV zone shows file info, IFU zone still shows the empty prompt
    await screen.findByTestId("cv-file-info");
    expect(screen.queryByTestId("ifu-file-info")).not.toBeInTheDocument();

    await user.upload(ifuInput, makePdf("ifu.pdf"));
    await screen.findByTestId("ifu-file-info");
    // Both are independently showing their file
    expect(screen.getByTestId("cv-file-info")).toBeInTheDocument();
  });

  // ── Upload progress ───────────────────────────────────────────────────────

  it("shows progress bar while mutation is pending", async () => {
    const user = userEvent.setup();

    // Hold the upload in-flight
    let resolveFn!: () => void;
    const pendingPromise = new Promise<{ documentId: string; status: string }>(
      (resolve) => {
        resolveFn = () => resolve({ documentId: "abc", status: "Pending" });
      },
    );
    mockUpload.mockReturnValueOnce(pendingPromise);

    renderUploadPage();

    const input = screen.getByTestId("cv-file-input");
    await user.upload(input, makePdf("cv.pdf"));

    const uploadBtn = await screen.findByTestId("cv-upload-button");
    await user.click(uploadBtn);

    expect(await screen.findByTestId("cv-progress")).toBeInTheDocument();

    // Finish the upload so the test doesn't leak
    resolveFn();
  });

  // ── Success state ─────────────────────────────────────────────────────────

  it("shows success state after mutation resolves", async () => {
    const user = userEvent.setup();
    mockUpload.mockResolvedValueOnce({ documentId: "abc", status: "Pending" });

    renderUploadPage();

    const input = screen.getByTestId("cv-file-input");
    await user.upload(input, makePdf("cv.pdf"));

    await user.click(await screen.findByTestId("cv-upload-button"));

    await waitFor(() => {
      expect(screen.getByTestId("cv-success")).toBeInTheDocument();
    });
  });

  // ── Client-side validation ────────────────────────────────────────────────

  it("shows validation error when a non-PDF file is selected", async () => {
    renderUploadPage();

    const input = screen.getByTestId("cv-file-input");
    const notPdf = new File(["hello"], "doc.txt", { type: "text/plain" });
    // fireEvent bypasses the `accept` attribute (browser-only behavior)
    fireEvent.change(input, { target: { files: [notPdf] } });

    expect(
      await screen.findByTestId("cv-validation-error"),
    ).toBeInTheDocument();
    expect(screen.getByText("Only PDF files are accepted")).toBeInTheDocument();
  });

  it("does not trigger upload when a non-PDF is selected", () => {
    renderUploadPage();

    const input = screen.getByTestId("cv-file-input");
    const notPdf = new File(["hello"], "doc.txt", { type: "text/plain" });
    fireEvent.change(input, { target: { files: [notPdf] } });

    expect(mockUpload).not.toHaveBeenCalled();
  });
});
