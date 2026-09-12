import { useCallback, useEffect, useMemo, useState } from 'react'
import { useAuth } from '@/hooks/useAuth'
import { obterAlertas } from '@/services/alertaService'
import type { Alerta } from '@/types/alerta'

const CHAVE_LIDOS = 'rl360:alertas-lidos'
const INTERVALO_MS = 60_000

function carregarLidos(): Set<string> {
  try {
    const raw = localStorage.getItem(CHAVE_LIDOS)
    if (raw) return new Set(JSON.parse(raw) as string[])
  } catch {
    /* ignora */
  }
  return new Set()
}

function salvarLidos(ids: Set<string>) {
  localStorage.setItem(CHAVE_LIDOS, JSON.stringify([...ids]))
}

export function useAlertasNotificacao() {
  const { autenticado } = useAuth()
  const [alertas, setAlertas] = useState<Alerta[]>([])
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')
  const [lidos, setLidos] = useState<Set<string>>(() => carregarLidos())

  const recarregar = useCallback(async () => {
    if (!autenticado) return
    try {
      setErro('')
      setCarregando(true)
      const lista = await obterAlertas()
      setAlertas(lista)
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao carregar alertas')
    } finally {
      setCarregando(false)
    }
  }, [autenticado])

  useEffect(() => {
    void recarregar()
    if (!autenticado) return
    const id = window.setInterval(() => void recarregar(), INTERVALO_MS)
    return () => window.clearInterval(id)
  }, [autenticado, recarregar])

  const naoLidos = useMemo(
    () => alertas.filter((a) => !lidos.has(a.id)).length,
    [alertas, lidos],
  )

  const marcarComoLido = useCallback((id: string) => {
    setLidos((atual) => {
      if (atual.has(id)) return atual
      const next = new Set(atual)
      next.add(id)
      salvarLidos(next)
      return next
    })
  }, [])

  const marcarTodosComoLidos = useCallback(() => {
    setLidos((atual) => {
      const next = new Set(atual)
      alertas.forEach((a) => next.add(a.id))
      salvarLidos(next)
      return next
    })
  }, [alertas])

  const removerAlerta = useCallback((id: string) => {
    setAlertas((atual) => atual.filter((a) => a.id !== id))
    marcarComoLido(id)
  }, [marcarComoLido])

  return {
    alertas,
    carregando,
    erro,
    naoLidos,
    recarregar,
    marcarComoLido,
    marcarTodosComoLidos,
    removerAlerta,
    alertaNaoLido: (id: string) => !lidos.has(id),
  }
}
