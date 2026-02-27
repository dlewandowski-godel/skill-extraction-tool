import { departmentsQueryKey } from "@/hooks/useDepartmentsQuery";
import { createDepartment } from "@/lib/departments-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useCreateDepartmentMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (name: string) => createDepartment(name),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: departmentsQueryKey() });
    },
  });
}
