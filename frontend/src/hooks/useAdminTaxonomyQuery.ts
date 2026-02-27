import { getAdminTaxonomy } from "@/lib/taxonomy-client";
import { useQuery } from "@tanstack/react-query";

export const adminTaxonomyQueryKey = (search?: string, category?: string) =>
  ["admin", "taxonomy", search ?? "", category ?? ""] as const;

export function useAdminTaxonomyQuery(search?: string, category?: string) {
  return useQuery({
    queryKey: adminTaxonomyQueryKey(search, category),
    queryFn: () => getAdminTaxonomy(search, category),
  });
}
