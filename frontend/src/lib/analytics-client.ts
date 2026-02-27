import { apiClient } from "@/lib/api-client";

export interface TopSkillDto {
  skillName: string;
  employeeCount: number;
}

export interface DepartmentSkillDto {
  name: string;
  count: number;
}

export interface DepartmentSkillsDto {
  department: string;
  skills: DepartmentSkillDto[];
}

export interface SkillGapDto {
  skillName: string;
  employeesWithSkill: number;
  totalEmployees: number;
  gapPercent: number;
}

export interface UploadActivityDto {
  date: string;
  cvCount: number;
  ifuCount: number;
}

export interface ProficiencyDistributionDto {
  level: string;
  count: number;
}

export async function getTopSkills(limit = 10): Promise<TopSkillDto[]> {
  const { data } = await apiClient.get<TopSkillDto[]>(
    `/api/admin/analytics/top-skills?limit=${limit}`,
  );
  return data;
}

export async function getSkillsByDepartment(): Promise<DepartmentSkillsDto[]> {
  const { data } = await apiClient.get<DepartmentSkillsDto[]>(
    "/api/admin/analytics/skills-by-department",
  );
  return data;
}

export async function getSkillGaps(
  department?: string,
): Promise<SkillGapDto[]> {
  const params = department
    ? `?department=${encodeURIComponent(department)}`
    : "";
  const { data } = await apiClient.get<SkillGapDto[]>(
    `/api/admin/analytics/skill-gaps${params}`,
  );
  return data;
}

export async function getUploadActivity(
  period = "30d",
): Promise<UploadActivityDto[]> {
  const { data } = await apiClient.get<UploadActivityDto[]>(
    `/api/admin/analytics/upload-activity?period=${period}`,
  );
  return data;
}

export async function getProficiencyDistribution(): Promise<
  ProficiencyDistributionDto[]
> {
  const { data } = await apiClient.get<ProficiencyDistributionDto[]>(
    "/api/admin/analytics/proficiency-distribution",
  );
  return data;
}
