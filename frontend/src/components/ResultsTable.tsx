import type { ProteinSummary } from '../api/types'

interface ResultsTableProps {
  results: ProteinSummary[]
  selectedAccession: string | null
  onSelect: (accession: string) => void
}

export function ResultsTable({ results, selectedAccession, onSelect }: ResultsTableProps) {
  if (results.length === 0) {
    return <p>No results found.</p>
  }

  return (
    <table>
      <thead>
        <tr>
          <th>Gene</th>
          <th>Protein</th>
          <th>Organism</th>
          <th>Accession</th>
        </tr>
      </thead>
      <tbody>
        {results.map((result) => (
          <tr
            key={result.accession}
            onClick={() => onSelect(result.accession)}
            aria-selected={result.accession === selectedAccession}
            style={{ cursor: 'pointer' }}
          >
            <td>{result.geneName}</td>
            <td>{result.proteinName}</td>
            <td>{result.organism}</td>
            <td>{result.accession}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
