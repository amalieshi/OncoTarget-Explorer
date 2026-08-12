import { useState, type FormEvent } from 'react'

interface SearchBoxProps {
  onSearch: (query: string) => void
}

export function SearchBox({ onSearch }: SearchBoxProps) {
  const [value, setValue] = useState('')

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    onSearch(value.trim())
  }

  return (
    <form onSubmit={handleSubmit} role="search">
      <label htmlFor="protein-search">Search by gene symbol or protein name</label>
      <div style={{ display: 'flex', gap: 8, marginTop: 8 }}>
        <input
          id="protein-search"
          type="text"
          value={value}
          onChange={(e) => setValue(e.target.value)}
          placeholder="e.g. HER2, ERBB2, TROP2"
        />
        <button type="submit">Search</button>
      </div>
    </form>
  )
}
