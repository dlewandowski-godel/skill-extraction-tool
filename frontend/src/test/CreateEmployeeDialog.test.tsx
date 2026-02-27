import { CreateEmployeeDialog } from "@/components/admin/CreateEmployeeDialog";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";

// ── Mocks ────────────────────────────────────────────────────────────────────

vi.mock("@/lib/departments-client", () => ({
  getDepartments: vi.fn().mockResolvedValue([
    {
      id: "dept-1",
      name: "Engineering",
      employeeCount: 5,
      requiredSkillCount: 0,
    },
    { id: "dept-2", name: "Sales", employeeCount: 3, requiredSkillCount: 0 },
  ]),
}));

vi.mock("@/lib/employees-client", () => ({
  createEmployee: vi.fn().mockResolvedValue({
    employeeId: "new-id",
    tempPassword: "TempPass123!",
    message: "Created",
  }),
}));

import { createEmployee } from "@/lib/employees-client";

const mockCreateEmployee = vi.mocked(createEmployee);

// ── Helpers ───────────────────────────────────────────────────────────────────

function renderDialog(onClose = vi.fn(), onCreated = vi.fn()) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <CreateEmployeeDialog open onClose={onClose} onCreated={onCreated} />
    </QueryClientProvider>,
  );
}

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("CreateEmployeeDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders all required form fields", async () => {
    renderDialog();

    expect(screen.getByLabelText(/First Name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Last Name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Email/i)).toBeInTheDocument();
    // MUI Select renders label as visible text; no direct aria linkage in JSDOM
    expect(screen.getByText("Role")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Create/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Cancel/i })).toBeInTheDocument();
  });

  it("shows validation errors when required fields are empty and Create is clicked", async () => {
    const user = userEvent.setup();
    renderDialog();

    await user.click(screen.getByRole("button", { name: /Create/i }));

    expect(
      await screen.findByText(/First name is required/i),
    ).toBeInTheDocument();
    expect(screen.getByText(/Last name is required/i)).toBeInTheDocument();
    expect(screen.getByText(/Email is required/i)).toBeInTheDocument();
    expect(mockCreateEmployee).not.toHaveBeenCalled();
  });

  it("shows validation error for an invalid email address", async () => {
    const user = userEvent.setup();
    renderDialog();

    await user.type(screen.getByLabelText(/First Name/i), "Jane");
    await user.type(screen.getByLabelText(/Last Name/i), "Doe");
    await user.type(screen.getByLabelText(/Email/i), "not-an-email");
    await user.click(screen.getByRole("button", { name: /Create/i }));

    expect(
      await screen.findByText(/Enter a valid email address/i),
    ).toBeInTheDocument();
    expect(mockCreateEmployee).not.toHaveBeenCalled();
  });

  it("calls createEmployee and invokes onCreated with the temp password on success", async () => {
    const user = userEvent.setup();
    const onCreated = vi.fn();
    renderDialog(vi.fn(), onCreated);

    await user.type(screen.getByLabelText(/First Name/i), "Jane");
    await user.type(screen.getByLabelText(/Last Name/i), "Doe");
    await user.type(screen.getByLabelText(/Email/i), "jane@example.com");
    await user.click(screen.getByRole("button", { name: /Create/i }));

    await waitFor(() => {
      expect(mockCreateEmployee).toHaveBeenCalledWith(
        expect.objectContaining({
          firstName: "Jane",
          lastName: "Doe",
          email: "jane@example.com",
          role: "Employee",
        }),
      );
      expect(onCreated).toHaveBeenCalledWith("TempPass123!");
    });
  });
});
