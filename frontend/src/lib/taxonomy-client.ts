import { apiClient } from "@/lib/api-client";

export interface TaxonomyAdminSkillDto {
  id: string;
  name: string;
  category: string;
  aliases: string[];
  isActive: boolean;
  createdAt: string;
}

export interface RequiredSkillDto {
  skillId: string;
  name: string;
  category: string;
}

export async function getAdminTaxonomy(
  search?: string,
  category?: string,
): Promise<TaxonomyAdminSkillDto[]> {
  const params: Record<string, string> = {};
  if (search) params.search = search;
  if (category) params.category = category;
  const { data } = await apiClient.get<TaxonomyAdminSkillDto[]>(
    "/api/admin/taxonomy",
    { params },
  );
  return data;
}

export async function addTaxonomySkill(payload: {
  name: string;
  category: string;
  aliases: string[];
}): Promise<{ skillId: string }> {
  const { data } = await apiClient.post<{ skillId: string }>(
    "/api/admin/taxonomy",
    payload,
  );
  return data;
}

export async function updateTaxonomySkill(
  id: string,
  payload: { name: string; category: string; aliases: string[] },
): Promise<void> {
  await apiClient.put(`/api/admin/taxonomy/${id}`, payload);
}

export async function deactivateTaxonomySkill(id: string): Promise<void> {
  await apiClient.delete(`/api/admin/taxonomy/${id}`);
}

export async function activateTaxonomySkill(id: string): Promise<void> {
  await apiClient.put(`/api/admin/taxonomy/${id}/activate`, {});
}

export async function getRequiredSkills(
  departmentId: string,
): Promise<RequiredSkillDto[]> {
  const { data } = await apiClient.get<RequiredSkillDto[]>(
    `/api/admin/departments/${departmentId}/required-skills`,
  );
  return data;
}

export async function addRequiredSkill(
  departmentId: string,
  skillId: string,
): Promise<void> {
  await apiClient.post(
    `/api/admin/departments/${departmentId}/required-skills`,
    { skillId },
  );
}

export async function removeRequiredSkill(
  departmentId: string,
  skillId: string,
): Promise<void> {
  await apiClient.delete(
    `/api/admin/departments/${departmentId}/required-skills/${skillId}`,
  );
}
