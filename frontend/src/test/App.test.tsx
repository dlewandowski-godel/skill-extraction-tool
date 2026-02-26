import { render, screen } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ThemeProvider } from '@mui/material'
import { theme } from '@/theme'
import App from '../App'

function renderWithProviders(ui: React.ReactElement) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  })
  return render(
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <ThemeProvider theme={theme}>{ui}</ThemeProvider>
      </BrowserRouter>
    </QueryClientProvider>,
  )
}

describe('App', () => {
  it('renders without crashing', () => {
    renderWithProviders(<App />)
    expect(screen.getByText('Skill Extraction Tool')).toBeInTheDocument()
  })
})
