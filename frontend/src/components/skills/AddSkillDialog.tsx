import { useAddSkillMutation } from "@/hooks/useAddSkillMutation";
import { useSkillsTaxonomyQuery } from "@/hooks/useSkillsTaxonomyQuery";
import type { TaxonomySkillDto } from "@/lib/skills-client";
import {
  Autocomplete,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import { useState } from "react";

const PROFICIENCY_LEVELS = [
  "Beginner",
  "Intermediate",
  "Advanced",
  "Expert",
] as const;
type ProficiencyLevel = (typeof PROFICIENCY_LEVELS)[number];

interface AddSkillDialogProps {
  open: boolean;
  employeeId: string;
  onClose: () => void;
}

export function AddSkillDialog({
  open,
  employeeId,
  onClose,
}: AddSkillDialogProps) {
  const { data: skills = [], isLoading: isLoadingSkills } =
    useSkillsTaxonomyQuery();
  const { mutate, isPending } = useAddSkillMutation();

  const [selectedSkill, setSelectedSkill] = useState<TaxonomySkillDto | null>(
    null,
  );
  const [proficiency, setProficiency] =
    useState<ProficiencyLevel>("Intermediate");

  function handleSubmit() {
    if (!selectedSkill) return;
    mutate(
      {
        employeeId,
        skillId: selectedSkill.skillId,
        proficiencyLevel: proficiency,
      },
      {
        onSuccess: () => {
          handleClose();
        },
      },
    );
  }

  function handleClose() {
    setSelectedSkill(null);
    setProficiency("Intermediate");
    onClose();
  }

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Skill</DialogTitle>
      <DialogContent
        sx={{ display: "flex", flexDirection: "column", gap: 3, pt: 2 }}
      >
        <Autocomplete<TaxonomySkillDto>
          options={skills}
          loading={isLoadingSkills}
          getOptionLabel={(o) => `${o.name} (${o.category})`}
          groupBy={(o) => o.category}
          value={selectedSkill}
          onChange={(_e, val) => setSelectedSkill(val)}
          renderInput={(params) => (
            <TextField
              {...params}
              label="Skill"
              required
              InputProps={{
                ...params.InputProps,
                endAdornment: (
                  <>
                    {isLoadingSkills ? <CircularProgress size={18} /> : null}
                    {params.InputProps.endAdornment}
                  </>
                ),
              }}
            />
          )}
        />

        <FormControl fullWidth required>
          <InputLabel id="proficiency-label">Proficiency Level</InputLabel>
          <Select
            labelId="proficiency-label"
            label="Proficiency Level"
            value={proficiency}
            onChange={(e) => setProficiency(e.target.value as ProficiencyLevel)}
          >
            {PROFICIENCY_LEVELS.map((level) => (
              <MenuItem key={level} value={level}>
                {level}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={isPending}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={!selectedSkill || isPending}
          loading={isPending}
        >
          Add Skill
        </Button>
      </DialogActions>
    </Dialog>
  );
}
