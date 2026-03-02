import {
  NotificationProvider,
  useNotify,
} from "@/contexts/NotificationContext";
import { theme } from "@/theme";
import { ThemeProvider } from "@mui/material";
import { act, render, renderHook, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import React from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

function wrapper({ children }: { children: React.ReactNode }) {
  return (
    <ThemeProvider theme={theme}>
      <NotificationProvider>{children}</NotificationProvider>
    </ThemeProvider>
  );
}

describe("NotificationProvider", () => {
  beforeEach(() => vi.useFakeTimers());
  afterEach(() => vi.useRealTimers());

  it("renders the notification container", () => {
    render(
      <ThemeProvider theme={theme}>
        <NotificationProvider>
          <div />
        </NotificationProvider>
      </ThemeProvider>,
    );

    expect(screen.getByTestId("notification-container")).toBeInTheDocument();
  });

  it("renders an Alert for each notification added", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.success("First");
      result.current.error("Second");
    });

    expect(screen.getByText("First")).toBeInTheDocument();
    expect(screen.getByText("Second")).toBeInTheDocument();
  });

  it("dismissing a notification removes it", async () => {
    vi.useRealTimers(); // userEvent needs real timers
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.success("Dismiss me");
    });

    expect(screen.getByText("Dismiss me")).toBeInTheDocument();

    const closeButton = screen.getByRole("button", { name: /close/i });
    await userEvent.click(closeButton);

    expect(screen.queryByText("Dismiss me")).not.toBeInTheDocument();
  });

  it("multiple notifications display in FIFO order", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.success("First message");
      result.current.warning("Second message");
      result.current.error("Third message");
    });

    const alerts = screen.getAllByRole("alert");
    expect(alerts.length).toBe(3);

    // FIFO: first added should be first in the list
    expect(alerts[0].textContent).toContain("First message");
    expect(alerts[1].textContent).toContain("Second message");
    expect(alerts[2].textContent).toContain("Third message");
  });

  it("shows a maximum of 3 notifications at once", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.success("One");
      result.current.success("Two");
      result.current.success("Three");
      result.current.success("Four"); // queued, not shown yet
    });

    const alerts = screen.getAllByRole("alert");
    expect(alerts.length).toBe(3);
    expect(screen.queryByText("Four")).not.toBeInTheDocument();
  });
});
