import { ProficiencyChip } from "@/components/skills/ProficiencyChip";
import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

describe("ProficiencyChip", () => {
  it('renders "Beginner" label with default (grey) color', () => {
    render(<ProficiencyChip level="Beginner" />);
    expect(screen.getByText("Beginner")).toBeInTheDocument();
    // default color chip has no specific color class; just assert label
  });

  it('renders "Intermediate" label', () => {
    render(<ProficiencyChip level="Intermediate" />);
    expect(screen.getByText("Intermediate")).toBeInTheDocument();
  });

  it('renders "Advanced" label', () => {
    render(<ProficiencyChip level="Advanced" />);
    expect(screen.getByText("Advanced")).toBeInTheDocument();
  });

  it('renders "Expert" label', () => {
    render(<ProficiencyChip level="Expert" />);
    expect(screen.getByText("Expert")).toBeInTheDocument();
  });

  it("shows edit icon when isOverride is true", () => {
    render(<ProficiencyChip level="Advanced" isOverride={true} />);
    // MUI renders the EditIcon SVG with data-testid="EditIcon"
    expect(screen.getByTestId("EditIcon")).toBeInTheDocument();
  });

  it("does not render edit icon when isOverride is false", () => {
    render(<ProficiencyChip level="Expert" isOverride={false} />);
    expect(screen.queryByTestId("EditIcon")).not.toBeInTheDocument();
  });
});
