import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect } from 'react'
import { useAuth } from '@/hooks/useAuth'
import type { ResumoDashboard, SnapshotRadar } from '@/types/dashboard'

interface Opcoes {
  onDashboard?: (resumo: ResumoDashboard) => void
  onRadar?: (radar: SnapshotRadar) => void
}

export function useDashboardTempoReal({ onDashboard, onRadar }: Opcoes) {
  const { sessao } = useAuth()

  useEffect(() => {
    const token = sessao?.token
    if (!token) return

    const conexao = new HubConnectionBuilder()
      .withUrl('/hubs/dashboard', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .configureLogging(LogLevel.Warning)
      .build()

    if (onDashboard) {
      conexao.on('dashboard:atualizado', (resumo: ResumoDashboard) => onDashboard(resumo))
    }
    if (onRadar) {
      conexao.on('radar:atualizado', (radar: SnapshotRadar) => onRadar(radar))
    }

    void conexao.start().catch(() => {
      /* polling cobre ausência do hub */
    })

    return () => {
      void conexao.stop()
    }
  }, [sessao?.token, onDashboard, onRadar])
}
