import { useDepartmentsQuery } from "@/hooks/useDepartmentsQuery";
import { useEditEmployeeMutation } from "@/hooks/useEditEmployeeMutation";
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
  Stack,
  TextField,
} from "@mui/material";
import { useEffect, useState } from "react";

interface Props {
  open: boolean;
  employeeId: string;
  initialFirstName: string;
  initialLastName: string;
  initialDepartmentId: string | null;
  initialRole: string;
  callerId: string;
  onClose: () => void;
  onSuccess: () => void;
}

export function EditEmployeeDialog({
  open,
  employeeId,
  initialFirstName,
  initialLastName,
  initialDepartmentId,
  initialRole,
  callerId,
  onClose,
  onSuccess,
}: Props) {
  const [firstName, setFirstName] = useState(initialFirstName);
  const [lastName, setLastName] = useState(initialLastName);
  const [role, setRole] = useState(initialRole);
  const [departmentId, setDepartmentId] = useState(initialDepartmentId ?? "");
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (open) {
      setFirstName(initialFirstName);
      setLastName(initialLastName);
      setRole(initialRole);
      setDepartmentId(initialDepartmentId ?? "");
      setErrors({});
    }
  }, [
    open,
    initialFirstName,
    initialLastName,
    initialRole,
    initialDepartmentId,
  ]);

  const { data: departments = [] } = useDepartmentsQuery();
  const editMutation = useEditEmployeeMutation();

  function validate() {
    const next: Record<string, string> = {};
    if (!firstName.trim()) next.firstName = "First name is required.";
    if (!lastName.trim()) next.lastName = "Last name is required.";
    if (!role) next.role = "Role is required.";
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit() {
    if (!validate()) return;
    await editMutation.mutateAsync({
      employeeId,
      request: {
        firstName,
        lastName,
        role,
        departmentId: departmentId || null,
      },
    });
    onSuccess();
    onClose();
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit Employee</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label="First Name"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            error={!!errors.firstName}
            helperText={errors.firstName}
            fullWidth
            required
          />
          <TextField
            label="Last Name"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            error={!!errors.lastName}
            helperText={errors.lastName}
            fullWidth
            required
          />
          <FormControl fullWidth required error={!!errors.role}>
            <InputLabel>Role</InputLabel>
            <Select
              value={role}
              label="Role"
              onChange={(e) => setRole(e.target.value)}
              disabled={employeeId === callerId}
            >
              <MenuItem value="Employee">Employee</MenuItem>
              <MenuItem value="Admin">Admin</MenuItem>
            </Select>
          </FormControl>
          <FormControl fullWidth>
            <InputLabel>Department</InputLabel>
            <Select
              value={departmentId}
              label="Department"
              onChange={(e) => setDepartmentId(e.target.value)}
            >
              <MenuItem value="">
                <em>None</em>
              </MenuItem>
              {departments.map((d) => (
                <MenuItem key={d.id} value={d.id}>
                  {d.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={editMutation.isPending}
        >
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
}
