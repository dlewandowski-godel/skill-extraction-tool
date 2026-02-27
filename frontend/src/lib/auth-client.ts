import axios from "axios";

/**
 * Separate Axios instance for authentication endpoints.
 * Sends the HTTP-only refresh cookie via `withCredentials: true`.
 * Does NOT attach the JWT Bearer token — avoids an infinite loop
 * where the 401 retry interceptor on `apiClient` would call itself.
 */
export const authClient = axios.create({
  // Empty baseURL → relative URLs (/api/...) so the Vite proxy (dev) or
  // nginx proxy (production) routes requests to the backend.
  baseURL: import.meta.env.VITE_API_URL || "",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});
