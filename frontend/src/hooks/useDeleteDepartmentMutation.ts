import { departmentsQueryKey } from "@/hooks/useDepartmentsQuery";
import { deleteDepartment } from "@/lib/departments-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useDeleteDepartmentMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deleteDepartment(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: departmentsQueryKey() });
    },
  });
}
