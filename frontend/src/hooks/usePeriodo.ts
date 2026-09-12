import { useContext } from 'react'
import { PeriodoContext } from '@/hooks/periodoContext'

export function usePeriodo() {
  const ctx = useContext(PeriodoContext)
  if (!ctx) throw new Error('usePeriodo deve ser usado dentro de PeriodoProvider')
  return ctx
}
