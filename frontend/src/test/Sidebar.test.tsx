import { Sidebar } from "@/components/layout/Sidebar";
import { theme } from "@/theme";
import { ThemeProvider } from "@mui/material";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";

// Mock AuthContext
vi.mock("@/contexts/AuthContext", () => ({
  useAuth: vi.fn(() => ({
    user: { id: "1", email: "admin@test.com", role: "Admin" },
    role: "Admin",
    logout: vi.fn(),
  })),
}));

function renderSidebar(initialPath = "/dashboard") {
  return render(
    <ThemeProvider theme={theme}>
      <MemoryRouter
        initialEntries={[initialPath]}
        future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
      >
        <Sidebar mobileOpen={false} onMobileClose={vi.fn()} />
      </MemoryRouter>
    </ThemeProvider>,
  );
}

describe("Sidebar", () => {
  it("renders all common navigation links", () => {
    renderSidebar();

    expect(screen.getAllByText("Dashboard").length).toBeGreaterThan(0);
    expect(screen.getAllByText("My Profile").length).toBeGreaterThan(0);
    expect(screen.getAllByText("Upload Documents").length).toBeGreaterThan(0);
  });

  it("renders admin-only navigation links for admin users", () => {
    renderSidebar();

    expect(screen.getAllByText("Employees").length).toBeGreaterThan(0);
    expect(screen.getAllByText("Taxonomy").length).toBeGreaterThan(0);
    expect(screen.getAllByText("Departments").length).toBeGreaterThan(0);
    expect(screen.getAllByText("Admin Dashboard").length).toBeGreaterThan(0);
  });

  it("active route link has aria-current='page' on /dashboard", () => {
    renderSidebar("/dashboard");

    // NavLink sets aria-current="page" on the active link
    const activeLinks = screen.getAllByRole("link", {
      current: "page",
    });
    expect(activeLinks.length).toBeGreaterThan(0);
    const activeHrefs = activeLinks.map((l) => l.getAttribute("href"));
    expect(activeHrefs.some((h) => h === "/dashboard")).toBe(true);
  });

  it("active route link has aria-current='page' on /admin/taxonomy", () => {
    renderSidebar("/admin/taxonomy");

    const activeLinks = screen.getAllByRole("link", { current: "page" });
    const activeHrefs = activeLinks.map((l) => l.getAttribute("href"));
    expect(activeHrefs.some((h) => h === "/admin/taxonomy")).toBe(true);
  });

  it("renders the user email", () => {
    renderSidebar();
    expect(screen.getAllByText("admin@test.com").length).toBeGreaterThan(0);
  });

  it("renders a logout button", () => {
    renderSidebar();
    expect(
      screen.getAllByRole("button", { name: /logout/i }).length,
    ).toBeGreaterThan(0);
  });
});
