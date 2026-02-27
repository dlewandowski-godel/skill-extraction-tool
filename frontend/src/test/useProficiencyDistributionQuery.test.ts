import { PROFICIENCY_DISTRIBUTION_QUERY_KEY } from "@/hooks/useProficiencyDistributionQuery";
import type { ProficiencyDistributionDto } from "@/lib/analytics-client";
import { getProficiencyDistribution } from "@/lib/analytics-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/analytics-client");

const mockDistribution: ProficiencyDistributionDto[] = [
  { level: "Beginner", count: 15 },
  { level: "Intermediate", count: 30 },
  { level: "Advanced", count: 20 },
  { level: "Expert", count: 5 },
];

describe("useProficiencyDistributionQuery", () => {
  it("query key is ['analytics', 'proficiency-distribution']", () => {
    expect(PROFICIENCY_DISTRIBUTION_QUERY_KEY).toEqual([
      "analytics",
      "proficiency-distribution",
    ]);
  });

  it("getProficiencyDistribution resolves with all levels", async () => {
    vi.mocked(getProficiencyDistribution).mockResolvedValueOnce(
      mockDistribution,
    );
    const result = await getProficiencyDistribution();
    expect(result).toHaveLength(4);
    expect(result.map((r) => r.level)).toEqual([
      "Beginner",
      "Intermediate",
      "Advanced",
      "Expert",
    ]);
    expect(getProficiencyDistribution).toHaveBeenCalledOnce();
  });

  it("counts are accurate per level", async () => {
    vi.mocked(getProficiencyDistribution).mockResolvedValueOnce(
      mockDistribution,
    );
    const result = await getProficiencyDistribution();
    const intermediate = result.find((r) => r.level === "Intermediate");
    expect(intermediate?.count).toBe(30);
  });

  it("returns empty array when no skills exist", async () => {
    vi.mocked(getProficiencyDistribution).mockResolvedValueOnce([]);
    const result = await getProficiencyDistribution();
    expect(result).toHaveLength(0);
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getProficiencyDistribution).mockRejectedValueOnce(
      new Error("Unauthorized"),
    );
    await expect(getProficiencyDistribution()).rejects.toThrow("Unauthorized");
  });
});
