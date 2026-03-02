import { Alert, Box, Collapse } from "@mui/material";
import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

export type NotificationSeverity = "success" | "error" | "warning" | "info";

export interface Notification {
  id: string;
  message: string;
  severity: NotificationSeverity;
  /** Auto-dismiss delay in ms, or `false` for no auto-dismiss. */
  autoDismiss: number | false;
}

export interface NotifyFunctions {
  success: (message: string) => void;
  error: (message: string) => void;
  warning: (message: string) => void;
}

// ---------------------------------------------------------------------------
// Context
// ---------------------------------------------------------------------------

const NotificationContext = createContext<NotifyFunctions | null>(null);

// ---------------------------------------------------------------------------
// Provider
// ---------------------------------------------------------------------------

const MAX_VISIBLE = 3;

function NotificationItem({
  notification,
  onDismiss,
}: {
  notification: Notification;
  onDismiss: (id: string) => void;
}) {
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (notification.autoDismiss !== false) {
      timerRef.current = setTimeout(() => {
        onDismiss(notification.id);
      }, notification.autoDismiss);
    }
    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, [notification.id, notification.autoDismiss, onDismiss]);

  return (
    <Collapse in unmountOnExit>
      <Alert
        severity={notification.severity}
        variant="filled"
        onClose={() => onDismiss(notification.id)}
        sx={{ minWidth: 300, maxWidth: 480, mb: 1 }}
        data-testid={`notification-${notification.severity}`}
      >
        {notification.message}
      </Alert>
    </Collapse>
  );
}

export function NotificationProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [queue, setQueue] = useState<Notification[]>([]);

  const remove = useCallback((id: string) => {
    setQueue((prev) => prev.filter((n) => n.id !== id));
  }, []);

  const add = useCallback(
    (
      severity: NotificationSeverity,
      message: string,
      autoDismiss: number | false,
    ) => {
      const notification: Notification = {
        id: crypto.randomUUID(),
        message,
        severity,
        autoDismiss,
      };
      setQueue((prev) => [...prev, notification]);
    },
    [],
  );

  const notify: NotifyFunctions = useMemo(
    () => ({
      success: (message) => add("success", message, 4000),
      error: (message) => add("error", message, false),
      warning: (message) => add("warning", message, 6000),
    }),
    [add],
  );

  const visible = queue.slice(0, MAX_VISIBLE);

  return (
    <NotificationContext.Provider value={notify}>
      {children}

      {/* Fixed notification stack — bottom-right corner */}
      <Box
        aria-live="polite"
        aria-label="notifications"
        sx={{
          position: "fixed",
          bottom: 24,
          right: 24,
          zIndex: 2000,
          display: "flex",
          flexDirection: "column-reverse",
          gap: 0,
        }}
        data-testid="notification-container"
      >
        {visible.map((n) => (
          <NotificationItem key={n.id} notification={n} onDismiss={remove} />
        ))}
      </Box>
    </NotificationContext.Provider>
  );
}

// ---------------------------------------------------------------------------
// Hook
// ---------------------------------------------------------------------------

export function useNotify(): NotifyFunctions {
  const ctx = useContext(NotificationContext);
  if (!ctx)
    throw new Error("useNotify must be used within <NotificationProvider>");
  return ctx;
}
