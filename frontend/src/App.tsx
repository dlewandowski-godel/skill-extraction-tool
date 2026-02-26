import { Typography, Container, Box } from '@mui/material'

function App() {
  return (
    <Container maxWidth="lg">
      <Box sx={{ mt: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Skill Extraction Tool
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Welcome to the Skill Extraction Tool.
        </Typography>
      </Box>
    </Container>
  )
}

export default App
