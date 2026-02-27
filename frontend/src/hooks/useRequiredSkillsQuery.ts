import { getRequiredSkills } from "@/lib/taxonomy-client";
import { useQuery } from "@tanstack/react-query";

export const requiredSkillsQueryKey = (departmentId: string) =>
  ["admin", "departments", departmentId, "required-skills"] as const;

export function useRequiredSkillsQuery(departmentId: string) {
  return useQuery({
    queryKey: requiredSkillsQueryKey(departmentId),
    queryFn: () => getRequiredSkills(departmentId),
    enabled: !!departmentId,
  });
}
