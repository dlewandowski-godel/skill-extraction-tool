import { useChangeProficiencyMutation } from "@/hooks/useChangeProficiencyMutation";
import type { ProficiencyLevel } from "@/lib/profile-client";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
} from "@mui/material";
import { useState } from "react";

const PROFICIENCY_LEVELS = [
  "Beginner",
  "Intermediate",
  "Advanced",
  "Expert",
] as const;

interface ChangeProficiencyDialogProps {
  open: boolean;
  employeeId: string;
  skillId: string;
  skillName: string;
  currentLevel: ProficiencyLevel;
  onClose: () => void;
}

export function ChangeProficiencyDialog({
  open,
  employeeId,
  skillId,
  skillName,
  currentLevel,
  onClose,
}: ChangeProficiencyDialogProps) {
  const [level, setLevel] = useState<ProficiencyLevel>(currentLevel);
  const { mutate, isPending } = useChangeProficiencyMutation();

  function handleSubmit() {
    mutate(
      { employeeId, skillId, proficiencyLevel: level },
      { onSuccess: onClose },
    );
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Change Proficiency — {skillName}</DialogTitle>
      <DialogContent sx={{ pt: 2 }}>
        <FormControl fullWidth>
          <InputLabel id="level-label">Proficiency Level</InputLabel>
          <Select
            labelId="level-label"
            label="Proficiency Level"
            value={level}
            onChange={(e) => setLevel(e.target.value as ProficiencyLevel)}
          >
            {PROFICIENCY_LEVELS.map((l) => (
              <MenuItem key={l} value={l}>
                {l}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isPending}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={isPending || level === currentLevel}
          loading={isPending}
        >
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
}
