import { useAuth } from "@/contexts/AuthContext";
import { Navigate, Outlet, useLocation } from "react-router-dom";

/**
 * Wraps admin-only routes.
 * Non-admin users are redirected to /dashboard with a "Not authorized" message
 * passed via location state so the dashboard can display a toast / alert.
 */
export function AdminRoute() {
  const { role } = useAuth();
  const location = useLocation();

  if (role !== "Admin") {
    return (
      <Navigate
        to="/dashboard"
        state={{ message: "Not authorized", from: location }}
        replace
      />
    );
  }

  return <Outlet />;
}
