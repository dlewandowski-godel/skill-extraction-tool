import { useAuth } from "@/contexts/AuthContext";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Container,
  Grid,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function RegisterPage() {
  const { register, isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [firstNameError, setFirstNameError] = useState("");
  const [lastNameError, setLastNameError] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [confirmPasswordError, setConfirmPasswordError] = useState("");
  const [apiError, setApiError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Redirect already-authenticated users
  if (!isLoading && isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  const validate = (): boolean => {
    let valid = true;
    setFirstNameError("");
    setLastNameError("");
    setEmailError("");
    setPasswordError("");
    setConfirmPasswordError("");

    if (!firstName.trim()) {
      setFirstNameError("First name is required");
      valid = false;
    }
    if (!lastName.trim()) {
      setLastNameError("Last name is required");
      valid = false;
    }
    if (!email.trim()) {
      setEmailError("Email is required");
      valid = false;
    } else if (!EMAIL_REGEX.test(email)) {
      setEmailError("Enter a valid email address");
      valid = false;
    }
    if (!password) {
      setPasswordError("Password is required");
      valid = false;
    } else if (password.length < 6) {
      setPasswordError("Password must be at least 6 characters");
      valid = false;
    }
    if (!confirmPassword) {
      setConfirmPasswordError("Please confirm your password");
      valid = false;
    } else if (password !== confirmPassword) {
      setConfirmPasswordError("Passwords do not match");
      valid = false;
    }

    return valid;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    setApiError("");
    setIsSubmitting(true);
    try {
      await register(email, password, firstName, lastName);
      navigate("/dashboard", { replace: true });
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data
          ?.message ?? "Registration failed. Please try again.";
      setApiError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          mt: 8,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
        }}
      >
        <Paper elevation={3} sx={{ p: 4, width: "100%" }}>
          <Typography
            variant="h5"
            component="h1"
            gutterBottom
            textAlign="center"
          >
            Create Account
          </Typography>

          {apiError && (
            <Alert severity="error" sx={{ mb: 2 }} role="alert">
              {apiError}
            </Alert>
          )}

          <Box component="form" onSubmit={handleSubmit} noValidate>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <TextField
                  label="First Name"
                  fullWidth
                  margin="normal"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  error={!!firstNameError}
                  helperText={firstNameError}
                  autoComplete="given-name"
                  inputProps={{ "data-testid": "first-name-input" }}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  label="Last Name"
                  fullWidth
                  margin="normal"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  error={!!lastNameError}
                  helperText={lastNameError}
                  autoComplete="family-name"
                  inputProps={{ "data-testid": "last-name-input" }}
                />
              </Grid>
            </Grid>

            <TextField
              label="Email"
              type="email"
              fullWidth
              margin="normal"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              error={!!emailError}
              helperText={emailError}
              autoComplete="email"
              inputProps={{ "data-testid": "email-input" }}
            />
            <TextField
              label="Password"
              type="password"
              fullWidth
              margin="normal"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              error={!!passwordError}
              helperText={passwordError}
              autoComplete="new-password"
              inputProps={{ "data-testid": "password-input" }}
            />
            <TextField
              label="Confirm Password"
              type="password"
              fullWidth
              margin="normal"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              error={!!confirmPasswordError}
              helperText={confirmPasswordError}
              autoComplete="new-password"
              inputProps={{ "data-testid": "confirm-password-input" }}
            />

            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 2 }}
              disabled={isSubmitting}
              data-testid="submit-button"
            >
              {isSubmitting ? (
                <CircularProgress size={24} color="inherit" />
              ) : (
                "Sign Up"
              )}
            </Button>
          </Box>

          <Box sx={{ mt: 2, textAlign: "center" }}>
            <Typography variant="body2">
              Already have an account?{" "}
              <Link to="/login" style={{ fontWeight: 600 }}>
                Sign In
              </Link>
            </Typography>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
