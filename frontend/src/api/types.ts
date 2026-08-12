export interface ProteinSummary {
  accession: string
  geneName: string
  proteinName: string
  organism: string
}

export interface CrossReference {
  database: string
  id: string
}

export interface ProteinDetail {
  accession: string
  geneName: string
  proteinName: string
  organism: string
  functionSummary: string | null
  subcellularLocations: string[]
  sequenceLength: number
  diseaseAssociations: string[]
  crossReferences: CrossReference[]
}

export interface ShortlistItem {
  accession: string
  geneName: string
  proteinName: string
  addedAtUtc: string
}

export interface ShortlistCreateRequest {
  accession: string
  geneName: string
  proteinName: string
}
