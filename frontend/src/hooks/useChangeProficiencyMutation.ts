import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { changeProficiency } from "@/lib/admin-profile-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

interface ChangeProficiencyVariables {
  employeeId: string;
  skillId: string;
  proficiencyLevel: string;
}

export function useChangeProficiencyMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      employeeId,
      skillId,
      proficiencyLevel,
    }: ChangeProficiencyVariables) =>
      changeProficiency(employeeId, skillId, proficiencyLevel),
    onSuccess: (_data, { employeeId }) => {
      queryClient.invalidateQueries({
        queryKey: employeeProfileQueryKey(employeeId),
      });
    },
  });
}
