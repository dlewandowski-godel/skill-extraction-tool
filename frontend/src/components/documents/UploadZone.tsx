import { MY_DOCUMENTS_QUERY_KEY } from "@/hooks/useDocumentStatusQuery";
import { uploadDocument, type DocumentType } from "@/lib/documents-client";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import InsertDriveFileIcon from "@mui/icons-material/InsertDriveFile";
import {
  Alert,
  Box,
  Button,
  LinearProgress,
  Paper,
  Typography,
} from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRef, useState } from "react";

const MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function validateFile(file: File): string {
  if (file.size === 0) return "Uploaded file is empty";
  if (file.type !== "application/pdf") return "Only PDF files are accepted";
  if (file.size > MAX_FILE_SIZE) return "File size must not exceed 10 MB";
  return "";
}

interface UploadZoneProps {
  documentType: DocumentType;
  label: string;
}

export function UploadZone({ documentType, label }: UploadZoneProps) {
  const typeKey = documentType.toLowerCase(); // 'cv' | 'ifu'
  const queryClient = useQueryClient();

  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [validationError, setValidationError] = useState("");
  const [progress, setProgress] = useState<number | null>(null);

  const { mutate, isPending, isSuccess, reset } = useMutation({
    mutationFn: (file: File) =>
      uploadDocument(file, documentType, (pct) => setProgress(pct)),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: MY_DOCUMENTS_QUERY_KEY });
    },
    onSettled: () => {
      setProgress(null);
    },
  });

  const handleFileSelect = (file: File) => {
    const error = validateFile(file);
    if (error) {
      setValidationError(error);
      setSelectedFile(null);
      return;
    }
    setValidationError("");
    setSelectedFile(file);
    reset();
  };

  const handleUpload = () => {
    if (selectedFile) mutate(selectedFile);
  };

  const handleReset = () => {
    setSelectedFile(null);
    setValidationError("");
    reset();
  };

  // Drag-and-drop handlers
  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    const file = e.dataTransfer.files[0];
    if (file) handleFileSelect(file);
  };

  return (
    <Box>
      <Typography variant="subtitle1" gutterBottom fontWeight={600}>
        {label}
      </Typography>

      <Paper
        data-testid={`${typeKey}-upload-zone`}
        variant="outlined"
        onDrop={handleDrop}
        onDragOver={(e) => {
          e.preventDefault();
          setIsDragging(true);
        }}
        onDragLeave={() => setIsDragging(false)}
        onClick={() => {
          if (!selectedFile && !isSuccess) fileInputRef.current?.click();
        }}
        sx={{
          p: 3,
          textAlign: "center",
          cursor: selectedFile || isSuccess ? "default" : "pointer",
          borderStyle: "dashed",
          borderColor: isDragging ? "primary.main" : "divider",
          bgcolor: isDragging ? "action.hover" : "background.paper",
          transition: "border-color 0.2s, background-color 0.2s",
        }}
      >
        <input
          ref={fileInputRef}
          type="file"
          accept="application/pdf"
          hidden
          data-testid={`${typeKey}-file-input`}
          onChange={(e) => {
            const file = e.target.files?.[0];
            if (file) handleFileSelect(file);
            // Reset so the same file can be re-selected
            e.target.value = "";
          }}
        />

        {isSuccess ? (
          // ── Success state ──────────────────────────────────────────────────
          <Box data-testid={`${typeKey}-success`}>
            <CheckCircleIcon color="success" sx={{ fontSize: 48 }} />
            <Typography
              variant="body1"
              color="success.main"
              fontWeight={600}
              mt={1}
            >
              Uploaded successfully!
            </Typography>
            <Button
              size="small"
              sx={{ mt: 1 }}
              onClick={(e) => {
                e.stopPropagation();
                handleReset();
              }}
            >
              Upload another
            </Button>
          </Box>
        ) : selectedFile ? (
          // ── File selected ──────────────────────────────────────────────────
          <Box data-testid={`${typeKey}-file-info`}>
            <InsertDriveFileIcon color="primary" sx={{ fontSize: 48 }} />
            <Typography
              fontWeight={500}
              noWrap
              sx={{ maxWidth: 280, mx: "auto" }}
            >
              {selectedFile.name}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              {formatFileSize(selectedFile.size)}
            </Typography>

            {isPending && (
              <LinearProgress
                data-testid={`${typeKey}-progress`}
                variant={progress !== null ? "determinate" : "indeterminate"}
                value={progress ?? undefined}
                sx={{ mt: 2, borderRadius: 1 }}
              />
            )}

            {!isPending && (
              <Box
                sx={{
                  mt: 2,
                  display: "flex",
                  gap: 1,
                  justifyContent: "center",
                }}
              >
                <Button
                  variant="contained"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleUpload();
                  }}
                  data-testid={`${typeKey}-upload-button`}
                >
                  Upload
                </Button>
                <Button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleReset();
                  }}
                  data-testid={`${typeKey}-remove-button`}
                >
                  Remove
                </Button>
              </Box>
            )}
          </Box>
        ) : (
          // ── Empty / drop state ─────────────────────────────────────────────
          <Box>
            <CloudUploadIcon sx={{ fontSize: 48, color: "text.secondary" }} />
            <Typography color="text.secondary" mt={1}>
              Drag &amp; drop or click to select a PDF (max 10&nbsp;MB)
            </Typography>
            <Button
              variant="outlined"
              size="small"
              sx={{ mt: 1.5 }}
              onClick={(e) => {
                e.stopPropagation();
                fileInputRef.current?.click();
              }}
            >
              Browse file
            </Button>
          </Box>
        )}
      </Paper>

      {validationError && (
        <Alert
          severity="error"
          sx={{ mt: 1 }}
          data-testid={`${typeKey}-validation-error`}
        >
          {validationError}
        </Alert>
      )}
    </Box>
  );
}
