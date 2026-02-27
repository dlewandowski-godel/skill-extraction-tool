import { useActivateTaxonomySkillMutation } from "@/hooks/useActivateTaxonomySkillMutation";
import { useAddTaxonomySkillMutation } from "@/hooks/useAddTaxonomySkillMutation";
import { useAdminTaxonomyQuery } from "@/hooks/useAdminTaxonomyQuery";
import { useDeactivateTaxonomySkillMutation } from "@/hooks/useDeactivateTaxonomySkillMutation";
import { useUpdateTaxonomySkillMutation } from "@/hooks/useUpdateTaxonomySkillMutation";
import type { TaxonomyAdminSkillDto } from "@/lib/taxonomy-client";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import PlayArrowIcon from "@mui/icons-material/PlayArrow";
import SearchIcon from "@mui/icons-material/Search";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Box,
  Breadcrumbs,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
  InputAdornment,
  Skeleton,
  Snackbar,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import Link from "@mui/material/Link";
import { useMemo, useState } from "react";
import { Link as RouterLink } from "react-router-dom";

type SkillDialogMode = "add" | "edit";

interface SkillFormState {
  name: string;
  category: string;
  aliasInput: string;
  aliases: string[];
}

const emptyForm = (): SkillFormState => ({
  name: "",
  category: "",
  aliasInput: "",
  aliases: [],
});

export function AdminTaxonomyPage() {
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");

  const {
    data: skills = [],
    isLoading,
    isError,
  } = useAdminTaxonomyQuery(search || undefined, categoryFilter || undefined);

  const addMutation = useAddTaxonomySkillMutation();
  const updateMutation = useUpdateTaxonomySkillMutation();
  const deactivateMutation = useDeactivateTaxonomySkillMutation();
  const activateMutation = useActivateTaxonomySkillMutation();

  // Dialog state
  const [dialogMode, setDialogMode] = useState<SkillDialogMode>("add");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingSkillId, setEditingSkillId] = useState<string | null>(null);
  const [form, setForm] = useState<SkillFormState>(emptyForm);
  const [formError, setFormError] = useState("");

  // Deactivate confirm
  const [confirmTarget, setConfirmTarget] =
    useState<TaxonomyAdminSkillDto | null>(null);

  // Snackbar
  const [snackbar, setSnackbar] = useState<{ open: boolean; message: string }>({
    open: false,
    message: "",
  });

  const showSnackbar = (message: string) =>
    setSnackbar({ open: true, message });

  // Group by category
  const grouped = useMemo(() => {
    const map = new Map<string, TaxonomyAdminSkillDto[]>();
    for (const s of skills) {
      const arr = map.get(s.category) ?? [];
      arr.push(s);
      map.set(s.category, arr);
    }
    return Array.from(map.entries()).sort(([a], [b]) => a.localeCompare(b));
  }, [skills]);

  // Distinct categories for filter chips (unfiltered — always show all)
  const allCategories = useMemo(
    () => Array.from(new Set(skills.map((s) => s.category))).sort(),
    [skills],
  );

  const openAdd = () => {
    setDialogMode("add");
    setEditingSkillId(null);
    setForm(emptyForm);
    setFormError("");
    setDialogOpen(true);
  };

  const openEdit = (skill: TaxonomyAdminSkillDto) => {
    setDialogMode("edit");
    setEditingSkillId(skill.id);
    setForm({
      name: skill.name,
      category: skill.category,
      aliasInput: "",
      aliases: [...skill.aliases],
    });
    setFormError("");
    setDialogOpen(true);
  };

  const addAlias = () => {
    const trimmed = form.aliasInput.trim();
    if (!trimmed) return;
    if (!form.aliases.includes(trimmed)) {
      setForm((f) => ({
        ...f,
        aliases: [...f.aliases, trimmed],
        aliasInput: "",
      }));
    } else {
      setForm((f) => ({ ...f, aliasInput: "" }));
    }
  };

  const removeAlias = (alias: string) =>
    setForm((f) => ({ ...f, aliases: f.aliases.filter((a) => a !== alias) }));

  const handleDialogSubmit = async () => {
    if (!form.name.trim() || !form.category.trim()) {
      setFormError("Name and category are required.");
      return;
    }
    setFormError("");

    try {
      if (dialogMode === "add") {
        await addMutation.mutateAsync({
          name: form.name.trim(),
          category: form.category.trim(),
          aliases: form.aliases,
        });
        showSnackbar("Skill added.");
      } else {
        await updateMutation.mutateAsync({
          id: editingSkillId!,
          payload: {
            name: form.name.trim(),
            category: form.category.trim(),
            aliases: form.aliases,
          },
        });
        showSnackbar("Skill updated.");
      }
      setDialogOpen(false);
    } catch {
      setFormError("A skill with this name already exists in this category.");
    }
  };

  const handleDeactivateConfirm = async () => {
    if (!confirmTarget) return;
    try {
      await deactivateMutation.mutateAsync(confirmTarget.id);
      showSnackbar(`"${confirmTarget.name}" deactivated.`);
    } catch {
      showSnackbar("Failed to deactivate skill.");
    }
    setConfirmTarget(null);
  };

  const handleActivate = async (skill: TaxonomyAdminSkillDto) => {
    try {
      await activateMutation.mutateAsync(skill.id);
      showSnackbar(`"${skill.name}" activated.`);
    } catch {
      showSnackbar("Failed to activate skill.");
    }
  };

  return (
    <Box p={3}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link
          component={RouterLink}
          to="/admin/employees"
          underline="hover"
          color="inherit"
        >
          Admin
        </Link>
        <Typography color="text.primary">Skill Taxonomy</Typography>
      </Breadcrumbs>

      <Stack
        direction="row"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h5" fontWeight="bold">
          Skill Taxonomy
        </Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAdd}>
          Add Skill
        </Button>
      </Stack>

      {/* Search */}
      <TextField
        placeholder="Search skills..."
        size="small"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        sx={{ mb: 2, width: 360 }}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon fontSize="small" />
            </InputAdornment>
          ),
        }}
      />

      {/* Category filter chips */}
      {allCategories.length > 0 && (
        <Stack direction="row" spacing={1} mb={2} flexWrap="wrap">
          <Chip
            label="All"
            onClick={() => setCategoryFilter("")}
            color={categoryFilter === "" ? "primary" : "default"}
            variant={categoryFilter === "" ? "filled" : "outlined"}
          />
          {allCategories.map((cat) => (
            <Chip
              key={cat}
              label={cat}
              onClick={() =>
                setCategoryFilter(cat === categoryFilter ? "" : cat)
              }
              color={categoryFilter === cat ? "primary" : "default"}
              variant={categoryFilter === cat ? "filled" : "outlined"}
            />
          ))}
        </Stack>
      )}

      {isLoading && <Skeleton variant="rectangular" height={120} />}
      {isError && <Alert severity="error">Failed to load taxonomy.</Alert>}

      {!isLoading && !isError && grouped.length === 0 && (
        <Typography color="text.secondary">No skills found.</Typography>
      )}

      {grouped.map(([category, categorySkills]) => (
        <Accordion key={category} defaultExpanded>
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Typography fontWeight="bold">
              {category}{" "}
              <Typography
                component="span"
                color="text.secondary"
                fontWeight="normal"
              >
                ({categorySkills.length})
              </Typography>
            </Typography>
          </AccordionSummary>
          <AccordionDetails sx={{ p: 0 }}>
            {categorySkills.map((skill) => (
              <Box
                key={skill.id}
                display="flex"
                alignItems="center"
                px={2}
                py={1}
                borderBottom="1px solid"
                borderColor="divider"
                sx={{ opacity: skill.isActive ? 1 : 0.5 }}
              >
                <Box flex={1}>
                  <Stack direction="row" spacing={1} alignItems="center">
                    <Typography fontWeight="medium">{skill.name}</Typography>
                    {!skill.isActive && (
                      <Chip label="Inactive" size="small" color="default" />
                    )}
                  </Stack>
                  {skill.aliases.length > 0 && (
                    <Stack
                      direction="row"
                      spacing={0.5}
                      mt={0.5}
                      flexWrap="wrap"
                    >
                      {skill.aliases.map((a) => (
                        <Chip
                          key={a}
                          label={a}
                          size="small"
                          variant="outlined"
                        />
                      ))}
                    </Stack>
                  )}
                </Box>
                <Stack direction="row">
                  <Tooltip title="Edit">
                    <IconButton size="small" onClick={() => openEdit(skill)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  {skill.isActive ? (
                    <Tooltip title="Deactivate">
                      <IconButton
                        size="small"
                        onClick={() => setConfirmTarget(skill)}
                        color="error"
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  ) : (
                    <Tooltip title="Re-activate">
                      <IconButton
                        size="small"
                        onClick={() => handleActivate(skill)}
                        color="success"
                      >
                        <PlayArrowIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  )}
                </Stack>
              </Box>
            ))}
          </AccordionDetails>
        </Accordion>
      ))}

      {/* Add / Edit dialog */}
      <Dialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle>
          {dialogMode === "add" ? "Add Skill" : "Edit Skill"}
        </DialogTitle>
        <DialogContent>
          <Stack spacing={2} mt={1}>
            <TextField
              label="Name"
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              fullWidth
              size="small"
            />
            <TextField
              label="Category"
              value={form.category}
              onChange={(e) =>
                setForm((f) => ({ ...f, category: e.target.value }))
              }
              fullWidth
              size="small"
            />
            {/* Aliases */}
            <Stack direction="row" spacing={1}>
              <TextField
                label="Add alias"
                value={form.aliasInput}
                onChange={(e) =>
                  setForm((f) => ({ ...f, aliasInput: e.target.value }))
                }
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    addAlias();
                  }
                }}
                size="small"
                sx={{ flex: 1 }}
              />
              <Button variant="outlined" size="small" onClick={addAlias}>
                Add
              </Button>
            </Stack>
            {form.aliases.length > 0 && (
              <Stack direction="row" spacing={0.5} flexWrap="wrap">
                {form.aliases.map((a) => (
                  <Chip
                    key={a}
                    label={a}
                    onDelete={() => removeAlias(a)}
                    size="small"
                  />
                ))}
              </Stack>
            )}
            {formError && <Alert severity="error">{formError}</Alert>}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button
            onClick={handleDialogSubmit}
            variant="contained"
            disabled={addMutation.isPending || updateMutation.isPending}
          >
            {dialogMode === "add" ? "Add" : "Save"}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Deactivate confirm dialog */}
      <Dialog open={!!confirmTarget} onClose={() => setConfirmTarget(null)}>
        <DialogTitle>Deactivate Skill</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to deactivate{" "}
            <strong>{confirmTarget?.name}</strong>? It will no longer appear in
            extraction results.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmTarget(null)}>Cancel</Button>
          <Button
            onClick={handleDeactivateConfirm}
            color="error"
            variant="contained"
            disabled={deactivateMutation.isPending}
          >
            Deactivate
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={3000}
        onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
        message={snackbar.message}
      />
    </Box>
  );
}
