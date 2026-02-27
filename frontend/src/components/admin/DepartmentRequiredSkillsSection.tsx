import { useAddRequiredSkillMutation } from "@/hooks/useAddRequiredSkillMutation";
import { useRemoveRequiredSkillMutation } from "@/hooks/useRemoveRequiredSkillMutation";
import { useRequiredSkillsQuery } from "@/hooks/useRequiredSkillsQuery";
import { useSkillsTaxonomyQuery } from "@/hooks/useSkillsTaxonomyQuery";
import DeleteIcon from "@mui/icons-material/Delete";
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import { useState } from "react";

interface Props {
  departmentId: string;
}

export function DepartmentRequiredSkillsSection({ departmentId }: Props) {
  const {
    data: requiredSkills = [],
    isLoading,
    isError,
  } = useRequiredSkillsQuery(departmentId);

  const { data: allSkills = [] } = useSkillsTaxonomyQuery();

  const addMutation = useAddRequiredSkillMutation(departmentId);
  const removeMutation = useRemoveRequiredSkillMutation(departmentId);

  const [addOpen, setAddOpen] = useState(false);
  const [selectedSkillId, setSelectedSkillId] = useState<string | null>(null);
  const [addError, setAddError] = useState("");

  const requiredIds = new Set(requiredSkills.map((s) => s.skillId));
  const availableSkills = allSkills.filter((s) => !requiredIds.has(s.skillId));

  const handleAdd = async () => {
    if (!selectedSkillId) {
      setAddError("Please select a skill.");
      return;
    }
    try {
      await addMutation.mutateAsync(selectedSkillId);
      setAddOpen(false);
      setSelectedSkillId(null);
      setAddError("");
    } catch {
      setAddError("Skill is already required or could not be added.");
    }
  };

  const handleRemove = async (skillId: string) => {
    await removeMutation.mutateAsync(skillId);
  };

  return (
    <Box mt={1} pt={1} borderTop="1px solid" borderColor="divider">
      <Stack
        direction="row"
        justifyContent="space-between"
        alignItems="center"
        mb={1}
      >
        <Typography variant="subtitle2" color="text.secondary">
          Required Skills
        </Typography>
        <Button
          size="small"
          onClick={() => {
            setSelectedSkillId(null);
            setAddError("");
            setAddOpen(true);
          }}
        >
          + Add
        </Button>
      </Stack>

      {isLoading && <CircularProgress size={16} />}
      {isError && (
        <Alert severity="error" sx={{ py: 0 }}>
          Failed to load.
        </Alert>
      )}

      {!isLoading && !isError && requiredSkills.length === 0 && (
        <Typography variant="body2" color="text.secondary">
          No required skills defined.
        </Typography>
      )}

      <Stack direction="row" spacing={0.5} flexWrap="wrap">
        {requiredSkills.map((skill) => (
          <Chip
            key={skill.skillId}
            label={`${skill.name} (${skill.category})`}
            size="small"
            onDelete={() => handleRemove(skill.skillId)}
            deleteIcon={
              <Tooltip title="Remove">
                <DeleteIcon />
              </Tooltip>
            }
          />
        ))}
      </Stack>

      {/* Add required skill dialog */}
      <Dialog
        open={addOpen}
        onClose={() => setAddOpen(false)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Add Required Skill</DialogTitle>
        <DialogContent>
          <Autocomplete
            options={availableSkills}
            getOptionLabel={(o) => `${o.name} (${o.category})`}
            onChange={(_, val) => setSelectedSkillId(val?.skillId ?? null)}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Select skill"
                error={!!addError}
                helperText={addError}
                sx={{ mt: 1 }}
              />
            )}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAddOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleAdd}
            disabled={addMutation.isPending}
          >
            Add
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
