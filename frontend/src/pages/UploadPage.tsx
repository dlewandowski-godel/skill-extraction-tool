import { DocumentStatusList } from "@/components/documents/DocumentStatusList";
import { UploadZone } from "@/components/documents/UploadZone";
import { useDocumentStatusQuery } from "@/hooks/useDocumentStatusQuery";
import { Box, CircularProgress, Divider, Typography } from "@mui/material";

export function UploadPage() {
  const { data: documents, isLoading } = useDocumentStatusQuery();

  return (
    <Box sx={{ p: 4, maxWidth: 960, mx: "auto" }}>
      <Typography variant="h4" gutterBottom>
        Upload Documents
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
        Upload your CV and/or IFU (PDF, max 10&nbsp;MB each) for skill
        extraction. New uploads replace the previous active document of the same
        type.
      </Typography>

      <Box
        sx={{
          display: "flex",
          gap: 4,
          flexWrap: "wrap",
          "& > *": { flex: "1 1 280px", minWidth: 0 },
        }}
      >
        <UploadZone documentType="CV" label="CV (Curriculum Vitae)" />
        <UploadZone documentType="IFU" label="IFU (Instructions for Use)" />
      </Box>

      <Divider sx={{ my: 4 }} />

      <Typography variant="h6" gutterBottom>
        My Documents
      </Typography>

      {isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <DocumentStatusList documents={documents ?? []} />
      )}
    </Box>
  );
}
