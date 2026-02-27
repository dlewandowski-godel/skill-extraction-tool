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
import { RegisterPage } from "@/pages/RegisterPage";

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

function renderRegister() {
  return render(
    <MemoryRouter
      initialEntries={["/register"]}
      future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
    >
      <Routes>
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/dashboard" element={<div>Dashboard</div>} />
        <Route path="/login" element={<div>Login Page</div>} />
      </Routes>
    </MemoryRouter>,
  );
}

// --- Tests ------------------------------------------------------------------

describe("RegisterPage", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders all fields and submit button", () => {
    mockUseAuth.mockReturnValue(makeAuth());

    renderRegister();

    expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/last name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/^email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/^password$/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/confirm password/i)).toBeInTheDocument();
    expect(screen.getByTestId("submit-button")).toBeInTheDocument();
  });

  it("shows validation error when first name is empty", async () => {
    const user = userEvent.setup();
    mockUseAuth.mockReturnValue(makeAuth());

    renderRegister();

    await user.click(screen.getByTestId("submit-button"));

    expect(
      await screen.findByText("First name is required"),
    ).toBeInTheDocument();
  });

  it("shows validation error when passwords do not match", async () => {
    const user = userEvent.setup();
    mockUseAuth.mockReturnValue(makeAuth());

    renderRegister();

    await user.type(screen.getByTestId("first-name-input"), "John");
    await user.type(screen.getByTestId("last-name-input"), "Doe");
    await user.type(screen.getByLabelText(/^email/i), "john@test.com");
    await user.type(screen.getByTestId("password-input"), "Password1!");
    await user.type(
      screen.getByTestId("confirm-password-input"),
      "Different1!",
    );
    await user.click(screen.getByTestId("submit-button"));

    expect(
      await screen.findByText("Passwords do not match"),
    ).toBeInTheDocument();
  });

  it("redirects to /dashboard on successful registration", async () => {
    const user = userEvent.setup();
    const mockRegister = vi.fn().mockResolvedValueOnce(undefined);
    mockUseAuth.mockReturnValue(makeAuth({ register: mockRegister }));

    renderRegister();

    await user.type(screen.getByTestId("first-name-input"), "John");
    await user.type(screen.getByTestId("last-name-input"), "Doe");
    await user.type(screen.getByLabelText(/^email/i), "john@test.com");
    await user.type(screen.getByTestId("password-input"), "Password1!");
    await user.type(screen.getByTestId("confirm-password-input"), "Password1!");
    await user.click(screen.getByTestId("submit-button"));

    await waitFor(() => {
      expect(screen.getByText("Dashboard")).toBeInTheDocument();
    });
    expect(mockRegister).toHaveBeenCalledWith(
      "john@test.com",
      "Password1!",
      "John",
      "Doe",
    );
  });

  it("shows API error message on failed registration", async () => {
    const user = userEvent.setup();
    const mockRegister = vi.fn().mockRejectedValueOnce({
      response: { data: { message: "Email is already in use." } },
    });
    mockUseAuth.mockReturnValue(makeAuth({ register: mockRegister }));

    renderRegister();

    await user.type(screen.getByTestId("first-name-input"), "John");
    await user.type(screen.getByTestId("last-name-input"), "Doe");
    await user.type(screen.getByLabelText(/^email/i), "taken@test.com");
    await user.type(screen.getByTestId("password-input"), "Password1!");
    await user.type(screen.getByTestId("confirm-password-input"), "Password1!");
    await user.click(screen.getByTestId("submit-button"));

    expect(
      await screen.findByText("Email is already in use."),
    ).toBeInTheDocument();
  });

  it("redirects to /dashboard when already authenticated", () => {
    mockUseAuth.mockReturnValue(
      makeAuth({ isAuthenticated: true, isLoading: false }),
    );

    renderRegister();

    expect(screen.getByText("Dashboard")).toBeInTheDocument();
    expect(screen.queryByLabelText(/first name/i)).not.toBeInTheDocument();
  });

  it("shows a link to the login page", () => {
    mockUseAuth.mockReturnValue(makeAuth());

    renderRegister();

    expect(screen.getByRole("link", { name: /sign in/i })).toHaveAttribute(
      "href",
      "/login",
    );
  });
});
