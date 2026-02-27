import { useRemoveSkillMutation } from "@/hooks/useRemoveSkillMutation";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";

interface RemoveSkillDialogProps {
  open: boolean;
  employeeId: string;
  skillId: string;
  skillName: string;
  employeeName: string;
  onClose: () => void;
}

export function RemoveSkillDialog({
  open,
  employeeId,
  skillId,
  skillName,
  employeeName,
  onClose,
}: RemoveSkillDialogProps) {
  const { mutate, isPending } = useRemoveSkillMutation();

  function handleConfirm() {
    mutate({ employeeId, skillId }, { onSuccess: onClose });
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Remove Skill</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Remove <strong>{skillName}</strong> from {employeeName}&apos;s
          profile? This action cannot be undone.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isPending}>
          Cancel
        </Button>
        <Button
          variant="contained"
          color="error"
          onClick={handleConfirm}
          disabled={isPending}
          loading={isPending}
        >
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
}
