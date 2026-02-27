import { getUploadActivity } from "@/lib/analytics-client";
import { useQuery } from "@tanstack/react-query";

export const UPLOAD_ACTIVITY_QUERY_KEY = [
  "analytics",
  "upload-activity",
] as const;

const STALE_TIME = 2 * 60 * 1000;

export function useUploadActivityQuery(period = "30d") {
  return useQuery({
    queryKey: [...UPLOAD_ACTIVITY_QUERY_KEY, period],
    queryFn: () => getUploadActivity(period),
    staleTime: STALE_TIME,
  });
}
