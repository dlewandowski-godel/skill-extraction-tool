import { deactivateTaxonomySkill } from "@/lib/taxonomy-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useDeactivateTaxonomySkillMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deactivateTaxonomySkill(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "taxonomy"] });
    },
  });
}
