import { useEffect, useRef, useState } from 'react'
import { Bell, Loader2 } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { ItemAlertaLista } from '@/components/alerts/ItemAlertaLista'
import { useAlertasNotificacao } from '@/hooks/useAlertasNotificacao'
import { rotaParaAlerta } from '@/utils/alertas'

export function CentralNotificacoes() {
  const navigate = useNavigate()
  const [aberto, setAberto] = useState(false)
  const painelRef = useRef<HTMLDivElement>(null)
  const botaoRef = useRef<HTMLButtonElement>(null)

  const {
    alertas,
    carregando,
    erro,
    naoLidos,
    marcarComoLido,
    marcarTodosComoLidos,
  } = useAlertasNotificacao()

  useEffect(() => {
    if (!aberto) return

    marcarTodosComoLidos()

    function fecharSeFora(e: MouseEvent) {
      const alvo = e.target as Node
      if (
        painelRef.current?.contains(alvo) ||
        botaoRef.current?.contains(alvo)
      ) {
        return
      }
      setAberto(false)
    }

    document.addEventListener('mousedown', fecharSeFora)
    return () => document.removeEventListener('mousedown', fecharSeFora)
  }, [aberto, marcarTodosComoLidos])

  function abrirAlerta(id: string) {
    const alerta = alertas.find((a) => a.id === id)
    if (!alerta) return
    marcarComoLido(id)
    setAberto(false)
    navigate(rotaParaAlerta(alerta))
  }

  return (
    <div className="relative">
      <button
        ref={botaoRef}
        type="button"
        onClick={() => setAberto((v) => !v)}
        className="btn-ghost relative p-2"
        title="Central de alertas"
        aria-label="Central de alertas"
        aria-expanded={aberto}
      >
        <Bell className="h-4 w-4" />
        {naoLidos > 0 && (
          <span className="absolute right-1 top-1 flex h-[18px] min-w-[18px] items-center justify-center rounded-full bg-red-500 px-1 text-[9px] font-bold text-white ring-2 ring-rl-bg">
            {naoLidos > 9 ? '9+' : naoLidos}
          </span>
        )}
      </button>

      {aberto && (
        <div
          ref={painelRef}
          role="dialog"
          aria-label="Alertas recentes"
          className="absolute right-0 top-full z-50 mt-2 w-[min(100vw-2rem,380px)] overflow-hidden rounded-xl border border-rl-border bg-rl-popover shadow-2xl"
        >
          <div className="flex items-center justify-between border-b border-rl-border px-4 py-3">
            <div>
              <p className="text-xs font-bold uppercase tracking-wide text-rl-heading">Alertas</p>
              <p className="text-[10px] text-rl-muted">
                {alertas.length === 0
                  ? 'Nenhum alerta ativo'
                  : `${alertas.length} alerta${alertas.length !== 1 ? 's' : ''} ativo${alertas.length !== 1 ? 's' : ''}`}
              </p>
            </div>
            {carregando && <Loader2 className="h-4 w-4 animate-spin text-rl-muted" />}
          </div>

          <div className="max-h-[min(60vh,420px)] overflow-y-auto">
            {erro && (
              <p className="px-4 py-6 text-center text-xs text-red-400">{erro}</p>
            )}

            {!erro && !carregando && alertas.length === 0 && (
              <p className="px-4 py-8 text-center text-xs text-rl-muted">
                Tudo certo por aqui — nenhum alerta crítico no momento.
              </p>
            )}

            {!erro &&
              alertas.map((alerta) => (
                <ItemAlertaLista
                  key={alerta.id}
                  alerta={alerta}
                  compacto
                  onClick={() => abrirAlerta(alerta.id)}
                />
              ))}
          </div>

          <div className="border-t border-rl-border p-2">
            <button
              type="button"
              onClick={() => {
                setAberto(false)
                navigate('/alertas')
              }}
              className="w-full rounded-lg py-2 text-center text-[11px] font-semibold text-emerald-600 transition hover:bg-emerald-500/10"
            >
              Ver todos os alertas
            </button>
          </div>
        </div>
      )}
    </div>
  )
}
