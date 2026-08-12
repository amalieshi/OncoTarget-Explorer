import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { describe, expect, it, vi } from 'vitest'
import { ShortlistPanel } from './ShortlistPanel'
import { apiClient } from '../api/client'
import type { ShortlistItem } from '../api/types'

vi.mock('../api/client', () => ({
  apiClient: { get: vi.fn(), post: vi.fn(), delete: vi.fn() },
}))

function renderWithClient(ui: React.ReactElement) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>)
}

const sampleItems: ShortlistItem[] = [
  { accession: 'P04626', geneName: 'ERBB2', proteinName: 'Receptor tyrosine-protein kinase erbB-2', addedAtUtc: '2026-01-01T00:00:00Z' },
]

describe('ShortlistPanel', () => {
  it('shows an empty state when the shortlist has no items', async () => {
    vi.mocked(apiClient.get).mockResolvedValueOnce([])

    renderWithClient(<ShortlistPanel />)

    expect(await screen.findByText(/shortlist is empty/i)).toBeInTheDocument()
  })

  it('renders shortlist items and removes one on click', async () => {
    const user = userEvent.setup()
    vi.mocked(apiClient.get).mockResolvedValueOnce(sampleItems)
    vi.mocked(apiClient.delete).mockResolvedValueOnce(undefined)

    renderWithClient(<ShortlistPanel />)

    expect(await screen.findByText(/ERBB2/)).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: /remove/i }))

    expect(apiClient.delete).toHaveBeenCalledWith('/api/shortlist/P04626')
  })
})
