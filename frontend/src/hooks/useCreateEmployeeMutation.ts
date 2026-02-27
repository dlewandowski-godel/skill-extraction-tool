import {
  createEmployee,
  type CreateEmployeeRequest,
} from "@/lib/employees-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useCreateEmployeeMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateEmployeeRequest) => createEmployee(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "employees"] });
    },
  });
}
