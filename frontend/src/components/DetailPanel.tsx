import { useProteinDetail, useShortlist, useAddToShortlist, useRemoveFromShortlist } from '../api/hooks'

interface DetailPanelProps {
  accession: string | null
}

export function DetailPanel({ accession }: DetailPanelProps) {
  const detail = useProteinDetail(accession)
  const shortlist = useShortlist()
  const addToShortlist = useAddToShortlist()
  const removeFromShortlist = useRemoveFromShortlist()

  if (accession === null) {
    return <p>Select a protein from the results to see its details.</p>
  }

  if (detail.isLoading) {
    return <p>Loading protein details…</p>
  }

  if (detail.isError) {
    return <p role="alert">Could not load details for {accession}.</p>
  }

  const protein = detail.data!
  const isShortlisted = shortlist.data?.some((item) => item.accession === protein.accession) ?? false

  return (
    <article>
      <h2>
        {protein.geneName} <small>({protein.accession})</small>
      </h2>
      <p>
        <strong>{protein.proteinName}</strong> &middot; {protein.organism}
      </p>

      {isShortlisted ? (
        <button
          type="button"
          onClick={() => removeFromShortlist.mutate(protein.accession)}
          disabled={removeFromShortlist.isPending}
        >
          Remove from shortlist
        </button>
      ) : (
        <button
          type="button"
          onClick={() =>
            addToShortlist.mutate({
              accession: protein.accession,
              geneName: protein.geneName,
              proteinName: protein.proteinName,
            })
          }
          disabled={addToShortlist.isPending}
        >
          Add to shortlist
        </button>
      )}

      {protein.functionSummary && (
        <section>
          <h3>Function</h3>
          <p>{protein.functionSummary}</p>
        </section>
      )}

      <section>
        <h3>Subcellular location</h3>
        {protein.subcellularLocations.length > 0 ? (
          <ul>
            {protein.subcellularLocations.map((location) => (
              <li key={location}>{location}</li>
            ))}
          </ul>
        ) : (
          <p>No subcellular location data available.</p>
        )}
      </section>

      <section>
        <h3>Sequence length</h3>
        <p>{protein.sequenceLength} amino acids</p>
      </section>

      <section>
        <h3>Disease associations</h3>
        {protein.diseaseAssociations.length > 0 ? (
          <ul>
            {protein.diseaseAssociations.map((disease) => (
              <li key={disease}>{disease}</li>
            ))}
          </ul>
        ) : (
          <p>No disease associations reported.</p>
        )}
      </section>

      <section>
        <h3>Cross-references</h3>
        {protein.crossReferences.length > 0 ? (
          <ul>
            {protein.crossReferences.map((xref) => (
              <li key={`${xref.database}-${xref.id}`}>
                {xref.database}: {xref.id}
              </li>
            ))}
          </ul>
        ) : (
          <p>No cross-references available.</p>
        )}
      </section>
    </article>
  )
}
