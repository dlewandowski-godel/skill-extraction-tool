import { EditEmployeeDialog } from "@/components/admin/EditEmployeeDialog";
import { AddSkillDialog } from "@/components/skills/AddSkillDialog";
import { ChangeProficiencyDialog } from "@/components/skills/ChangeProficiencyDialog";
import { ProficiencyChip } from "@/components/skills/ProficiencyChip";
import { RemoveSkillDialog } from "@/components/skills/RemoveSkillDialog";
import { useAuth } from "@/contexts/AuthContext";
import { useActivateEmployeeMutation } from "@/hooks/useActivateEmployeeMutation";
import { useDeactivateEmployeeMutation } from "@/hooks/useDeactivateEmployeeMutation";
import { useEmployeeProfileQuery } from "@/hooks/useEmployeeProfileQuery";
import type { ProficiencyLevel, SkillDto } from "@/lib/profile-client";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import {
  Alert,
  Box,
  Breadcrumbs,
  Card,
  CardContent,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
  Link,
  Skeleton,
  Snackbar,
  Tooltip,
  Typography,
} from "@mui/material";
import Button from "@mui/material/Button";
import { useState } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";

function groupByCategory(skills: SkillDto[]): Map<string, SkillDto[]> {
  const map = new Map<string, SkillDto[]>();
  for (const skill of skills) {
    const list = map.get(skill.category) ?? [];
    list.push(skill);
    map.set(skill.category, list);
  }
  return map;
}

interface SkillTarget {
  skillId: string;
  skillName: string;
  currentLevel: ProficiencyLevel;
}

export function AdminEmployeeProfilePage() {
  const { id = "" } = useParams<{ id: string }>();
  const { user: authUser } = useAuth();
  const { data, isLoading, isError } = useEmployeeProfileQuery(id);

  const deactivateMutation = useDeactivateEmployeeMutation();
  const activateMutation = useActivateEmployeeMutation();

  const [addOpen, setAddOpen] = useState(false);
  const [removeTarget, setRemoveTarget] = useState<SkillTarget | null>(null);
  const [editTarget, setEditTarget] = useState<SkillTarget | null>(null);
  const [editEmployeeOpen, setEditEmployeeOpen] = useState(false);
  const [deactivateConfirmOpen, setDeactivateConfirmOpen] = useState(false);
  const [activateConfirmOpen, setActivateConfirmOpen] = useState(false);
  const [snackMsg, setSnackMsg] = useState<string | null>(null);

  if (isLoading) {
    return (
      <Box sx={{ p: 4 }}>
        <Skeleton variant="text" width={200} sx={{ mb: 1 }} />
        <Skeleton variant="text" width={300} height={48} />
        <Skeleton variant="text" width={180} sx={{ mb: 3 }} />
        {[1, 2].map((i) => (
          <Skeleton key={i} variant="rounded" height={120} sx={{ mb: 2 }} />
        ))}
      </Box>
    );
  }

  if (isError || !data) {
    return (
      <Box sx={{ p: 4 }}>
        <Typography color="error">
          Employee not found or failed to load.
        </Typography>
      </Box>
    );
  }

  const grouped = groupByCategory(data.skills);

  return (
    <Box sx={{ p: 4, maxWidth: 960, mx: "auto" }}>
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
        <Link
          component={RouterLink}
          to="/admin/employees"
          underline="hover"
          color="inherit"
        >
          Employees
        </Link>
        <Typography color="text.primary">{data.fullName}</Typography>
        <Typography color="text.primary">Profile</Typography>
      </Breadcrumbs>

      {/* Header */}
      <Box
        sx={{
          display: "flex",
          alignItems: "flex-start",
          justifyContent: "space-between",
          mb: 3,
        }}
      >
        <Box>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <Typography variant="h4">{data.fullName}</Typography>
            {!data.isActive && (
              <Chip label="Inactive" color="default" size="small" />
            )}
          </Box>
          {data.department && (
            <Typography variant="subtitle1" color="text.secondary">
              {data.department}
            </Typography>
          )}
        </Box>
        <Box sx={{ display: "flex", gap: 1 }}>
          <Button
            variant="outlined"
            startIcon={<EditIcon />}
            onClick={() => setEditEmployeeOpen(true)}
          >
            Edit
          </Button>
          {data.isActive ? (
            <Button
              variant="outlined"
              color="error"
              onClick={() => setDeactivateConfirmOpen(true)}
              disabled={id === authUser?.id}
            >
              Deactivate
            </Button>
          ) : (
            <Button
              variant="outlined"
              color="success"
              onClick={() => setActivateConfirmOpen(true)}
            >
              Activate
            </Button>
          )}
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => setAddOpen(true)}
          >
            Add Skill
          </Button>
        </Box>
      </Box>

      {data.skills.length === 0 ? (
        <Typography color="text.secondary">
          No skills on record for this employee.
        </Typography>
      ) : (
        Array.from(grouped.entries()).map(([category, skills]) => (
          <Card key={category} sx={{ mb: 3 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                {category}
              </Typography>
              {skills.map((skill) => (
                <Box
                  key={skill.skillId}
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    gap: 1.5,
                    py: 0.75,
                    borderBottom: "1px solid",
                    borderColor: "divider",
                    "&:last-child": { borderBottom: "none" },
                  }}
                >
                  <Typography variant="body1" sx={{ flexGrow: 1 }}>
                    {skill.skillName}
                  </Typography>
                  <ProficiencyChip
                    level={skill.proficiencyLevel}
                    isOverride={skill.isManualOverride}
                  />
                  <Tooltip title="Change proficiency">
                    <IconButton
                      size="small"
                      aria-label="Change proficiency"
                      onClick={() =>
                        setEditTarget({
                          skillId: skill.skillId,
                          skillName: skill.skillName,
                          currentLevel: skill.proficiencyLevel,
                        })
                      }
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Remove skill">
                    <IconButton
                      size="small"
                      aria-label="Remove skill"
                      color="error"
                      onClick={() =>
                        setRemoveTarget({
                          skillId: skill.skillId,
                          skillName: skill.skillName,
                          currentLevel: skill.proficiencyLevel,
                        })
                      }
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </Box>
              ))}
            </CardContent>
          </Card>
        ))
      )}

      {/* Add Skill Dialog */}
      <AddSkillDialog
        open={addOpen}
        employeeId={id}
        onClose={() => setAddOpen(false)}
      />

      {/* Remove Skill Dialog */}
      {removeTarget && (
        <RemoveSkillDialog
          open={!!removeTarget}
          employeeId={id}
          skillId={removeTarget.skillId}
          skillName={removeTarget.skillName}
          employeeName={data.fullName}
          onClose={() => setRemoveTarget(null)}
        />
      )}

      {/* Change Proficiency Dialog */}
      {editTarget && (
        <ChangeProficiencyDialog
          open={!!editTarget}
          employeeId={id}
          skillId={editTarget.skillId}
          skillName={editTarget.skillName}
          currentLevel={editTarget.currentLevel}
          onClose={() => setEditTarget(null)}
        />
      )}

      {/* Edit Employee Dialog */}
      <EditEmployeeDialog
        open={editEmployeeOpen}
        employeeId={id}
        initialFirstName={data.firstName}
        initialLastName={data.lastName}
        initialDepartmentId={data.departmentId ?? null}
        initialRole={data.role ?? "Employee"}
        callerId={authUser?.id ?? ""}
        onClose={() => setEditEmployeeOpen(false)}
        onSuccess={() => setSnackMsg("Employee details updated.")}
      />

      {/* Deactivate Confirm */}
      <Dialog
        open={deactivateConfirmOpen}
        onClose={() => setDeactivateConfirmOpen(false)}
      >
        <DialogTitle>Deactivate Account</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to deactivate <strong>{data.fullName}</strong>
            ? They will no longer be able to log in.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeactivateConfirmOpen(false)}>
            Cancel
          </Button>
          <Button
            color="error"
            variant="contained"
            disabled={deactivateMutation.isPending}
            onClick={async () => {
              await deactivateMutation.mutateAsync(id);
              setDeactivateConfirmOpen(false);
              setSnackMsg(`${data.fullName} has been deactivated.`);
            }}
          >
            Deactivate
          </Button>
        </DialogActions>
      </Dialog>

      {/* Activate Confirm */}
      <Dialog
        open={activateConfirmOpen}
        onClose={() => setActivateConfirmOpen(false)}
      >
        <DialogTitle>Activate Account</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Re-activate <strong>{data.fullName}</strong>? They will be able to
            log in again.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setActivateConfirmOpen(false)}>Cancel</Button>
          <Button
            color="success"
            variant="contained"
            disabled={activateMutation.isPending}
            onClick={async () => {
              await activateMutation.mutateAsync(id);
              setActivateConfirmOpen(false);
              setSnackMsg(`${data.fullName} has been activated.`);
            }}
          >
            Activate
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
          severity="success"
          onClose={() => setSnackMsg(null)}
          sx={{ width: "100%" }}
        >
          {snackMsg}
        </Alert>
      </Snackbar>
    </Box>
  );
}
