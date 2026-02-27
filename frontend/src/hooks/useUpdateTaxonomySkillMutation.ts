import { updateTaxonomySkill } from "@/lib/taxonomy-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useUpdateTaxonomySkillMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      payload,
    }: {
      id: string;
      payload: { name: string; category: string; aliases: string[] };
    }) => updateTaxonomySkill(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "taxonomy"] });
    },
  });
}
