import { apiClient } from "@/lib/api-client";
import type { EmployeeProfileDto } from "@/lib/profile-client";

export async function getEmployeeProfile(
  employeeId: string,
): Promise<EmployeeProfileDto> {
  const { data } = await apiClient.get<EmployeeProfileDto>(
    `/api/admin/employees/${employeeId}/profile`,
  );
  return data;
}

export async function addSkillToEmployee(
  employeeId: string,
  skillId: string,
  proficiencyLevel: string,
): Promise<void> {
  await apiClient.post(`/api/admin/employees/${employeeId}/skills`, {
    skillId,
    proficiencyLevel,
  });
}

export async function removeSkillFromEmployee(
  employeeId: string,
  skillId: string,
): Promise<void> {
  await apiClient.delete(
    `/api/admin/employees/${employeeId}/skills/${skillId}`,
  );
}

export async function changeProficiency(
  employeeId: string,
  skillId: string,
  proficiencyLevel: string,
): Promise<void> {
  await apiClient.put(`/api/admin/employees/${employeeId}/skills/${skillId}`, {
    proficiencyLevel,
  });
}
