import { createContext } from 'react'
import type { IntervaloDatas } from '@/utils/datas'

export interface PeriodoContexto {
  periodo: IntervaloDatas
  aplicarPeriodo: (inicio: Date, fim: Date) => void
  aplicarPredefinido: (tipo: string) => void
}

export const PeriodoContext = createContext<PeriodoContexto | null>(null)
