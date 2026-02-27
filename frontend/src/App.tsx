import { AdminRoute } from "@/components/auth/AdminRoute";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { RegisterPage } from "@/pages/RegisterPage";
import { Navigate, Route, Routes } from "react-router-dom";

function App() {
  return (
    <Routes>
      {/* Public */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Authenticated */}
      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<DashboardPage />} />

        {/* Admin-only */}
        <Route element={<AdminRoute />}>
          <Route
            path="/admin/*"
            element={<div>Admin area (coming soon)</div>}
          />
        </Route>
      </Route>

      {/* Default redirects */}
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;
