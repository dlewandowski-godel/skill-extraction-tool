import { AdminRoute } from "@/components/auth/AdminRoute";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import { AdminDashboardPage } from "@/pages/AdminDashboardPage";
import { AdminDepartmentsPage } from "@/pages/AdminDepartmentsPage";
import { AdminEmployeeListPage } from "@/pages/AdminEmployeeListPage";
import { AdminEmployeeProfilePage } from "@/pages/AdminEmployeeProfilePage";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { ProfilePage } from "@/pages/ProfilePage";
import { RegisterPage } from "@/pages/RegisterPage";
import { UploadPage } from "@/pages/UploadPage";
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
        <Route path="/upload" element={<UploadPage />} />
        <Route path="/profile" element={<ProfilePage />} />

        {/* Admin-only */}
        <Route element={<AdminRoute />}>
          <Route path="/admin/employees" element={<AdminEmployeeListPage />} />
          <Route
            path="/admin/employees/:id"
            element={<AdminEmployeeProfilePage />}
          />
          <Route path="/admin/departments" element={<AdminDepartmentsPage />} />
          <Route path="/admin/dashboard" element={<AdminDashboardPage />} />
          <Route
            path="/admin"
            element={<Navigate to="/admin/employees" replace />}
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
