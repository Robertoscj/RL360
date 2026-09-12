import { useNavigate } from 'react-router-dom'
import type { MouseEvent } from 'react'
import { Bell } from 'lucide-react'
import { Topbar } from '@/components/layout/Topbar'
import { ItemAlertaLista } from '@/components/alerts/ItemAlertaLista'
import { LoadingSpinner } from '@/components/ui/LoadingSpinner'
import { useAlertasNotificacao } from '@/hooks/useAlertasNotificacao'
import { resolverAlerta } from '@/services/alertaService'
import { rotaParaAlerta } from '@/utils/alertas'

export function AlertasPage() {
  const navigate = useNavigate()
  const {
    alertas,
    carregando,
    erro,
    recarregar,
    marcarComoLido,
    removerAlerta,
    alertaNaoLido,
  } = useAlertasNotificacao()

  async function resolver(id: string, e: MouseEvent) {
    e.stopPropagation()
    try {
      await resolverAlerta(id)
      removerAlerta(id)
    } catch {
      /* mantém na lista se falhar */
    }
  }

  return (
    <div className="min-h-screen bg-rl-bg">
      <Topbar
        titulo="Central de Alertas"
        subtitulo="Histórico e gestão de avisos do radar"
        onAtualizar={() => void recarregar()}
        atualizando={carregando}
      />

      <div className="p-5">
        {carregando && alertas.length === 0 ? (
          <LoadingSpinner />
        ) : erro ? (
          <div className="glass-card p-6 text-center text-sm text-red-400">{erro}</div>
        ) : alertas.length === 0 ? (
          <div className="glass-card flex flex-col items-center justify-center py-16 text-center">
            <Bell className="mb-3 h-10 w-10 text-rl-muted" />
            <p className="text-sm font-semibold text-rl-heading">Nenhum alerta ativo</p>
            <p className="mt-1 max-w-sm text-xs text-rl-muted">
              Quando o radar detectar riscos ou oportunidades, eles aparecerão aqui e no sino da topbar.
            </p>
          </div>
        ) : (
          <div className="glass-card divide-y divide-rl-border overflow-hidden">
            {alertas.map((alerta) => (
              <div key={alerta.id} className="group relative">
                <ItemAlertaLista
                  alerta={alerta}
                  naoLido={alertaNaoLido(alerta.id)}
                  onClick={() => {
                    marcarComoLido(alerta.id)
                    navigate(rotaParaAlerta(alerta))
                  }}
                />
                <button
                  type="button"
                  onClick={(e) => void resolver(alerta.id, e)}
                  className="absolute right-10 top-1/2 hidden -translate-y-1/2 rounded-md border border-rl-border bg-rl-surface px-2 py-1 text-[10px] font-semibold text-rl-muted transition hover:border-emerald-500/40 hover:text-emerald-600 group-hover:block"
                >
                  Resolver
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
