import type { AuthContextValue } from "@/contexts/AuthContext";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";

// --- Mocks ------------------------------------------------------------------

vi.mock("@/contexts/AuthContext", () => ({
  useAuth: vi.fn(),
}));

// --- Subjects under test ----------------------------------------------------

import { useAuth } from "@/contexts/AuthContext";
import { LoginPage } from "@/pages/LoginPage";

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

function renderLogin() {
  return render(
    <MemoryRouter
      initialEntries={["/login"]}
      future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
    >
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/dashboard" element={<div>Dashboard</div>} />
      </Routes>
    </MemoryRouter>,
  );
}

// --- Tests ------------------------------------------------------------------

describe("LoginPage", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders email, password fields and submit button", () => {
    mockUseAuth.mockReturnValue(makeAuth());

    renderLogin();

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByTestId("submit-button")).toBeInTheDocument();
  });

  it("shows validation error when email is empty", async () => {
    const user = userEvent.setup();
    mockUseAuth.mockReturnValue(makeAuth());

    renderLogin();

    await user.click(screen.getByTestId("submit-button"));

    expect(await screen.findByText("Email is required")).toBeInTheDocument();
  });

  it("shows validation error when password is empty", async () => {
    const user = userEvent.setup();
    mockUseAuth.mockReturnValue(makeAuth());

    renderLogin();

    await user.type(screen.getByLabelText(/email/i), "valid@test.com");
    await user.click(screen.getByTestId("submit-button"));

    expect(await screen.findByText("Password is required")).toBeInTheDocument();
  });

  it("redirects to /dashboard on successful login", async () => {
    const user = userEvent.setup();
    const mockLogin = vi.fn().mockResolvedValueOnce(undefined);
    mockUseAuth.mockReturnValue(makeAuth({ login: mockLogin }));

    renderLogin();

    await user.type(screen.getByLabelText(/email/i), "admin@test.com");
    await user.type(screen.getByLabelText(/password/i), "Password1!");
    await user.click(screen.getByTestId("submit-button"));

    await waitFor(() => {
      expect(screen.getByText("Dashboard")).toBeInTheDocument();
    });
    expect(mockLogin).toHaveBeenCalledWith("admin@test.com", "Password1!");
  });

  it("shows API error message on failed login", async () => {
    const user = userEvent.setup();
    const mockLogin = vi.fn().mockRejectedValueOnce({
      response: { data: { message: "Invalid credentials. Please try again." } },
    });
    mockUseAuth.mockReturnValue(makeAuth({ login: mockLogin }));

    renderLogin();

    await user.type(screen.getByLabelText(/email/i), "bad@test.com");
    await user.type(screen.getByLabelText(/password/i), "wrongpass");
    await user.click(screen.getByTestId("submit-button"));

    expect(
      await screen.findByText("Invalid credentials. Please try again."),
    ).toBeInTheDocument();
  });

  it("redirects to /dashboard when already authenticated", () => {
    mockUseAuth.mockReturnValue(
      makeAuth({ isAuthenticated: true, isLoading: false }),
    );

    renderLogin();

    expect(screen.getByText("Dashboard")).toBeInTheDocument();
    expect(screen.queryByLabelText(/email/i)).not.toBeInTheDocument();
  });
});
