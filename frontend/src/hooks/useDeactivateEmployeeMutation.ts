import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { deactivateEmployee } from "@/lib/employees-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useDeactivateEmployeeMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (employeeId: string) => deactivateEmployee(employeeId),
    onSuccess: (_data, employeeId) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "employees"] });
      queryClient.invalidateQueries({
        queryKey: employeeProfileQueryKey(employeeId),
      });
    },
  });
}
