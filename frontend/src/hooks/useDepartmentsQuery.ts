import { getDepartments } from "@/lib/departments-client";
import { useQuery } from "@tanstack/react-query";

export const departmentsQueryKey = () => ["admin", "departments"] as const;

export function useDepartmentsQuery() {
  return useQuery({
    queryKey: departmentsQueryKey(),
    queryFn: getDepartments,
  });
}
