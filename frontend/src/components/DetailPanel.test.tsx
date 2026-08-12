import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { describe, expect, it, vi } from 'vitest'
import { DetailPanel } from './DetailPanel'
import { apiClient } from '../api/client'
import type { ProteinDetail, ShortlistItem } from '../api/types'

vi.mock('../api/client', () => ({
  apiClient: { get: vi.fn(), post: vi.fn(), delete: vi.fn() },
}))

function renderWithClient(ui: React.ReactElement) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>)
}

const sampleDetail: ProteinDetail = {
  accession: 'P04626',
  geneName: 'ERBB2',
  proteinName: 'Receptor tyrosine-protein kinase erbB-2',
  organism: 'Homo sapiens',
  functionSummary: 'Protein tyrosine kinase.',
  subcellularLocations: ['Cell membrane'],
  sequenceLength: 1255,
  diseaseAssociations: ['Gastric cancer'],
  crossReferences: [{ database: 'HGNC', id: 'HGNC:3430' }],
}

function mockGet(shortlist: ShortlistItem[]) {
  vi.mocked(apiClient.get).mockImplementation((path: string) => {
    if (path.startsWith('/api/shortlist')) return Promise.resolve(shortlist)
    return Promise.resolve(sampleDetail)
  })
}

describe('DetailPanel', () => {
  it('prompts for a selection when no accession is chosen', () => {
    renderWithClient(<DetailPanel accession={null} />)

    expect(screen.getByText(/select a protein/i)).toBeInTheDocument()
  })

  it('renders protein details once the request resolves', async () => {
    mockGet([])

    renderWithClient(<DetailPanel accession="P04626" />)

    expect(await screen.findByText('Protein tyrosine kinase.')).toBeInTheDocument()
    expect(screen.getByText('Cell membrane')).toBeInTheDocument()
    expect(screen.getByText('Gastric cancer')).toBeInTheDocument()
    expect(screen.getByText('HGNC: HGNC:3430')).toBeInTheDocument()
  })

  it('shows an error state when the request fails', async () => {
    vi.mocked(apiClient.get).mockRejectedValueOnce(new Error('not found'))

    renderWithClient(<DetailPanel accession="UNKNOWN" />)

    expect(await screen.findByRole('alert')).toHaveTextContent(/could not load details/i)
  })

  it('adds the protein to the shortlist when not already on it', async () => {
    const user = userEvent.setup()
    mockGet([])
    vi.mocked(apiClient.post).mockResolvedValueOnce({
      accession: 'P04626',
      geneName: 'ERBB2',
      proteinName: 'Receptor tyrosine-protein kinase erbB-2',
      addedAtUtc: '2026-01-01T00:00:00Z',
    })

    renderWithClient(<DetailPanel accession="P04626" />)

    const addButton = await screen.findByRole('button', { name: /add to shortlist/i })
    await user.click(addButton)

    expect(apiClient.post).toHaveBeenCalledWith('/api/shortlist', {
      accession: 'P04626',
      geneName: 'ERBB2',
      proteinName: 'Receptor tyrosine-protein kinase erbB-2',
    })
  })

  it('shows a remove button when the protein is already shortlisted', async () => {
    mockGet([
      { accession: 'P04626', geneName: 'ERBB2', proteinName: 'Receptor tyrosine-protein kinase erbB-2', addedAtUtc: '2026-01-01T00:00:00Z' },
    ])

    renderWithClient(<DetailPanel accession="P04626" />)

    expect(await screen.findByRole('button', { name: /remove from shortlist/i })).toBeInTheDocument()
  })
})
