import { getSkillsByDepartment } from "@/lib/analytics-client";
import { keepPreviousData, useQuery } from "@tanstack/react-query";

export const SKILLS_BY_DEPARTMENT_QUERY_KEY = [
  "analytics",
  "skills-by-department",
] as const;

const STALE_TIME = 2 * 60 * 1000;

export function useSkillsByDepartmentQuery() {
  return useQuery({
    queryKey: SKILLS_BY_DEPARTMENT_QUERY_KEY,
    queryFn: getSkillsByDepartment,
    staleTime: STALE_TIME,
    placeholderData: keepPreviousData,
  });
}
