import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { ResultsTable } from './ResultsTable'
import type { ProteinSummary } from '../api/types'

const sampleResults: ProteinSummary[] = [
  { accession: 'P04626', geneName: 'ERBB2', proteinName: 'Receptor tyrosine-protein kinase erbB-2', organism: 'Homo sapiens' },
]

describe('ResultsTable', () => {
  it('shows an empty state when there are no results', () => {
    render(<ResultsTable results={[]} selectedAccession={null} onSelect={vi.fn()} />)

    expect(screen.getByText(/no results found/i)).toBeInTheDocument()
  })

  it('renders each result and calls onSelect when a row is clicked', async () => {
    const user = userEvent.setup()
    const onSelect = vi.fn()

    render(<ResultsTable results={sampleResults} selectedAccession={null} onSelect={onSelect} />)

    expect(screen.getByText('ERBB2')).toBeInTheDocument()
    await user.click(screen.getByText('P04626'))

    expect(onSelect).toHaveBeenCalledWith('P04626')
  })
})
