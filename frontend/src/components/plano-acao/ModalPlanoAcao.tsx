import { useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { ChevronRight, Target, X } from 'lucide-react'
import { formatarMoeda } from '@/utils/format'
import {
  estiloPrioridadePlano,
  ordenarPlanoAcao,
  rotaParaPlanoAcao,
  rotuloPrioridadePlano,
  rotuloRota,
} from '@/utils/planoAcao'
import type { ItemPlanoAcao } from '@/types/dashboard'

interface Props {
  aberto: boolean
  onFechar: () => void
  itens: ItemPlanoAcao[]
}

export function ModalPlanoAcao({ aberto, onFechar, itens }: Props) {
  const navigate = useNavigate()
  const painelRef = useRef<HTMLDivElement>(null)
  const ordenados = ordenarPlanoAcao(itens)
  const impactoTotal = ordenados.reduce((s, i) => s + i.impactoEsperado, 0)

  useEffect(() => {
    if (!aberto) return

    function onKey(e: KeyboardEvent) {
      if (e.key === 'Escape') onFechar()
    }

    document.addEventListener('keydown', onKey)
    document.body.style.overflow = 'hidden'
    return () => {
      document.removeEventListener('keydown', onKey)
      document.body.style.overflow = ''
    }
  }, [aberto, onFechar])

  if (!aberto) return null

  function irParaAcao(item: ItemPlanoAcao) {
    onFechar()
    navigate(rotaParaPlanoAcao(item))
  }

  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center p-4 sm:items-center">
      <button
        type="button"
        className="absolute inset-0 bg-black/50 backdrop-blur-sm"
        aria-label="Fechar plano de ação"
        onClick={onFechar}
      />

      <div
        ref={painelRef}
        role="dialog"
        aria-modal="true"
        aria-label="Plano de ação sugerido"
        className="relative flex max-h-[min(90vh,720px)] w-full max-w-2xl flex-col overflow-hidden rounded-2xl border border-rl-border bg-rl-popover shadow-2xl"
      >
        <div className="border-b border-rl-border bg-gradient-to-r from-emerald-600/10 to-emerald-500/5 px-5 py-4">
          <div className="flex items-start justify-between gap-3">
            <div className="flex items-start gap-3">
              <div className="rounded-xl bg-emerald-500/15 p-2.5 text-emerald-600">
                <Target className="h-5 w-5" />
              </div>
              <div>
                <h2 className="text-base font-black text-rl-heading">Plano de Ação Sugerido</h2>
                <p className="mt-0.5 text-xs text-rl-muted">
                  {ordenados.length} ação{ordenados.length !== 1 ? 'ões' : ''} priorizadas pelo radar de lucro
                </p>
                {impactoTotal > 0 && (
                  <p className="mt-1 text-[11px] font-semibold text-emerald-600">
                    Impacto potencial combinado: {formatarMoeda(impactoTotal)}
                  </p>
                )}
              </div>
            </div>
            <button
              type="button"
              onClick={onFechar}
              className="rounded-lg p-1.5 text-rl-muted transition hover:bg-rl-card hover:text-rl-heading"
              aria-label="Fechar"
            >
              <X className="h-5 w-5" />
            </button>
          </div>
        </div>

        <div className="flex-1 overflow-y-auto p-4">
          {ordenados.length === 0 ? (
            <p className="py-8 text-center text-sm text-rl-muted">
              Nenhuma ação sugerida no momento. O radar está estável.
            </p>
          ) : (
            <div className="space-y-3">
              {ordenados.map((item, indice) => {
                const estilo = estiloPrioridadePlano(item.prioridade)
                const rota = rotaParaPlanoAcao(item)
                return (
                  <article
                    key={`${item.titulo}-${indice}`}
                    className={`rounded-xl border border-rl-border border-l-[3px] p-4 ${estilo.borda} ${estilo.fundo}`}
                  >
                    <div className="flex flex-wrap items-start justify-between gap-2">
                      <span
                        className={`rounded-full border px-2 py-0.5 text-[10px] font-bold uppercase tracking-wide ${estilo.badge}`}
                      >
                        {rotuloPrioridadePlano(item.prioridade)}
                      </span>
                      <span className="text-sm font-black text-emerald-600">
                        +{formatarMoeda(item.impactoEsperado)}
                      </span>
                    </div>
                    <h3 className="mt-2 text-sm font-bold leading-snug text-rl-heading">{item.titulo}</h3>
                    <p className="mt-1 text-xs leading-relaxed text-rl-muted">{item.justificativa}</p>
                    <button
                      type="button"
                      onClick={() => irParaAcao(item)}
                      className="mt-3 inline-flex items-center gap-1 text-[11px] font-bold text-emerald-600 transition hover:text-emerald-500"
                    >
                      {rotuloRota(rota)}
                      <ChevronRight className="h-3.5 w-3.5" />
                    </button>
                  </article>
                )
              })}
            </div>
          )}
        </div>

        <div className="border-t border-rl-border bg-rl-surface px-5 py-3">
          <p className="text-center text-[10px] text-rl-muted">
            Ações geradas automaticamente com base nos fatores do radar · atualize o dashboard para recalcular
          </p>
        </div>
      </div>
    </div>
  )
}
