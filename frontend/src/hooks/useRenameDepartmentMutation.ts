import { departmentsQueryKey } from "@/hooks/useDepartmentsQuery";
import { renameDepartment } from "@/lib/departments-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

interface RenameDepartmentVariables {
  id: string;
  name: string;
}

export function useRenameDepartmentMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, name }: RenameDepartmentVariables) =>
      renameDepartment(id, name),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: departmentsQueryKey() });
    },
  });
}
