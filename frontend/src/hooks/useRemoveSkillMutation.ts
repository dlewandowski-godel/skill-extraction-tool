import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { removeSkillFromEmployee } from "@/lib/admin-profile-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

interface RemoveSkillVariables {
  employeeId: string;
  skillId: string;
}

export function useRemoveSkillMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ employeeId, skillId }: RemoveSkillVariables) =>
      removeSkillFromEmployee(employeeId, skillId),
    onSuccess: (_data, { employeeId }) => {
      queryClient.invalidateQueries({
        queryKey: employeeProfileQueryKey(employeeId),
      });
    },
  });
}
