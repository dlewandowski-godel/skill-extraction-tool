import { getEmployees, type GetEmployeesParams } from "@/lib/employees-client";
import { useQuery } from "@tanstack/react-query";

export const employeesQueryKey = (params: GetEmployeesParams = {}) =>
  ["admin", "employees", params] as const;

export function useEmployeesQuery(params: GetEmployeesParams = {}) {
  return useQuery({
    queryKey: employeesQueryKey(params),
    queryFn: () => getEmployees(params),
  });
}
