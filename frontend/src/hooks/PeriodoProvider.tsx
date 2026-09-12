import {
  useCallback,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { PeriodoContext } from '@/hooks/periodoContext'
import {
  deIsoData,
  normalizarIntervalo,
  paraIsoData,
  periodoPredefinido,
  type IntervaloDatas,
} from '@/utils/datas'

const CHAVE_STORAGE = 'rl360:periodo'

function carregarPeriodoSalvo(): IntervaloDatas {
  try {
    const raw = localStorage.getItem(CHAVE_STORAGE)
    if (raw) {
      const parsed = JSON.parse(raw) as { inicio: string; fim: string }
      return { inicio: deIsoData(parsed.inicio), fim: deIsoData(parsed.fim) }
    }
  } catch {
    /* usa padrão */
  }
  return periodoPredefinido('esteMes')
}

function salvarPeriodo(periodo: IntervaloDatas) {
  localStorage.setItem(
    CHAVE_STORAGE,
    JSON.stringify({ inicio: paraIsoData(periodo.inicio), fim: paraIsoData(periodo.fim) }),
  )
}

export function PeriodoProvider({ children }: { children: ReactNode }) {
  const [periodo, setPeriodo] = useState<IntervaloDatas>(carregarPeriodoSalvo)

  const aplicarPeriodo = useCallback((inicio: Date, fim: Date) => {
    const next = normalizarIntervalo(inicio, fim)
    setPeriodo(next)
    salvarPeriodo(next)
  }, [])

  const aplicarPredefinido = useCallback((tipo: string) => {
    const next = periodoPredefinido(tipo)
    setPeriodo(next)
    salvarPeriodo(next)
  }, [])

  const valor = useMemo(
    () => ({ periodo, aplicarPeriodo, aplicarPredefinido }),
    [periodo, aplicarPeriodo, aplicarPredefinido],
  )

  return <PeriodoContext.Provider value={valor}>{children}</PeriodoContext.Provider>
}
