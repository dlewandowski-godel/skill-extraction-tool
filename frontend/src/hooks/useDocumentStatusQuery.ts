import { getMyDocuments, type DocumentDto } from "@/lib/documents-client";
import { useQuery } from "@tanstack/react-query";

export const MY_DOCUMENTS_QUERY_KEY = ["my-documents"] as const;

/**
 * Pure function — exported so it can be unit-tested without React.
 * Returns the refetchInterval value for document status polling.
 */
export function getRefetchInterval(
  data: DocumentDto[] | undefined,
): number | false {
  if (!data) return false;
  const hasActive = data.some(
    (d) => d.status === "Pending" || d.status === "Processing",
  );
  return hasActive ? 5000 : false;
}

export function useDocumentStatusQuery() {
  return useQuery({
    queryKey: MY_DOCUMENTS_QUERY_KEY,
    queryFn: getMyDocuments,
    refetchInterval: (query) => getRefetchInterval(query.state.data),
  });
}
