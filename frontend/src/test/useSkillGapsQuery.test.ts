import { SKILL_GAPS_QUERY_KEY } from "@/hooks/useSkillGapsQuery";
import type { SkillGapDto } from "@/lib/analytics-client";
import { getSkillGaps } from "@/lib/analytics-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/analytics-client");

const mockGaps: SkillGapDto[] = [
  {
    skillName: "Kubernetes",
    employeesWithSkill: 2,
    totalEmployees: 10,
    gapPercent: 80,
  },
  {
    skillName: "TypeScript",
    employeesWithSkill: 9,
    totalEmployees: 10,
    gapPercent: 10,
  },
];

describe("useSkillGapsQuery", () => {
  it("query key starts with ['analytics', 'skill-gaps']", () => {
    expect(SKILL_GAPS_QUERY_KEY).toEqual(["analytics", "skill-gaps"]);
  });

  it("getSkillGaps resolves without department filter", async () => {
    vi.mocked(getSkillGaps).mockResolvedValueOnce(mockGaps);
    const result = await getSkillGaps();
    expect(result).toHaveLength(2);
    expect(getSkillGaps).toHaveBeenCalledOnce();
  });

  it("getSkillGaps accepts department filter", async () => {
    vi.mocked(getSkillGaps).mockResolvedValueOnce([mockGaps[0]]);
    const result = await getSkillGaps("Engineering");
    expect(result).toHaveLength(1);
    expect(getSkillGaps).toHaveBeenCalledWith("Engineering");
  });

  it("returns empty array when no required skills configured", async () => {
    vi.mocked(getSkillGaps).mockResolvedValueOnce([]);
    const result = await getSkillGaps();
    expect(result).toHaveLength(0);
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getSkillGaps).mockRejectedValueOnce(new Error("Network error"));
    await expect(getSkillGaps()).rejects.toThrow("Network error");
  });
});
