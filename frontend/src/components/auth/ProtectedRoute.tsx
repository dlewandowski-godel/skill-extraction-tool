import { useAuth } from "@/contexts/AuthContext";
import { Box, CircularProgress } from "@mui/material";
import { Navigate, Outlet, useLocation } from "react-router-dom";

/**
 * Wraps all authenticated routes.
 * - While the initial silent-refresh is running, show a centered spinner.
 * - Unauthenticated users are redirected to /login, with the original
 *   path saved in location.state so we can send them back after login.
 */
export function ProtectedRoute() {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          mt: 12,
        }}
        data-testid="auth-loading"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
}
