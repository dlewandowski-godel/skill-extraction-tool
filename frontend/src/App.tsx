import { AdminRoute } from "@/components/auth/AdminRoute";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import { AppLayout } from "@/components/layout/AppLayout";
import { PageSkeleton } from "@/components/layout/PageSkeleton";
import { lazy, Suspense } from "react";
import { Navigate, Route, Routes } from "react-router-dom";

// ---------------------------------------------------------------------------
// Route-level code splitting — each page is a separate bundle chunk
// ---------------------------------------------------------------------------
const AdminDashboardPage = lazy(() =>
  import("@/pages/AdminDashboardPage").then((m) => ({
    default: m.AdminDashboardPage,
  })),
);
const AdminDepartmentsPage = lazy(() =>
  import("@/pages/AdminDepartmentsPage").then((m) => ({
    default: m.AdminDepartmentsPage,
  })),
);
const AdminEmployeeListPage = lazy(() =>
  import("@/pages/AdminEmployeeListPage").then((m) => ({
    default: m.AdminEmployeeListPage,
  })),
);
const AdminEmployeeProfilePage = lazy(() =>
  import("@/pages/AdminEmployeeProfilePage").then((m) => ({
    default: m.AdminEmployeeProfilePage,
  })),
);
const AdminTaxonomyPage = lazy(() =>
  import("@/pages/AdminTaxonomyPage").then((m) => ({
    default: m.AdminTaxonomyPage,
  })),
);
const DashboardPage = lazy(() =>
  import("@/pages/DashboardPage").then((m) => ({ default: m.DashboardPage })),
);
const LoginPage = lazy(() =>
  import("@/pages/LoginPage").then((m) => ({ default: m.LoginPage })),
);
const ProfilePage = lazy(() =>
  import("@/pages/ProfilePage").then((m) => ({ default: m.ProfilePage })),
);
const RegisterPage = lazy(() =>
  import("@/pages/RegisterPage").then((m) => ({ default: m.RegisterPage })),
);
const UploadPage = lazy(() =>
  import("@/pages/UploadPage").then((m) => ({ default: m.UploadPage })),
);

function App() {
  return (
    <Suspense fallback={<PageSkeleton />}>
      <Routes>
        {/* Public */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Authenticated */}
        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/upload" element={<UploadPage />} />
            <Route path="/profile" element={<ProfilePage />} />

            {/* Admin-only */}
            <Route element={<AdminRoute />}>
              <Route
                path="/admin/employees"
                element={<AdminEmployeeListPage />}
              />
              <Route
                path="/admin/employees/:id"
                element={<AdminEmployeeProfilePage />}
              />
              <Route
                path="/admin/departments"
                element={<AdminDepartmentsPage />}
              />
              <Route path="/admin/dashboard" element={<AdminDashboardPage />} />
              <Route path="/admin/taxonomy" element={<AdminTaxonomyPage />} />
              <Route
                path="/admin"
                element={<Navigate to="/admin/employees" replace />}
              />
            </Route>
          </Route>
        </Route>

        {/* Default redirects */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </Suspense>
  );
}

export default App;
