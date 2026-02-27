import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { editEmployee, type EditEmployeeRequest } from "@/lib/employees-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

interface EditEmployeeVariables {
  employeeId: string;
  request: EditEmployeeRequest;
}

export function useEditEmployeeMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ employeeId, request }: EditEmployeeVariables) =>
      editEmployee(employeeId, request),
    onSuccess: (_data, { employeeId }) => {
      queryClient.invalidateQueries({ queryKey: ["admin", "employees"] });
      queryClient.invalidateQueries({
        queryKey: employeeProfileQueryKey(employeeId),
      });
    },
  });
}
