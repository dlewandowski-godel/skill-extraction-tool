import { ProficiencyChip } from "@/components/skills/ProficiencyChip";
import { useMyProfileQuery } from "@/hooks/useMyProfileQuery";
import type { SkillDto } from "@/lib/profile-client";
import {
  Box,
  Button,
  Card,
  CardContent,
  Skeleton,
  Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";

function groupByCategory(skills: SkillDto[]): Map<string, SkillDto[]> {
  const map = new Map<string, SkillDto[]>();
  for (const skill of skills) {
    const list = map.get(skill.category) ?? [];
    list.push(skill);
    map.set(skill.category, list);
  }
  return map;
}

export function ProfilePage() {
  const { data, isLoading, isError } = useMyProfileQuery();
  const navigate = useNavigate();

  if (isLoading) {
    return (
      <Box sx={{ p: 4 }}>
        <Skeleton variant="text" width={300} height={48} />
        <Skeleton variant="text" width={180} sx={{ mb: 3 }} />
        {[1, 2, 3].map((i) => (
          <Skeleton key={i} variant="rounded" height={120} sx={{ mb: 2 }} />
        ))}
      </Box>
    );
  }

  if (isError || !data) {
    return (
      <Box sx={{ p: 4 }}>
        <Typography color="error">Failed to load profile.</Typography>
      </Box>
    );
  }

  const grouped = groupByCategory(data.skills);

  return (
    <Box sx={{ p: 4, maxWidth: 900, mx: "auto" }}>
      <Typography variant="h4" gutterBottom>
        {data.fullName}
      </Typography>
      {data.department && (
        <Typography variant="subtitle1" color="text.secondary" sx={{ mb: 3 }}>
          {data.department}
        </Typography>
      )}

      {data.skills.length === 0 ? (
        <Box sx={{ textAlign: "center", mt: 8 }}>
          <Typography variant="h6" gutterBottom>
            No skills extracted yet
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Upload your CV or an IFU document to get started.
          </Typography>
          <Button variant="contained" onClick={() => navigate("/upload")}>
            Upload a Document
          </Button>
        </Box>
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
                </Box>
              ))}
            </CardContent>
          </Card>
        ))
      )}
    </Box>
  );
}
