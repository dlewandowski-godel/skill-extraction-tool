import { getTopSkills } from "@/lib/analytics-client";
import { useQuery } from "@tanstack/react-query";

export const TOP_SKILLS_QUERY_KEY = ["analytics", "top-skills"] as const;

const STALE_TIME = 2 * 60 * 1000;

export function useTopSkillsQuery(limit = 10) {
  return useQuery({
    queryKey: [...TOP_SKILLS_QUERY_KEY, limit],
    queryFn: () => getTopSkills(limit),
    staleTime: STALE_TIME,
  });
}
