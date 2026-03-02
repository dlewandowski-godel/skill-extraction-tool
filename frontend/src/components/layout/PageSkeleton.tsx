import { Box, Skeleton } from "@mui/material";

/**
 * Full-page loading placeholder shown while a lazily-loaded route is being
 * fetched. Mimics a typical content page with a heading + several text rows.
 */
export function PageSkeleton() {
  return (
    <Box p={4} data-testid="page-skeleton">
      {/* Heading */}
      <Skeleton variant="text" width="35%" height={40} sx={{ mb: 2 }} />
      {/* Card / content rows */}
      <Skeleton variant="rounded" height={120} sx={{ mb: 2 }} />
      <Skeleton variant="rounded" height={80} sx={{ mb: 1 }} />
      <Skeleton variant="rounded" height={80} sx={{ mb: 1 }} />
      <Skeleton variant="rounded" height={80} />
    </Box>
  );
}
