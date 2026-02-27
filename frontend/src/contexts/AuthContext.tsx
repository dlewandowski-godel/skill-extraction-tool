import { setAccessToken, setRefreshFunction } from "@/lib/api-client";
import { authClient } from "@/lib/auth-client";
import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

export interface AuthUser {
  id: string;
  email: string;
  role: string;
}

interface LoginResponse {
  accessToken: string;
  expiresIn: number;
  role: string;
}

export interface AuthContextValue {
  user: AuthUser | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  role: string | null;
  /** True while the initial silent-refresh on app load is in progress. */
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (
    email: string,
    password: string,
    firstName: string,
    lastName: string,
  ) => Promise<void>;
  logout: () => Promise<void>;
}

// ---------------------------------------------------------------------------
// Context
// ---------------------------------------------------------------------------

const AuthContext = createContext<AuthContextValue | null>(null);

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function decodeJwtPayload(token: string): Record<string, unknown> {
  // JWT payload is base64url; replace chars, then atob
  const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
  return JSON.parse(atob(base64)) as Record<string, unknown>;
}

// ---------------------------------------------------------------------------
// Provider
// ---------------------------------------------------------------------------

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [accessToken, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const refreshTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const clearAuth = useCallback(() => {
    setUser(null);
    setToken(null);
    setAccessToken(null);
    if (refreshTimerRef.current) {
      clearTimeout(refreshTimerRef.current);
      refreshTimerRef.current = null;
    }
  }, []);

  const scheduleRefresh = useCallback(
    (expiresIn: number, refreshFn: () => void) => {
      if (refreshTimerRef.current) clearTimeout(refreshTimerRef.current);
      // Fire ~1 minute before expiry (minimum 10 s to avoid immediate re-fire)
      const delayMs = Math.max((expiresIn - 60) * 1000, 10_000);
      refreshTimerRef.current = setTimeout(refreshFn, delayMs);
    },
    [],
  );

  // Forward declaration ref — lets scheduleRefresh reference silentRefresh
  // without circular hook dependency
  const silentRefreshRef = useRef<() => Promise<string | null>>(() =>
    Promise.resolve(null),
  );

  const applyTokenResponse = useCallback(
    (data: LoginResponse) => {
      const payload = decodeJwtPayload(data.accessToken);
      const authUser: AuthUser = {
        id: payload.sub as string,
        email: payload.email as string,
        role: data.role,
      };
      setUser(authUser);
      setToken(data.accessToken);
      setAccessToken(data.accessToken);
      scheduleRefresh(data.expiresIn, () => silentRefreshRef.current());
    },
    [scheduleRefresh],
  );

  const silentRefresh = useCallback(async (): Promise<string | null> => {
    try {
      const { data } =
        await authClient.post<LoginResponse>("/api/auth/refresh");
      applyTokenResponse(data);
      return data.accessToken;
    } catch {
      clearAuth();
      return null;
    }
  }, [applyTokenResponse, clearAuth]);

  // Keep the ref in sync so the timer always calls the latest version
  useEffect(() => {
    silentRefreshRef.current = silentRefresh;
  }, [silentRefresh]);

  // Register silentRefresh with api-client so the 401 interceptor can use it
  useEffect(() => {
    setRefreshFunction(silentRefresh);
  }, [silentRefresh]);

  // On app load — restore session from the HTTP-only refresh cookie
  useEffect(() => {
    silentRefresh().finally(() => setIsLoading(false));
    // Only run on mount — intentionally omit `silentRefresh` from deps
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const { data } = await authClient.post<LoginResponse>("/api/auth/login", {
        email,
        password,
      });
      applyTokenResponse(data);
    },
    [applyTokenResponse],
  );

  const register = useCallback(
    async (
      email: string,
      password: string,
      firstName: string,
      lastName: string,
    ) => {
      const { data } = await authClient.post<LoginResponse>(
        "/api/auth/register",
        {
          email,
          password,
          firstName,
          lastName,
        },
      );
      applyTokenResponse(data);
    },
    [applyTokenResponse],
  );

  const logout = useCallback(async () => {
    try {
      await authClient.post("/api/auth/logout");
    } catch {
      // Ignore — clear local state regardless
    }
    clearAuth();
  }, [clearAuth]);

  const value: AuthContextValue = {
    user,
    accessToken,
    isAuthenticated: !!accessToken,
    role: user?.role ?? null,
    isLoading,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// ---------------------------------------------------------------------------
// Hook
// ---------------------------------------------------------------------------

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within <AuthProvider>");
  return ctx;
}
