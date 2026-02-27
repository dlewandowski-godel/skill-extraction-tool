import { apiClient } from "@/lib/api-client";
import type { PagedResult } from "@/lib/types";

export interface EmployeeListItemDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  departmentName: string | null;
  role: string;
  isActive: boolean;
  lastUploadDate: string | null;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  departmentId: string | null;
}

export interface CreateEmployeeResponse {
  employeeId: string;
  tempPassword: string;
  message: string;
}

export interface EditEmployeeRequest {
  firstName: string;
  lastName: string;
  departmentId: string | null;
  role: string;
}

export interface GetEmployeesParams {
  page?: number;
  pageSize?: number;
  search?: string;
  department?: string;
}

export async function getEmployees(
  params: GetEmployeesParams = {},
): Promise<PagedResult<EmployeeListItemDto>> {
  const { page = 1, pageSize = 20, search, department } = params;
  const query = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
    ...(search ? { search } : {}),
    ...(department ? { department } : {}),
  });
  const { data } = await apiClient.get<PagedResult<EmployeeListItemDto>>(
    `/api/admin/employees?${query}`,
  );
  return data;
}

export async function createEmployee(
  request: CreateEmployeeRequest,
): Promise<CreateEmployeeResponse> {
  const { data } = await apiClient.post<CreateEmployeeResponse>(
    "/api/admin/employees",
    request,
  );
  return data;
}

export async function editEmployee(
  id: string,
  request: EditEmployeeRequest,
): Promise<void> {
  await apiClient.put(`/api/admin/employees/${id}`, request);
}

export async function deactivateEmployee(id: string): Promise<void> {
  await apiClient.put(`/api/admin/employees/${id}/deactivate`);
}

export async function activateEmployee(id: string): Promise<void> {
  await apiClient.put(`/api/admin/employees/${id}/activate`);
}
