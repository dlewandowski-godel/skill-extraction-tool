import { PageSkeleton } from "@/components/layout/PageSkeleton";
import { theme } from "@/theme";
import { ThemeProvider } from "@mui/material";
import { render, screen } from "@testing-library/react";
import React, { Suspense, lazy } from "react";
import { describe, expect, it } from "vitest";

function renderSkeleton() {
  return render(
    <ThemeProvider theme={theme}>
      <PageSkeleton />
    </ThemeProvider>,
  );
}

describe("PageSkeleton", () => {
  it("renders the page-skeleton container", () => {
    renderSkeleton();
    expect(screen.getByTestId("page-skeleton")).toBeInTheDocument();
  });

  it("renders multiple MUI Skeleton elements", () => {
    const { container } = renderSkeleton();
    // MUI Skeleton renders with role="progressbar" or a span with specific classes
    const skeletons = container.querySelectorAll(".MuiSkeleton-root");
    expect(skeletons.length).toBeGreaterThanOrEqual(4);
  });

  it("renders as Suspense fallback while lazy component loads", async () => {
    // Create a lazy component that never resolves during this test
    const NeverReady = lazy(
      () =>
        new Promise<{ default: React.ComponentType }>(() => {
          /* never resolves */
        }),
    );

    render(
      <ThemeProvider theme={theme}>
        <Suspense fallback={<PageSkeleton />}>
          <NeverReady />
        </Suspense>
      </ThemeProvider>,
    );

    expect(screen.getByTestId("page-skeleton")).toBeInTheDocument();
  });
});
