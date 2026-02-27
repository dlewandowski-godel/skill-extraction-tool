import { getProficiencyDistribution } from "@/lib/analytics-client";
import { useQuery } from "@tanstack/react-query";

export const PROFICIENCY_DISTRIBUTION_QUERY_KEY = [
  "analytics",
  "proficiency-distribution",
] as const;

const STALE_TIME = 2 * 60 * 1000;

export function useProficiencyDistributionQuery() {
  return useQuery({
    queryKey: PROFICIENCY_DISTRIBUTION_QUERY_KEY,
    queryFn: getProficiencyDistribution,
    staleTime: STALE_TIME,
  });
}
