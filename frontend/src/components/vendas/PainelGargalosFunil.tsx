import { AlertTriangle, ChevronRight } from 'lucide-react'
import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { EtapaFunil } from '@/types/vendas'

interface Props {
  etapas: EtapaFunil[]
}

function pctTaxa(valor: number) {
  return valor <= 1 ? valor * 100 : valor
}

export function PainelGargalosFunil({ etapas }: Props) {
  const comQueda = [...etapas]
    .map((e) => ({
      ...e,
      taxaAtual: pctTaxa(e.taxaConversao),
      taxaMeta: pctTaxa(e.taxaConversaoBase),
      queda: pctTaxa(e.taxaConversaoBase) - pctTaxa(e.taxaConversao),
    }))
    .filter((e) => e.queda > 0)
    .sort((a, b) => b.queda - a.queda)

  const pior = comQueda[0]

  return (
    <div className="glass-card flex h-full flex-col p-4">
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Gargalos do funil</p>
      <p className="text-[11px] text-slate-600">Etapas com maior queda de conversão</p>

      {pior && (
        <div className="mt-4 rounded-xl border border-amber-500/25 bg-amber-500/[0.06] p-3">
          <div className="flex items-start gap-2">
            <AlertTriangle className="mt-0.5 h-4 w-4 shrink-0 text-amber-400" />
            <div>
              <p className="text-[10px] font-bold uppercase text-amber-400">Maior gargalo</p>
              <p className="mt-1 text-[13px] font-bold text-rl-heading">{pior.nome}</p>
              <p className="mt-1 text-[11px] text-slate-400">
                Conversão {formatarPercentual(pior.taxaAtual, 1)} vs meta {formatarPercentual(pior.taxaMeta, 1)}
              </p>
              <p className="mt-2 text-sm font-black text-red-400">▼ {pior.queda.toFixed(1)} pontos percentuais</p>
              <p className="mt-1 text-[10px] text-slate-500">
                Impacto estimado: {formatarMoeda(pior.valorPotencial * (pior.queda / 100))} em risco
              </p>
            </div>
          </div>
        </div>
      )}

      <div className="mt-4 flex-1 space-y-2">
        {comQueda.map((e) => (
          <div
            key={e.nome}
            className="flex items-center justify-between rounded-lg border border-rl-border bg-rl-surface/50 px-3 py-2.5"
          >
            <div>
              <p className="text-[11px] font-semibold text-slate-200">{e.nome}</p>
              <p className="text-[10px] text-slate-500">{e.quantidade} oportunidades</p>
            </div>
            <div className="text-right">
              <p className="text-[11px] font-bold text-red-400">-{e.queda.toFixed(1)}pp</p>
              <p className="text-[9px] text-slate-600">{formatarMoeda(e.valorPotencial)}</p>
            </div>
          </div>
        ))}
      </div>

      <button
        type="button"
        className="mt-4 inline-flex items-center gap-1 text-[11px] font-semibold text-emerald-400 hover:text-emerald-300"
      >
        Ver plano de ação <ChevronRight className="h-3 w-3" />
      </button>
    </div>
  )
}
