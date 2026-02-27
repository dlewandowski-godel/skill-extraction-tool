import { SKILLS_BY_DEPARTMENT_QUERY_KEY } from "@/hooks/useSkillsByDepartmentQuery";
import type { DepartmentSkillsDto } from "@/lib/analytics-client";
import { getSkillsByDepartment } from "@/lib/analytics-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/analytics-client");

const mockData: DepartmentSkillsDto[] = [
  {
    department: "Engineering",
    skills: [
      { name: "TypeScript", count: 5 },
      { name: "Python", count: 3 },
    ],
  },
  {
    department: "Marketing",
    skills: [{ name: "Analytics", count: 2 }],
  },
];

describe("useSkillsByDepartmentQuery", () => {
  it("query key is ['analytics', 'skills-by-department']", () => {
    expect(SKILLS_BY_DEPARTMENT_QUERY_KEY).toEqual([
      "analytics",
      "skills-by-department",
    ]);
  });

  it("getSkillsByDepartment resolves with department list", async () => {
    vi.mocked(getSkillsByDepartment).mockResolvedValueOnce(mockData);
    const result = await getSkillsByDepartment();
    expect(result).toHaveLength(2);
    expect(result[0].department).toBe("Engineering");
    expect(getSkillsByDepartment).toHaveBeenCalledOnce();
  });

  it("returns empty array when no department data", async () => {
    vi.mocked(getSkillsByDepartment).mockResolvedValueOnce([]);
    const result = await getSkillsByDepartment();
    expect(result).toHaveLength(0);
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getSkillsByDepartment).mockRejectedValueOnce(
      new Error("Forbidden"),
    );
    await expect(getSkillsByDepartment()).rejects.toThrow("Forbidden");
  });
});
