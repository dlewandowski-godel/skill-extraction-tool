import { act, renderHook, waitFor } from "@testing-library/react";
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";

// --- Mocks (must be declared before imports that use them) -----------------

vi.mock("@/lib/auth-client", () => ({
  authClient: { post: vi.fn() },
}));

vi.mock("@/lib/api-client", () => ({
  setAccessToken: vi.fn(),
  setRefreshFunction: vi.fn(),
  getAccessToken: vi.fn(),
}));

// --- Subjects under test ----------------------------------------------------

import { AuthProvider, useAuth } from "@/contexts/AuthContext";
import { setAccessToken } from "@/lib/api-client";
import { authClient } from "@/lib/auth-client";

const mockPost = vi.mocked(authClient.post);
const mockSetAccessToken = vi.mocked(setAccessToken);

// --- Helpers ----------------------------------------------------------------

/** Build a base64url-encoded fake JWT (not cryptographically signed) */
function fakeToken(sub = "abc-123", email = "user@test.com") {
  const payload = btoa(JSON.stringify({ sub, email }))
    .replace(/=/g, "")
    .replace(/\+/g, "-")
    .replace(/\//g, "_");
  return `eyJhbGciOiJIUzI1NiJ9.${payload}.fakesig`;
}

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <AuthProvider>{children}</AuthProvider>
);

// --- Tests ------------------------------------------------------------------

describe("AuthContext", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("isLoading starts true then becomes false after mount refresh", async () => {
    mockPost.mockRejectedValueOnce(new Error("no cookie"));

    const { result } = renderHook(() => useAuth(), { wrapper });

    // Loading initially
    expect(result.current.isLoading).toBe(true);

    await waitFor(() => expect(result.current.isLoading).toBe(false));
    expect(result.current.isAuthenticated).toBe(false);
  });

  it("restores session when silent refresh succeeds on mount", async () => {
    mockPost.mockResolvedValueOnce({
      data: { accessToken: fakeToken(), expiresIn: 900, role: "User" },
    });

    const { result } = renderHook(() => useAuth(), { wrapper });

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(result.current.isAuthenticated).toBe(true);
    expect(result.current.role).toBe("User");
    expect(result.current.user?.email).toBe("user@test.com");
    expect(mockSetAccessToken).toHaveBeenCalledWith(expect.any(String));
  });

  it("login sets user and role on success", async () => {
    // mount refresh fails
    mockPost.mockRejectedValueOnce(new Error("no cookie"));

    const { result } = renderHook(() => useAuth(), { wrapper });
    await waitFor(() => expect(result.current.isLoading).toBe(false));

    // login succeeds
    mockPost.mockResolvedValueOnce({
      data: {
        accessToken: fakeToken("admin-id", "admin@test.com"),
        expiresIn: 900,
        role: "Admin",
      },
    });

    await act(async () => {
      await result.current.login("admin@test.com", "secret");
    });

    expect(result.current.isAuthenticated).toBe(true);
    expect(result.current.role).toBe("Admin");
    expect(result.current.user?.email).toBe("admin@test.com");
    expect(mockPost).toHaveBeenCalledWith("/api/auth/login", {
      email: "admin@test.com",
      password: "secret",
    });
  });

  it("logout clears auth state", async () => {
    // mount refresh succeeds
    mockPost.mockResolvedValueOnce({
      data: { accessToken: fakeToken(), expiresIn: 900, role: "User" },
    });

    const { result } = renderHook(() => useAuth(), { wrapper });
    await waitFor(() => expect(result.current.isAuthenticated).toBe(true));

    // logout
    mockPost.mockResolvedValueOnce({});

    await act(async () => {
      await result.current.logout();
    });

    expect(result.current.isAuthenticated).toBe(false);
    expect(result.current.user).toBeNull();
    expect(result.current.role).toBeNull();
    expect(mockSetAccessToken).toHaveBeenLastCalledWith(null);
  });

  it("register sets user and role on success", async () => {
    // mount refresh fails (no existing session)
    mockPost.mockRejectedValueOnce(new Error("no cookie"));

    const { result } = renderHook(() => useAuth(), { wrapper });
    await waitFor(() => expect(result.current.isLoading).toBe(false));
    expect(result.current.isAuthenticated).toBe(false);

    // register succeeds
    mockPost.mockResolvedValueOnce({
      data: {
        accessToken: fakeToken("new-id", "jane@test.com"),
        expiresIn: 900,
        role: "User",
      },
    });

    await act(async () => {
      await result.current.register(
        "jane@test.com",
        "Password1!",
        "Jane",
        "Doe",
      );
    });

    expect(result.current.isAuthenticated).toBe(true);
    expect(result.current.role).toBe("User");
    expect(result.current.user?.email).toBe("jane@test.com");
    expect(mockSetAccessToken).toHaveBeenCalledWith(expect.any(String));
    expect(mockPost).toHaveBeenCalledWith("/api/auth/register", {
      email: "jane@test.com",
      password: "Password1!",
      firstName: "Jane",
      lastName: "Doe",
    });
  });
});
