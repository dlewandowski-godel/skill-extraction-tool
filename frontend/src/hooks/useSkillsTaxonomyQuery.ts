import { getTaxonomySkills } from "@/lib/skills-client";
import { useQuery } from "@tanstack/react-query";

export const TAXONOMY_SKILLS_QUERY_KEY = ["taxonomy-skills"] as const;

export function useSkillsTaxonomyQuery() {
  return useQuery({
    queryKey: TAXONOMY_SKILLS_QUERY_KEY,
    queryFn: getTaxonomySkills,
    staleTime: 5 * 60 * 1000, // 5 minutes — taxonomy is fairly static
  });
}
