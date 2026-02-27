import { DepartmentRequiredSkillsSection } from "@/components/admin/DepartmentRequiredSkillsSection";
import { useCreateDepartmentMutation } from "@/hooks/useCreateDepartmentMutation";
import { useDeleteDepartmentMutation } from "@/hooks/useDeleteDepartmentMutation";
import { useDepartmentsQuery } from "@/hooks/useDepartmentsQuery";
import { useRenameDepartmentMutation } from "@/hooks/useRenameDepartmentMutation";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import {
  Alert,
  Box,
  Breadcrumbs,
  Button,
  Card,
  CardContent,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
  Skeleton,
  Snackbar,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import Link from "@mui/material/Link";
import { useState } from "react";
import { Link as RouterLink } from "react-router-dom";

export function AdminDepartmentsPage() {
  const { data: departments = [], isLoading, isError } = useDepartmentsQuery();
  const createMutation = useCreateDepartmentMutation();
  const renameMutation = useRenameDepartmentMutation();
  const deleteMutation = useDeleteDepartmentMutation();

  // Add dialog
  const [addOpen, setAddOpen] = useState(false);
  const [newName, setNewName] = useState("");
  const [addError, setAddError] = useState("");

  // Rename dialog
  const [renameTarget, setRenameTarget] = useState<{
    id: string;
    name: string;
  } | null>(null);
  const [renameName, setRenameName] = useState("");
  const [renameError, setRenameError] = useState("");

  // Delete confirm dialog
  const [deleteTarget, setDeleteTarget] = useState<{
    id: string;
    name: string;
  } | null>(null);

  // Snackbar
  const [snackMsg, setSnackMsg] = useState<string | null>(null);

  // --- Add ---
  async function handleAdd() {
    if (!newName.trim()) {
      setAddError("Department name is required.");
      return;
    }
    try {
      await createMutation.mutateAsync(newName.trim());
      setSnackMsg(`Department "${newName.trim()}" created.`);
      setAddOpen(false);
      setNewName("");
      setAddError("");
    } catch {
      setAddError("Failed to create department. It may already exist.");
    }
  }

  // --- Rename ---
  function openRename(id: string, name: string) {
    setRenameTarget({ id, name });
    setRenameName(name);
    setRenameError("");
  }

  async function handleRename() {
    if (!renameName.trim()) {
      setRenameError("Name is required.");
      return;
    }
    if (!renameTarget) return;
    try {
      await renameMutation.mutateAsync({
        id: renameTarget.id,
        name: renameName.trim(),
      });
      setSnackMsg(`Department renamed to "${renameName.trim()}".`);
      setRenameTarget(null);
    } catch {
      setRenameError("Failed to rename. Name may already exist.");
    }
  }

  // --- Delete ---
  async function handleDelete() {
    if (!deleteTarget) return;
    try {
      await deleteMutation.mutateAsync(deleteTarget.id);
      setSnackMsg(`Department "${deleteTarget.name}" deleted.`);
      setDeleteTarget(null);
    } catch {
      setSnackMsg("Cannot delete a department that has employees.");
      setDeleteTarget(null);
    }
  }

  return (
    <Box sx={{ p: 4, maxWidth: 800, mx: "auto" }}>
      {/* Breadcrumb */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link
          component={RouterLink}
          to="/admin"
          underline="hover"
          color="inherit"
        >
          Admin
        </Link>
        <Typography color="text.primary">Departments</Typography>
      </Breadcrumbs>

      {/* Header */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          mb: 3,
        }}
      >
        <Typography variant="h4">Departments</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => {
            setNewName("");
            setAddError("");
            setAddOpen(true);
          }}
        >
          Add Department
        </Button>
      </Box>

      {/* Loading */}
      {isLoading && (
        <Box>
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} variant="rounded" height={72} sx={{ mb: 1 }} />
          ))}
        </Box>
      )}

      {/* Error */}
      {isError && <Alert severity="error">Failed to load departments.</Alert>}

      {/* List */}
      {!isLoading &&
        !isError &&
        (departments.length === 0 ? (
          <Typography color="text.secondary">
            No departments yet. Click "Add Department" to create one.
          </Typography>
        ) : (
          departments.map((dept) => (
            <Card key={dept.id} sx={{ mb: 2 }}>
              <CardContent>
                <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                  <Box sx={{ flexGrow: 1 }}>
                    <Typography variant="h6">{dept.name}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      {dept.employeeCount} employee
                      {dept.employeeCount !== 1 ? "s" : ""}
                      {" · "}
                      {dept.requiredSkillCount} required skill
                      {dept.requiredSkillCount !== 1 ? "s" : ""}
                    </Typography>
                  </Box>
                  <Tooltip title="Rename">
                    <IconButton
                      onClick={() => openRename(dept.id, dept.name)}
                      aria-label="Rename department"
                    >
                      <EditIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip
                    title={
                      dept.employeeCount > 0
                        ? "Cannot delete — has employees"
                        : "Delete"
                    }
                  >
                    <span>
                      <IconButton
                        color="error"
                        onClick={() =>
                          setDeleteTarget({ id: dept.id, name: dept.name })
                        }
                        disabled={dept.employeeCount > 0}
                        aria-label="Delete department"
                      >
                        <DeleteIcon />
                      </IconButton>
                    </span>
                  </Tooltip>
                </Box>
                <DepartmentRequiredSkillsSection departmentId={dept.id} />
              </CardContent>
            </Card>
          ))
        ))}

      {/* Add Dialog */}
      <Dialog
        open={addOpen}
        onClose={() => setAddOpen(false)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Add Department</DialogTitle>
        <DialogContent>
          <TextField
            label="Department name"
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            error={!!addError}
            helperText={addError}
            fullWidth
            autoFocus
            sx={{ mt: 1 }}
            onKeyDown={(e) => e.key === "Enter" && handleAdd()}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAddOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleAdd}
            disabled={createMutation.isPending}
          >
            Create
          </Button>
        </DialogActions>
      </Dialog>

      {/* Rename Dialog */}
      <Dialog
        open={!!renameTarget}
        onClose={() => setRenameTarget(null)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Rename Department</DialogTitle>
        <DialogContent>
          <TextField
            label="New name"
            value={renameName}
            onChange={(e) => setRenameName(e.target.value)}
            error={!!renameError}
            helperText={renameError}
            fullWidth
            autoFocus
            sx={{ mt: 1 }}
            onKeyDown={(e) => e.key === "Enter" && handleRename()}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRenameTarget(null)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleRename}
            disabled={renameMutation.isPending}
          >
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirm */}
      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Department</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete{" "}
            <strong>{deleteTarget?.name}</strong>? This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button
            color="error"
            variant="contained"
            onClick={handleDelete}
            disabled={deleteMutation.isPending}
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar */}
      <Snackbar
        open={!!snackMsg}
        autoHideDuration={4000}
        onClose={() => setSnackMsg(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          severity="info"
          onClose={() => setSnackMsg(null)}
          sx={{ width: "100%" }}
        >
          {snackMsg}
        </Alert>
      </Snackbar>
    </Box>
  );
}
