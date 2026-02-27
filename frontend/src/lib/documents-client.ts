import { apiClient } from "@/lib/api-client";

export type DocumentStatus = "Pending" | "Processing" | "Done" | "Failed";
export type DocumentType = "CV" | "IFU";

export interface DocumentDto {
  documentId: string;
  documentType: DocumentType;
  fileName: string;
  status: DocumentStatus;
  uploadedAt: string;
  processedAt: string | null;
  errorMessage: string | null;
}

export interface UploadResult {
  documentId: string;
  status: string;
}

export async function uploadDocument(
  file: File,
  documentType: DocumentType,
  onProgress?: (percent: number) => void,
): Promise<UploadResult> {
  const fd = new FormData();
  fd.append("file", file);
  fd.append("documentType", documentType);

  const { data } = await apiClient.post<UploadResult>(
    "/api/documents/upload",
    fd,
    {
      headers: { "Content-Type": "multipart/form-data" },
      onUploadProgress: (e) => {
        if (e.total) {
          onProgress?.(Math.round((e.loaded / e.total) * 100));
        }
      },
    },
  );

  return data;
}

export async function getMyDocuments(): Promise<DocumentDto[]> {
  const { data } = await apiClient.get<DocumentDto[]>("/api/documents/my");
  return data;
}
