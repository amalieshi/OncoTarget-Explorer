import { useState } from 'react'
import { SearchBox } from './components/SearchBox'
import { ResultsTable } from './components/ResultsTable'
import { DetailPanel } from './components/DetailPanel'
import { ShortlistPanel } from './components/ShortlistPanel'
import { useSearchProteins } from './api/hooks'
import './App.css'

function App() {
  const [query, setQuery] = useState('')
  const [selectedAccession, setSelectedAccession] = useState<string | null>(null)

  const search = useSearchProteins(query)

  function handleSearch(nextQuery: string) {
    setQuery(nextQuery)
    setSelectedAccession(null)
  }

  return (
    <div className="app">
      <h1>OncoTarget Explorer</h1>
      <SearchBox onSearch={handleSearch} />

      <section style={{ marginTop: 24 }}>
        {search.isLoading && <p>Searching…</p>}
        {search.isError && <p role="alert">Search failed. Please try again.</p>}
        {search.data && (
          <ResultsTable
            results={search.data}
            selectedAccession={selectedAccession}
            onSelect={setSelectedAccession}
          />
        )}
      </section>

      <section style={{ marginTop: 24 }}>
        <DetailPanel accession={selectedAccession} />
      </section>

      <section style={{ marginTop: 24 }}>
        <h2>Shortlist</h2>
        <ShortlistPanel />
      </section>
    </div>
  )
}

export default App
