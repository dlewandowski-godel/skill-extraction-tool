import {
  NotificationProvider,
  useNotify,
} from "@/contexts/NotificationContext";
import { theme } from "@/theme";
import { ThemeProvider } from "@mui/material";
import { act, renderHook, screen } from "@testing-library/react";
import React from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

function wrapper({ children }: { children: React.ReactNode }) {
  return (
    <ThemeProvider theme={theme}>
      <NotificationProvider>{children}</NotificationProvider>
    </ThemeProvider>
  );
}

describe("useNotify", () => {
  beforeEach(() => vi.useFakeTimers());
  afterEach(() => vi.useRealTimers());

  it("throws when used outside NotificationProvider", () => {
    const consoleError = vi
      .spyOn(console, "error")
      .mockImplementation(() => {});
    expect(() => renderHook(() => useNotify())).toThrow(
      "useNotify must be used within <NotificationProvider>",
    );
    consoleError.mockRestore();
  });

  it("notify.success adds a success notification", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.success("Upload complete");
    });

    expect(screen.getByText("Upload complete")).toBeInTheDocument();
    expect(screen.getByTestId("notification-success")).toBeInTheDocument();
  });

  it("notify.error adds an error notification", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.error("Something failed");
    });

    expect(screen.getByText("Something failed")).toBeInTheDocument();
    expect(screen.getByTestId("notification-error")).toBeInTheDocument();
  });

  it("notify.warning adds a warning notification", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.warning("Check this");
    });

    expect(screen.getByText("Check this")).toBeInTheDocument();
    expect(screen.getByTestId("notification-warning")).toBeInTheDocument();
  });

  it("success notification auto-dismisses after 4 seconds", async () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.success("Auto gone");
    });

    expect(screen.getByText("Auto gone")).toBeInTheDocument();

    act(() => {
      vi.advanceTimersByTime(4000);
    });

    expect(screen.queryByText("Auto gone")).not.toBeInTheDocument();
  });

  it("error notification does not auto-dismiss after 4 seconds", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.error("Stays here");
    });

    act(() => {
      vi.advanceTimersByTime(10000);
    });

    expect(screen.getByText("Stays here")).toBeInTheDocument();
  });

  it("warning notification auto-dismisses after 6 seconds", () => {
    const { result } = renderHook(() => useNotify(), { wrapper });

    act(() => {
      result.current.warning("Warning fades");
    });

    act(() => {
      vi.advanceTimersByTime(6000);
    });

    expect(screen.queryByText("Warning fades")).not.toBeInTheDocument();
  });
});
