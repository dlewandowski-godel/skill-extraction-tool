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
  firstName: string;
  lastName: string;
  department: string | null;
  departmentId: string | null;
  role: string | null;
  isActive: boolean;
  skills: SkillDto[];
}

export async function getMyProfile(): Promise<EmployeeProfileDto> {
  const { data } = await apiClient.get<EmployeeProfileDto>("/api/profile/me");
  return data;
}
