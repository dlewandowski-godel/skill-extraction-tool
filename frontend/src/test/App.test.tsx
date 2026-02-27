import { theme } from "@/theme";
import { ThemeProvider } from "@mui/material";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen } from "@testing-library/react";
import React from "react";
import { MemoryRouter } from "react-router-dom";
import { vi } from "vitest";
import App from "../App";

// Mock AuthContext so App can render without a real auth server
vi.mock("@/contexts/AuthContext", () => ({
  useAuth: vi.fn(() => ({
    isAuthenticated: false,
    isLoading: true, // spinner shown; no redirect yet
    user: null,
    role: null,
    login: vi.fn(),
    register: vi.fn(),
    logout: vi.fn(),
    accessToken: null,
  })),
  AuthProvider: ({ children }: { children: React.ReactNode }) => (
    <>{children}</>
  ),
}));

function renderApp(initialPath = "/dashboard") {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter
        initialEntries={[initialPath]}
        future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
      >
        <ThemeProvider theme={theme}>
          <App />
        </ThemeProvider>
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe("App", () => {
  it("renders without crashing and shows loading spinner on protected routes", () => {
    renderApp("/dashboard");
    expect(screen.getByTestId("auth-loading")).toBeInTheDocument();
  });
});
