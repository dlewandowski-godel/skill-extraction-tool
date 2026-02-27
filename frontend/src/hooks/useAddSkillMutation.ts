import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { addSkillToEmployee } from "@/lib/admin-profile-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

interface AddSkillVariables {
  employeeId: string;
  skillId: string;
  proficiencyLevel: string;
}

export function useAddSkillMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      employeeId,
      skillId,
      proficiencyLevel,
    }: AddSkillVariables) =>
      addSkillToEmployee(employeeId, skillId, proficiencyLevel),
    onSuccess: (_data, { employeeId }) => {
      queryClient.invalidateQueries({
        queryKey: employeeProfileQueryKey(employeeId),
      });
    },
  });
}
