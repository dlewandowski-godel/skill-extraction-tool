import axios, { type InternalAxiosRequestConfig } from "axios";

let accessToken: string | null = null;

// Registered by AuthContext so the 401 interceptor can trigger a silent refresh
let refreshFn: (() => Promise<string | null>) | null = null;

// Queue of requests waiting for the in-flight refresh to complete
let isRefreshing = false;
let pendingQueue: Array<(token: string | null) => void> = [];

export const setAccessToken = (token: string | null) => {
  accessToken = token;
};

export const getAccessToken = () => accessToken;

/** Call this from AuthContext to register the silent-refresh function. */
export const setRefreshFunction = (fn: () => Promise<string | null>) => {
  refreshFn = fn;
};

export const apiClient = axios.create({
  // Empty baseURL → relative URLs (/api/...) so the Vite proxy (dev) or
  // nginx proxy (production) routes requests to the backend.
  baseURL: import.meta.env.VITE_API_URL || "",
  headers: {
    "Content-Type": "application/json",
  },
});

// Attach JWT from in-memory store on every request
apiClient.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

// Handle 401 — retry once after a silent refresh, then give up
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config as InternalAxiosRequestConfig & {
      _retried?: boolean;
    };

    if (error.response?.status !== 401 || original._retried) {
      return Promise.reject(error);
    }

    original._retried = true;

    if (!refreshFn) {
      setAccessToken(null);
      return Promise.reject(error);
    }

    if (isRefreshing) {
      // Another request is already refreshing — queue this one
      return new Promise<unknown>((resolve, reject) => {
        pendingQueue.push((token) => {
          if (token) {
            original.headers.Authorization = `Bearer ${token}`;
            resolve(apiClient(original));
          } else {
            reject(error);
          }
        });
      });
    }

    isRefreshing = true;
    try {
      const newToken = await refreshFn();
      pendingQueue.forEach((cb) => cb(newToken));
      pendingQueue = [];
      if (newToken) {
        original.headers.Authorization = `Bearer ${newToken}`;
        return apiClient(original);
      }
    } catch {
      pendingQueue.forEach((cb) => cb(null));
      pendingQueue = [];
      setAccessToken(null);
    } finally {
      isRefreshing = false;
    }

    return Promise.reject(error);
  },
);
