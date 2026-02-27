import { type DocumentDto, type DocumentStatus } from "@/lib/documents-client";
import {
  Box,
  Chip,
  Divider,
  List,
  ListItem,
  ListItemText,
  Tooltip,
  Typography,
  type ChipProps,
} from "@mui/material";

function statusChipProps(status: DocumentStatus): {
  color: ChipProps["color"];
  label: string;
} {
  switch (status) {
    case "Pending":
      return { color: "default", label: "Pending" };
    case "Processing":
      return { color: "primary", label: "Processing" };
    case "Done":
      return { color: "success", label: "Done" };
    case "Failed":
      return { color: "error", label: "Failed" };
  }
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleString(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  });
}

interface DocumentStatusListProps {
  documents: DocumentDto[];
}

export function DocumentStatusList({ documents }: DocumentStatusListProps) {
  if (documents.length === 0) {
    return (
      <Typography color="text.secondary" data-testid="no-documents">
        No documents uploaded yet.
      </Typography>
    );
  }

  return (
    <List disablePadding data-testid="document-list">
      {documents.map((doc, i) => {
        const chip = statusChipProps(doc.status);
        return (
          <Box key={doc.documentId}>
            {i > 0 && <Divider component="li" />}
            <ListItem
              alignItems="flex-start"
              data-testid={`document-item-${doc.documentId}`}
              sx={{ px: 0 }}
            >
              <ListItemText
                primary={doc.fileName}
                secondary={`${doc.documentType} · Uploaded ${formatDate(doc.uploadedAt)}`}
              />
              <Tooltip
                title={doc.errorMessage ?? ""}
                disableHoverListener={doc.status !== "Failed"}
                disableFocusListener={doc.status !== "Failed"}
                arrow
              >
                <Chip
                  label={chip.label}
                  color={chip.color}
                  size="small"
                  sx={{ ml: 2, alignSelf: "center" }}
                  data-testid={`status-chip-${doc.documentId}`}
                />
              </Tooltip>
            </ListItem>
          </Box>
        );
      })}
    </List>
  );
}
