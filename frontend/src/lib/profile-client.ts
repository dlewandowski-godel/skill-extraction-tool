import { apiClient } from "@/lib/api-client";

export type ProficiencyLevel =
  | "Beginner"
  | "Intermediate"
  | "Advanced"
  | "Expert";

export interface SkillDto {
  skillId: string;
  skillName: string;
  category: string;
  proficiencyLevel: ProficiencyLevel;
  isManualOverride: boolean;
  extractedAt: string;
}

export interface EmployeeProfileDto {
  userId: string;
  fullName: string;
  department: string | null;
  skills: SkillDto[];
}

export async function getMyProfile(): Promise<EmployeeProfileDto> {
  const { data } = await apiClient.get<EmployeeProfileDto>("/api/profile/me");
  return data;
}
