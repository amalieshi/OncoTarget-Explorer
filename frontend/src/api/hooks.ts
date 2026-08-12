import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from './client'
import type { ProteinDetail, ProteinSummary, ShortlistCreateRequest, ShortlistItem } from './types'

const shortlistKey = ['shortlist'] as const

export function useSearchProteins(query: string) {
  return useQuery({
    queryKey: ['proteins', 'search', query],
    queryFn: () => apiClient.get<ProteinSummary[]>(`/api/proteins/search?query=${encodeURIComponent(query)}`),
    enabled: query.trim().length > 0,
  })
}

export function useProteinDetail(accession: string | null) {
  return useQuery({
    queryKey: ['proteins', 'detail', accession],
    queryFn: () => apiClient.get<ProteinDetail>(`/api/proteins/${encodeURIComponent(accession!)}`),
    enabled: accession !== null,
  })
}

export function useShortlist() {
  return useQuery({
    queryKey: shortlistKey,
    queryFn: () => apiClient.get<ShortlistItem[]>('/api/shortlist'),
  })
}

export function useAddToShortlist() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: ShortlistCreateRequest) => apiClient.post<ShortlistItem>('/api/shortlist', request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: shortlistKey }),
  })
}

export function useRemoveFromShortlist() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (accession: string) => apiClient.delete(`/api/shortlist/${encodeURIComponent(accession)}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: shortlistKey }),
  })
}
