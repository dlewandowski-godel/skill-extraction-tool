import type { AuthContextValue } from "@/contexts/AuthContext";
import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";

// --- Mocks ------------------------------------------------------------------

vi.mock("@/contexts/AuthContext", () => ({
  useAuth: vi.fn(),
}));

// --- Subjects under test ----------------------------------------------------

import { AdminRoute } from "@/components/auth/AdminRoute";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import { useAuth } from "@/contexts/AuthContext";

const mockUseAuth = vi.mocked(useAuth);

// --- Helpers ----------------------------------------------------------------

function makeAuth(overrides: Partial<AuthContextValue> = {}): AuthContextValue {
  return {
    user: null,
    accessToken: null,
    isAuthenticated: false,
    role: null,
    isLoading: false,
    login: vi.fn(),
    register: vi.fn(),
    logout: vi.fn(),
    ...overrides,
  };
}

function renderProtected(initialPath = "/dashboard") {
  return render(
    <MemoryRouter
      initialEntries={[initialPath]}
      future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
    >
      <Routes>
        <Route path="/login" element={<div>Login Page</div>} />
        <Route element={<ProtectedRoute />}>
          <Route path="/dashboard" element={<div>Dashboard</div>} />
        </Route>
      </Routes>
    </MemoryRouter>,
  );
}

function renderAdmin(initialPath = "/admin") {
  return render(
    <MemoryRouter
      initialEntries={[initialPath]}
      future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
    >
      <Routes>
        <Route path="/dashboard" element={<div>Dashboard</div>} />
        <Route element={<AdminRoute />}>
          <Route path="/admin" element={<div>Admin Panel</div>} />
        </Route>
      </Routes>
    </MemoryRouter>,
  );
}

// --- Tests ------------------------------------------------------------------

describe("ProtectedRoute", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows a loading spinner while auth is resolving", () => {
    mockUseAuth.mockReturnValue(makeAuth({ isLoading: true }));

    renderProtected();

    expect(screen.getByTestId("auth-loading")).toBeInTheDocument();
    expect(screen.queryByText("Dashboard")).not.toBeInTheDocument();
    expect(screen.queryByText("Login Page")).not.toBeInTheDocument();
  });

  it("redirects to /login when not authenticated", () => {
    mockUseAuth.mockReturnValue(
      makeAuth({ isAuthenticated: false, isLoading: false }),
    );

    renderProtected();

    expect(screen.getByText("Login Page")).toBeInTheDocument();
    expect(screen.queryByText("Dashboard")).not.toBeInTheDocument();
  });

  it("renders child route when authenticated", () => {
    mockUseAuth.mockReturnValue(
      makeAuth({ isAuthenticated: true, isLoading: false }),
    );

    renderProtected();

    expect(screen.getByText("Dashboard")).toBeInTheDocument();
    expect(screen.queryByText("Login Page")).not.toBeInTheDocument();
  });
});

describe("AdminRoute", () => {
  beforeEach(() => vi.clearAllMocks());

  it("redirects non-admin to /dashboard", () => {
    mockUseAuth.mockReturnValue(makeAuth({ role: "User" }));

    renderAdmin();

    expect(screen.getByText("Dashboard")).toBeInTheDocument();
    expect(screen.queryByText("Admin Panel")).not.toBeInTheDocument();
  });

  it("renders admin content for Admin role", () => {
    mockUseAuth.mockReturnValue(makeAuth({ role: "Admin" }));

    renderAdmin();

    expect(screen.getByText("Admin Panel")).toBeInTheDocument();
    expect(screen.queryByText("Dashboard")).not.toBeInTheDocument();
  });
});
