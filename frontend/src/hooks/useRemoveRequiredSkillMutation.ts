import { requiredSkillsQueryKey } from "@/hooks/useRequiredSkillsQuery";
import { removeRequiredSkill } from "@/lib/taxonomy-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useRemoveRequiredSkillMutation(departmentId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (skillId: string) => removeRequiredSkill(departmentId, skillId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: requiredSkillsQueryKey(departmentId),
      });
    },
  });
}
