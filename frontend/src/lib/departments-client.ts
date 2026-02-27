import { apiClient } from "@/lib/api-client";

export interface DepartmentSummaryDto {
  id: string;
  name: string;
  employeeCount: number;
  requiredSkillCount: number;
}

export async function getDepartments(): Promise<DepartmentSummaryDto[]> {
  const { data } = await apiClient.get<DepartmentSummaryDto[]>(
    "/api/admin/departments",
  );
  return data;
}

export async function createDepartment(
  name: string,
): Promise<{ departmentId: string }> {
  const { data } = await apiClient.post<{ departmentId: string }>(
    "/api/admin/departments",
    { name },
  );
  return data;
}

export async function renameDepartment(
  id: string,
  name: string,
): Promise<void> {
  await apiClient.put(`/api/admin/departments/${id}`, { name });
}

export async function deleteDepartment(id: string): Promise<void> {
  await apiClient.delete(`/api/admin/departments/${id}`);
}
