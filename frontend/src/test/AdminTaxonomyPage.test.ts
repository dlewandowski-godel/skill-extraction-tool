import { adminTaxonomyQueryKey } from "@/hooks/useAdminTaxonomyQuery";
import type { TaxonomyAdminSkillDto } from "@/lib/taxonomy-client";
import {
  activateTaxonomySkill,
  addRequiredSkill,
  addTaxonomySkill,
  deactivateTaxonomySkill,
  getAdminTaxonomy,
  getRequiredSkills,
  removeRequiredSkill,
  updateTaxonomySkill,
} from "@/lib/taxonomy-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/taxonomy-client");

const mockSkills: TaxonomyAdminSkillDto[] = [
  {
    id: "1",
    name: "Python",
    category: "Programming",
    aliases: ["Python", "py"],
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: "2",
    name: "AutoCAD",
    category: "Design",
    aliases: ["AutoCAD"],
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: "3",
    name: "COBOL",
    category: "Programming",
    aliases: ["COBOL"],
    isActive: false,
    createdAt: new Date().toISOString(),
  },
];

describe("taxonomy-client", () => {
  it("getAdminTaxonomy — resolves with skill list", async () => {
    vi.mocked(getAdminTaxonomy).mockResolvedValueOnce(mockSkills);
    const result = await getAdminTaxonomy();
    expect(result).toEqual(mockSkills);
  });

  it("getAdminTaxonomy — passes search and category params", async () => {
    vi.mocked(getAdminTaxonomy).mockResolvedValueOnce([]);
    await getAdminTaxonomy("py", "programming");
    expect(getAdminTaxonomy).toHaveBeenCalledWith("py", "programming");
  });

  it("getAdminTaxonomy — propagates error on failure", async () => {
    vi.mocked(getAdminTaxonomy).mockRejectedValueOnce(
      new Error("Unauthorized"),
    );
    await expect(getAdminTaxonomy()).rejects.toThrow("Unauthorized");
  });

  it("addTaxonomySkill — resolves with skillId", async () => {
    vi.mocked(addTaxonomySkill).mockResolvedValueOnce({ skillId: "abc" });
    const result = await addTaxonomySkill({
      name: "Python",
      category: "Programming",
      aliases: ["Python"],
    });
    expect(result.skillId).toBe("abc");
  });

  it("addTaxonomySkill — throws on conflict", async () => {
    vi.mocked(addTaxonomySkill).mockRejectedValueOnce(
      Object.assign(new Error("Conflict"), { response: { status: 409 } }),
    );
    await expect(
      addTaxonomySkill({
        name: "Python",
        category: "Programming",
        aliases: [],
      }),
    ).rejects.toThrow("Conflict");
  });

  it("updateTaxonomySkill — resolves on success", async () => {
    vi.mocked(updateTaxonomySkill).mockResolvedValueOnce(undefined);
    await expect(
      updateTaxonomySkill("1", {
        name: "Python 3",
        category: "Programming",
        aliases: ["Python 3"],
      }),
    ).resolves.toBeUndefined();
  });

  it("deactivateTaxonomySkill — resolves on success", async () => {
    vi.mocked(deactivateTaxonomySkill).mockResolvedValueOnce(undefined);
    await expect(deactivateTaxonomySkill("1")).resolves.toBeUndefined();
  });

  it("activateTaxonomySkill — resolves on success", async () => {
    vi.mocked(activateTaxonomySkill).mockResolvedValueOnce(undefined);
    await expect(activateTaxonomySkill("1")).resolves.toBeUndefined();
  });

  it("getRequiredSkills — resolves with list", async () => {
    vi.mocked(getRequiredSkills).mockResolvedValueOnce([
      { skillId: "1", name: "Python", category: "Programming" },
    ]);
    const result = await getRequiredSkills("dept-1");
    expect(result).toHaveLength(1);
    expect(getRequiredSkills).toHaveBeenCalledWith("dept-1");
  });

  it("addRequiredSkill — resolves on success", async () => {
    vi.mocked(addRequiredSkill).mockResolvedValueOnce(undefined);
    await expect(
      addRequiredSkill("dept-1", "skill-1"),
    ).resolves.toBeUndefined();
    expect(addRequiredSkill).toHaveBeenCalledWith("dept-1", "skill-1");
  });

  it("removeRequiredSkill — resolves on success", async () => {
    vi.mocked(removeRequiredSkill).mockResolvedValueOnce(undefined);
    await expect(
      removeRequiredSkill("dept-1", "skill-1"),
    ).resolves.toBeUndefined();
  });
});

describe("adminTaxonomyQueryKey", () => {
  it("returns correct key without filters", () => {
    expect(adminTaxonomyQueryKey()).toEqual(["admin", "taxonomy", "", ""]);
  });

  it("returns correct key with search", () => {
    expect(adminTaxonomyQueryKey("python")).toEqual([
      "admin",
      "taxonomy",
      "python",
      "",
    ]);
  });

  it("returns correct key with both filters", () => {
    expect(adminTaxonomyQueryKey("py", "Programming")).toEqual([
      "admin",
      "taxonomy",
      "py",
      "Programming",
    ]);
  });
});
