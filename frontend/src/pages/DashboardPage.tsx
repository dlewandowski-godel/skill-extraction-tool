import { useAuth } from "@/contexts/AuthContext";
import { Alert, Box, Typography } from "@mui/material";
import { useLocation } from "react-router-dom";

interface LocationState {
  message?: string;
}

export function DashboardPage() {
  const { user } = useAuth();
  const location = useLocation();
  const message = (location.state as LocationState)?.message;

  return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>

      {message && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          {message}
        </Alert>
      )}

      {user && (
        <Typography variant="body1" color="text.secondary">
          Welcome, {user.email}
        </Typography>
      )}
    </Box>
  );
}
