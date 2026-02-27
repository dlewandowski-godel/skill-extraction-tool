import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { activateEmployee } from "@/lib/employees-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useActivateEmployeeMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (employeeId: string) => activateEmployee(employeeId),
    onSuccess: (_data, employeeId) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "employees"] });
      queryClient.invalidateQueries({
        queryKey: employeeProfileQueryKey(employeeId),
      });
    },
  });
}
