import { activateTaxonomySkill } from "@/lib/taxonomy-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useActivateTaxonomySkillMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => activateTaxonomySkill(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "taxonomy"] });
    },
  });
}
