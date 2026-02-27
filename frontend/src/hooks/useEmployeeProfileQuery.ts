import { getEmployeeProfile } from "@/lib/admin-profile-client";
import { useQuery } from "@tanstack/react-query";

export const employeeProfileQueryKey = (employeeId: string) =>
  ["employee-profile", employeeId] as const;

export function useEmployeeProfileQuery(employeeId: string) {
  return useQuery({
    queryKey: employeeProfileQueryKey(employeeId),
    queryFn: () => getEmployeeProfile(employeeId),
    enabled: !!employeeId,
  });
}
