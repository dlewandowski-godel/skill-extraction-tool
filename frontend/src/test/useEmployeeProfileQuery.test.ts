import { employeeProfileQueryKey } from "@/hooks/useEmployeeProfileQuery";
import { getEmployeeProfile } from "@/lib/admin-profile-client";
import type { EmployeeProfileDto } from "@/lib/profile-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/admin-profile-client");

const mockProfile: EmployeeProfileDto = {
  userId: "emp-1",
  fullName: "Alice Doe",
  department: "Engineering",
  skills: [
    {
      skillId: "skill-1",
      skillName: "Python",
      category: "Programming",
      proficiencyLevel: "Expert",
      isManualOverride: true,
      extractedAt: "2024-01-01T00:00:00Z",
    },
  ],
};

describe("useEmployeeProfileQuery", () => {
  it("query key includes employeeId", () => {
    expect(employeeProfileQueryKey("emp-1")).toEqual([
      "employee-profile",
      "emp-1",
    ]);
  });

  it("returns profile data on successful API response", async () => {
    vi.mocked(getEmployeeProfile).mockResolvedValueOnce(mockProfile);
    const data = await getEmployeeProfile("emp-1");
    expect(data.fullName).toBe("Alice Doe");
    expect(data.skills).toHaveLength(1);
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getEmployeeProfile).mockRejectedValueOnce(
      new Error("403 Forbidden"),
    );
    await expect(getEmployeeProfile("emp-1")).rejects.toThrow("403 Forbidden");
  });

  it("returns null/throws for non-existent employee", async () => {
    vi.mocked(getEmployeeProfile).mockRejectedValueOnce(
      new Error("404 Not Found"),
    );
    await expect(getEmployeeProfile("no-such-id")).rejects.toThrow(
      "404 Not Found",
    );
  });
});
