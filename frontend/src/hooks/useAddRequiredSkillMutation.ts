import { requiredSkillsQueryKey } from "@/hooks/useRequiredSkillsQuery";
import { addRequiredSkill } from "@/lib/taxonomy-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useAddRequiredSkillMutation(departmentId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (skillId: string) => addRequiredSkill(departmentId, skillId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: requiredSkillsQueryKey(departmentId),
      });
    },
  });
}
