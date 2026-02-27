import { apiClient } from "@/lib/api-client";

export interface TaxonomySkillDto {
  skillId: string;
  name: string;
  category: string;
}

export async function getTaxonomySkills(): Promise<TaxonomySkillDto[]> {
  const { data } = await apiClient.get<TaxonomySkillDto[]>("/api/skills");
  return data;
}
