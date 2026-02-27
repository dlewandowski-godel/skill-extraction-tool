import { TOP_SKILLS_QUERY_KEY } from "@/hooks/useTopSkillsQuery";
import type { TopSkillDto } from "@/lib/analytics-client";
import { getTopSkills } from "@/lib/analytics-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/analytics-client");

const mockTopSkills: TopSkillDto[] = [
  { skillName: "TypeScript", employeeCount: 12 },
  { skillName: "Python", employeeCount: 8 },
];

describe("useTopSkillsQuery", () => {
  it("query key starts with ['analytics', 'top-skills']", () => {
    expect(TOP_SKILLS_QUERY_KEY).toEqual(["analytics", "top-skills"]);
  });

  it("getTopSkills resolves with skill list", async () => {
    vi.mocked(getTopSkills).mockResolvedValueOnce(mockTopSkills);
    const result = await getTopSkills(10);
    expect(result).toEqual(mockTopSkills);
    expect(getTopSkills).toHaveBeenCalledWith(10);
  });

  it("returns empty array when API returns empty list", async () => {
    vi.mocked(getTopSkills).mockResolvedValueOnce([]);
    const result = await getTopSkills(10);
    expect(result).toHaveLength(0);
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getTopSkills).mockRejectedValueOnce(new Error("Unauthorized"));
    await expect(getTopSkills(10)).rejects.toThrow("Unauthorized");
  });
});
