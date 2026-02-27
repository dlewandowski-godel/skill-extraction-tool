import EditIcon from "@mui/icons-material/Edit";
import { Chip, Tooltip } from "@mui/material";
import type { ChipOwnProps } from "@mui/material/Chip";

type ProficiencyLevel = "Beginner" | "Intermediate" | "Advanced" | "Expert";

interface ProficiencyChipProps {
  level: ProficiencyLevel;
  isOverride?: boolean;
}

const LEVEL_COLOR: Record<ProficiencyLevel, ChipOwnProps["color"]> = {
  Beginner: "default",
  Intermediate: "primary",
  Advanced: "warning",
  Expert: "success",
};

export function ProficiencyChip({
  level,
  isOverride = false,
}: ProficiencyChipProps) {
  const chip = (
    <Chip
      label={level}
      color={LEVEL_COLOR[level]}
      size="small"
      icon={isOverride ? <EditIcon fontSize="small" /> : undefined}
    />
  );

  if (isOverride) {
    return <Tooltip title="Manually set">{chip}</Tooltip>;
  }

  return chip;
}
