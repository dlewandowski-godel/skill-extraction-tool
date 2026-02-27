import { UPLOAD_ACTIVITY_QUERY_KEY } from "@/hooks/useUploadActivityQuery";
import type { UploadActivityDto } from "@/lib/analytics-client";
import { getUploadActivity } from "@/lib/analytics-client";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/lib/analytics-client");

const mockActivity: UploadActivityDto[] = [
  { date: "2024-03-01", cvCount: 3, ifuCount: 1 },
  { date: "2024-03-02", cvCount: 0, ifuCount: 2 },
  { date: "2024-03-03", cvCount: 5, ifuCount: 0 },
];

describe("useUploadActivityQuery", () => {
  it("query key starts with ['analytics', 'upload-activity']", () => {
    expect(UPLOAD_ACTIVITY_QUERY_KEY).toEqual(["analytics", "upload-activity"]);
  });

  it("getUploadActivity resolves with activity data for default period", async () => {
    vi.mocked(getUploadActivity).mockResolvedValueOnce(mockActivity);
    const result = await getUploadActivity("30d");
    expect(result).toHaveLength(3);
    expect(result[0].date).toBe("2024-03-01");
    expect(getUploadActivity).toHaveBeenCalledWith("30d");
  });

  it("accepts custom period parameter", async () => {
    vi.mocked(getUploadActivity).mockResolvedValueOnce([]);
    await getUploadActivity("7d");
    expect(getUploadActivity).toHaveBeenCalledWith("7d");
  });

  it("includes both cvCount and ifuCount in each entry", async () => {
    vi.mocked(getUploadActivity).mockResolvedValueOnce(mockActivity);
    const result = await getUploadActivity();
    expect(result[0]).toHaveProperty("cvCount");
    expect(result[0]).toHaveProperty("ifuCount");
  });

  it("propagates error on API failure", async () => {
    vi.mocked(getUploadActivity).mockRejectedValueOnce(
      new Error("Server error"),
    );
    await expect(getUploadActivity()).rejects.toThrow("Server error");
  });
});
