import { CreateEmployeeDialog } from "@/components/admin/CreateEmployeeDialog";
import { useDepartmentsQuery } from "@/hooks/useDepartmentsQuery";
import { useEmployeesQuery } from "@/hooks/useEmployeesQuery";
import AddIcon from "@mui/icons-material/Add";
import {
  Alert,
  Box,
  Breadcrumbs,
  Button,
  Chip,
  FormControl,
  InputLabel,
  MenuItem,
  Pagination,
  Select,
  Skeleton,
  Snackbar,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import Link from "@mui/material/Link";
import { useState } from "react";
import { Link as RouterLink, useNavigate } from "react-router-dom";

const PAGE_SIZE = 20;

export function AdminEmployeeListPage() {
  const navigate = useNavigate();

  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [department, setDepartment] = useState("");
  const [searchInput, setSearchInput] = useState("");

  const [createOpen, setCreateOpen] = useState(false);
  const [snackbarMsg, setSnackbarMsg] = useState<string | null>(null);

  const { data: departments = [] } = useDepartmentsQuery();

  const { data, isLoading, isError } = useEmployeesQuery({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    department: department || undefined,
  });

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1;

  function handleSearchSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSearch(searchInput);
    setPage(1);
  }

  function handleDepartmentChange(value: string) {
    setDepartment(value);
    setPage(1);
  }

  function handleCreated(tempPassword: string) {
    setSnackbarMsg(
      `Employee account created. Temporary password: ${tempPassword}`,
    );
  }

  return (
    <Box sx={{ p: 4, maxWidth: 1200, mx: "auto" }}>
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
        <Typography color="text.primary">Employees</Typography>
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
        <Typography variant="h4">Employees</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => setCreateOpen(true)}
        >
          New Employee
        </Button>
      </Box>

      {/* Filters */}
      <Box
        component="form"
        onSubmit={handleSearchSubmit}
        sx={{ display: "flex", gap: 2, mb: 3, flexWrap: "wrap" }}
      >
        <TextField
          label="Search"
          size="small"
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          placeholder="Name or email…"
          sx={{ minWidth: 240 }}
        />
        <Button type="submit" variant="outlined" size="small">
          Search
        </Button>
        <FormControl size="small" sx={{ minWidth: 200 }}>
          <InputLabel>Department</InputLabel>
          <Select
            value={department}
            label="Department"
            onChange={(e) => handleDepartmentChange(e.target.value)}
          >
            <MenuItem value="">
              <em>All departments</em>
            </MenuItem>
            {departments.map((d) => (
              <MenuItem key={d.id} value={d.name}>
                {d.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Box>

      {/* Table */}
      {isLoading ? (
        <Box>
          {[1, 2, 3, 4, 5].map((i) => (
            <Skeleton key={i} variant="rounded" height={52} sx={{ mb: 1 }} />
          ))}
        </Box>
      ) : isError ? (
        <Alert severity="error">Failed to load employees.</Alert>
      ) : (
        <>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Department</TableCell>
                <TableCell>Role</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Last Upload</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data?.items.map((emp) => (
                <TableRow
                  key={emp.id}
                  hover
                  sx={{ cursor: "pointer" }}
                  onClick={() => navigate(`/admin/employees/${emp.id}`)}
                >
                  <TableCell>
                    {emp.firstName} {emp.lastName}
                  </TableCell>
                  <TableCell>{emp.email}</TableCell>
                  <TableCell>{emp.departmentName ?? "—"}</TableCell>
                  <TableCell>{emp.role}</TableCell>
                  <TableCell>
                    <Chip
                      label={emp.isActive ? "Active" : "Inactive"}
                      color={emp.isActive ? "success" : "default"}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    {emp.lastUploadDate
                      ? new Date(emp.lastUploadDate).toLocaleDateString()
                      : "—"}
                  </TableCell>
                </TableRow>
              ))}
              {data?.items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={6} align="center">
                    <Typography color="text.secondary" py={2}>
                      No employees found.
                    </Typography>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
          {totalPages > 1 && (
            <Box sx={{ display: "flex", justifyContent: "center", mt: 3 }}>
              <Pagination
                count={totalPages}
                page={page}
                onChange={(_e, v) => setPage(v)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}

      {/* Create Employee Dialog */}
      <CreateEmployeeDialog
        open={createOpen}
        onClose={() => setCreateOpen(false)}
        onCreated={handleCreated}
      />

      {/* Temp password snackbar */}
      <Snackbar
        open={!!snackbarMsg}
        autoHideDuration={12000}
        onClose={() => setSnackbarMsg(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          severity="success"
          onClose={() => setSnackbarMsg(null)}
          sx={{ width: "100%" }}
        >
          {snackbarMsg}
        </Alert>
      </Snackbar>
    </Box>
  );
}
