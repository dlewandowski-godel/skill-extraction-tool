import { useProficiencyDistributionQuery } from "@/hooks/useProficiencyDistributionQuery";
import { useSkillGapsQuery } from "@/hooks/useSkillGapsQuery";
import { useSkillsByDepartmentQuery } from "@/hooks/useSkillsByDepartmentQuery";
import { useTopSkillsQuery } from "@/hooks/useTopSkillsQuery";
import { useUploadActivityQuery } from "@/hooks/useUploadActivityQuery";
import type { DepartmentSkillsDto } from "@/lib/analytics-client";
import RefreshIcon from "@mui/icons-material/Refresh";
import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  CircularProgress,
  Grid,
  Typography,
} from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Line,
  LineChart,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

// ── constants ────────────────────────────────────────────────────────────────

const DEPT_COLORS = [
  "#1976d2",
  "#ed6c02",
  "#2e7d32",
  "#9c27b0",
  "#d32f2f",
  "#0288d1",
];

const PROFICIENCY_COLORS: Record<string, string> = {
  Beginner: "#9e9e9e",
  Intermediate: "#1976d2",
  Advanced: "#ed6c02",
  Expert: "#2e7d32",
};

// ── helpers ──────────────────────────────────────────────────────────────────

function pivotDepartmentData(data: DepartmentSkillsDto[]) {
  const skillTotals = new Map<string, number>();
  for (const dept of data) {
    for (const skill of dept.skills) {
      skillTotals.set(
        skill.name,
        (skillTotals.get(skill.name) ?? 0) + skill.count,
      );
    }
  }
  const top5 = [...skillTotals.entries()]
    .sort((a, b) => b[1] - a[1])
    .slice(0, 5)
    .map(([name]) => name);

  const rows = data.map((dept) => ({
    department: dept.department,
    ...Object.fromEntries(
      top5.map((s) => [s, dept.skills.find((sk) => sk.name === s)?.count ?? 0]),
    ),
  }));

  return { rows, skills: top5 };
}

// ── chart cards ──────────────────────────────────────────────────────────────

function ChartCard({
  title,
  loading,
  children,
}: {
  title: string;
  loading: boolean;
  children: React.ReactNode;
}) {
  return (
    <Card>
      <CardHeader title={title} />
      <CardContent>
        {loading ? (
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            minHeight={260}
          >
            <CircularProgress />
          </Box>
        ) : (
          children
        )}
      </CardContent>
    </Card>
  );
}

// ── individual charts ─────────────────────────────────────────────────────────

function TopSkillsChart() {
  const { data = [], isLoading } = useTopSkillsQuery(10);
  return (
    <ChartCard title="Top 10 Skills" loading={isLoading}>
      <ResponsiveContainer width="100%" height={320}>
        <BarChart
          layout="vertical"
          data={data}
          margin={{ top: 4, right: 24, bottom: 4, left: 8 }}
        >
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis type="number" allowDecimals={false} />
          <YAxis
            dataKey="skillName"
            type="category"
            width={130}
            tick={{ fontSize: 12 }}
          />
          <Tooltip />
          <Bar dataKey="employeeCount" name="Employees" fill="#1976d2" />
        </BarChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

function SkillsByDepartmentChart() {
  const { data = [], isLoading } = useSkillsByDepartmentQuery();
  const { rows, skills } = pivotDepartmentData(data);

  return (
    <ChartCard title="Skills per Department (Top 5 Skills)" loading={isLoading}>
      {data.length === 0 ? (
        <Typography color="text.secondary" sx={{ py: 4, textAlign: "center" }}>
          No department data yet. Departments are populated in Epic 7.
        </Typography>
      ) : (
        <ResponsiveContainer width="100%" height={320}>
          <BarChart
            data={rows}
            margin={{ top: 4, right: 24, bottom: 40, left: 8 }}
          >
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="department"
              tick={{ fontSize: 11 }}
              angle={-25}
              textAnchor="end"
            />
            <YAxis allowDecimals={false} />
            <Tooltip />
            <Legend />
            {skills.map((skill, i) => (
              <Bar
                key={skill}
                dataKey={skill}
                fill={DEPT_COLORS[i % DEPT_COLORS.length]}
              />
            ))}
          </BarChart>
        </ResponsiveContainer>
      )}
    </ChartCard>
  );
}

function SkillGapChart() {
  const { data = [], isLoading } = useSkillGapsQuery();

  return (
    <ChartCard title="Skill Gap Analysis" loading={isLoading}>
      {data.length === 0 ? (
        <Typography color="text.secondary" sx={{ py: 4, textAlign: "center" }}>
          No required skills configured yet. Configure them in the Taxonomy
          settings.
        </Typography>
      ) : (
        <ResponsiveContainer width="100%" height={320}>
          <BarChart
            layout="vertical"
            data={data}
            margin={{ top: 4, right: 40, bottom: 4, left: 8 }}
          >
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis type="number" domain={[0, 100]} unit="%" />
            <YAxis
              dataKey="skillName"
              type="category"
              width={130}
              tick={{ fontSize: 12 }}
            />
            <Tooltip formatter={(value: number) => [`${value}%`, "Gap"]} />
            <Bar dataKey="gapPercent" name="Gap %" radius={[0, 4, 4, 0]}>
              {data.map((entry, i) => (
                <Cell
                  key={`gap-${i}`}
                  fill={
                    entry.gapPercent >= 70
                      ? "#d32f2f"
                      : entry.gapPercent >= 30
                        ? "#ed6c02"
                        : "#2e7d32"
                  }
                />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      )}
    </ChartCard>
  );
}

function UploadActivityChart() {
  const { data = [], isLoading } = useUploadActivityQuery("30d");

  return (
    <ChartCard title="Upload Activity (Last 30 Days)" loading={isLoading}>
      <ResponsiveContainer width="100%" height={260}>
        <LineChart
          data={data}
          margin={{ top: 4, right: 24, bottom: 4, left: 8 }}
        >
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis
            dataKey="date"
            tick={{ fontSize: 10 }}
            tickFormatter={(v: string) => v.slice(5)} // show MM-DD
          />
          <YAxis allowDecimals={false} />
          <Tooltip />
          <Legend />
          <Line
            type="monotone"
            dataKey="cvCount"
            name="CVs"
            stroke="#1976d2"
            dot={false}
            strokeWidth={2}
          />
          <Line
            type="monotone"
            dataKey="ifuCount"
            name="IFUs"
            stroke="#2e7d32"
            dot={false}
            strokeWidth={2}
          />
        </LineChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

function ProficiencyDistributionChart() {
  const { data = [], isLoading } = useProficiencyDistributionQuery();

  return (
    <ChartCard title="Proficiency Distribution" loading={isLoading}>
      <ResponsiveContainer width="100%" height={260}>
        <PieChart>
          <Pie
            data={data}
            dataKey="count"
            nameKey="level"
            cx="50%"
            cy="50%"
            innerRadius={60}
            outerRadius={100}
          >
            {data.map((entry, i) => (
              <Cell
                key={`cell-${i}`}
                fill={PROFICIENCY_COLORS[entry.level] ?? "#9e9e9e"}
              />
            ))}
          </Pie>
          <Tooltip formatter={(value: number, name: string) => [value, name]} />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

// ── page ──────────────────────────────────────────────────────────────────────

export function AdminDashboardPage() {
  const queryClient = useQueryClient();

  function handleRefresh() {
    queryClient.invalidateQueries({ queryKey: ["analytics"] });
  }

  return (
    <Box sx={{ p: 4 }}>
      <Box
        display="flex"
        alignItems="center"
        justifyContent="space-between"
        mb={3}
      >
        <Typography variant="h4">Admin Dashboard</Typography>
        <Button
          variant="outlined"
          startIcon={<RefreshIcon />}
          onClick={handleRefresh}
        >
          Refresh
        </Button>
      </Box>

      <Grid container spacing={3}>
        {/* Row 1 */}
        <Grid size={{ xs: 12, md: 6 }}>
          <TopSkillsChart />
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <SkillsByDepartmentChart />
        </Grid>

        {/* Row 2 */}
        <Grid size={{ xs: 12, md: 6 }}>
          <SkillGapChart />
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <ProficiencyDistributionChart />
        </Grid>

        {/* Row 3: full width */}
        <Grid size={{ xs: 12 }}>
          <UploadActivityChart />
        </Grid>
      </Grid>
    </Box>
  );
}
