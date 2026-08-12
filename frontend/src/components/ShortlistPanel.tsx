import { useRemoveFromShortlist, useShortlist } from '../api/hooks'

export function ShortlistPanel() {
  const shortlist = useShortlist()
  const removeFromShortlist = useRemoveFromShortlist()

  if (shortlist.isLoading) {
    return <p>Loading shortlist…</p>
  }

  if (shortlist.isError) {
    return <p role="alert">Could not load your shortlist.</p>
  }

  const items = shortlist.data!

  if (items.length === 0) {
    return <p>Your shortlist is empty. Add a protein from its detail view.</p>
  }

  return (
    <ul>
      {items.map((item) => (
        <li key={item.accession}>
          {item.geneName} ({item.accession}) &middot; {item.proteinName}{' '}
          <button
            type="button"
            onClick={() => removeFromShortlist.mutate(item.accession)}
            disabled={removeFromShortlist.isPending}
          >
            Remove
          </button>
        </li>
      ))}
    </ul>
  )
}
