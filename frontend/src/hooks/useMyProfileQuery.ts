import { getMyProfile } from "@/lib/profile-client";
import { useQuery } from "@tanstack/react-query";

export const MY_PROFILE_QUERY_KEY = ["profile", "me"] as const;

export function useMyProfileQuery() {
  return useQuery({
    queryKey: MY_PROFILE_QUERY_KEY,
    queryFn: getMyProfile,
  });
}
