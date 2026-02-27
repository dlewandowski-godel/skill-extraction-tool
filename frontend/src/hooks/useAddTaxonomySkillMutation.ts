import { addTaxonomySkill } from "@/lib/taxonomy-client";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useAddTaxonomySkillMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: {
      name: string;
      category: string;
      aliases: string[];
    }) => addTaxonomySkill(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "taxonomy"] });
    },
  });
}
