import { getSkillGaps } from "@/lib/analytics-client";
import { keepPreviousData, useQuery } from "@tanstack/react-query";

export const SKILL_GAPS_QUERY_KEY = ["analytics", "skill-gaps"] as const;

const STALE_TIME = 2 * 60 * 1000;

export function useSkillGapsQuery(department?: string) {
  return useQuery({
    queryKey: [...SKILL_GAPS_QUERY_KEY, department ?? null],
    queryFn: () => getSkillGaps(department),
    staleTime: STALE_TIME,
    placeholderData: keepPreviousData,
  });
}
