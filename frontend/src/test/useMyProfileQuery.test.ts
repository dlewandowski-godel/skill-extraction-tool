import { MY_PROFILE_QUERY_KEY } from "@/hooks/useMyProfileQuery";
import type { EmployeeProfileDto } from "@/lib/profile-client";
import { getMyProfile } from "@/lib/profile-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/profile-client");

const mockProfile: EmployeeProfileDto = {
  userId: "user-1",
  fullName: "Jane Smith",
  department: "Engineering",
  skills: [
    {
      skillId: "skill-1",
      skillName: "TypeScript",
      category: "Programming",
      proficiencyLevel: "Advanced",
      isManualOverride: false,
      extractedAt: "2024-01-01T00:00:00Z",
    },
  ],
};

describe("useMyProfileQuery", () => {
  it("query key is ['profile', 'me']", () => {
    expect(MY_PROFILE_QUERY_KEY).toEqual(["profile", "me"]);
  });

  it("getMyProfile is called by queryFn", async () => {
    vi.mocked(getMyProfile).mockResolvedValueOnce(mockProfile);
    const result = await getMyProfile();
    expect(result).toEqual(mockProfile);
    expect(getMyProfile).toHaveBeenCalledOnce();
  });

  it("returns profile data on successful API response", async () => {
    vi.mocked(getMyProfile).mockResolvedValueOnce(mockProfile);
    const data = await getMyProfile();
    expect(data.fullName).toBe("Jane Smith");
    expect(data.skills).toHaveLength(1);
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getMyProfile).mockRejectedValueOnce(new Error("Network error"));
    await expect(getMyProfile()).rejects.toThrow("Network error");
  });
});
