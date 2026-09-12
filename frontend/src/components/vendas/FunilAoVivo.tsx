import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { EtapaFunil } from '@/types/vendas'

interface Props {
  etapas: EtapaFunil[]
  fechados?: { quantidade: number; valor: number }
}

const CORES = ['#22c55e', '#14b8a6', '#3b82f6', '#8b5cf6', '#f59e0b']

function pctTaxa(valor: number) {
  return valor <= 1 ? valor * 100 : valor
}

export function FunilAoVivo({ etapas, fechados }: Props) {
  const ordenadas = [...etapas].sort((a, b) => a.ordem - b.ordem)
  const maxQtd = Math.max(...ordenadas.map((e) => e.quantidade), 1)

  const linhas = fechados
    ? [
        ...ordenadas,
        {
          nome: 'Fechado',
          ordem: ordenadas.length + 1,
          quantidade: fechados.quantidade,
          valorPotencial: fechados.valor,
          taxaConversao: ordenadas[0]?.quantidade
            ? fechados.quantidade / ordenadas[0].quantidade
            : 0,
          taxaConversaoBase: 0.08,
        },
      ]
    : ordenadas

  return (
    <div className="glass-card p-4">
      <div className="mb-4 flex flex-wrap items-start justify-between gap-2">
        <div>
          <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Funil ao vivo</p>
          <p className="text-[11px] text-slate-600">Pipeline comercial em tempo real</p>
        </div>
        <span className="inline-flex items-center gap-1.5 rounded-full border border-emerald-500/30 bg-emerald-500/10 px-2 py-0.5 text-[9px] font-bold uppercase text-emerald-400">
          <span className="h-1.5 w-1.5 animate-pulse rounded-full bg-emerald-400" />
          Live
        </span>
      </div>

      <div className="space-y-3">
        {linhas.map((etapa, i) => {
          const largura = Math.max(18, (etapa.quantidade / maxQtd) * 100)
          const taxa = pctTaxa(etapa.taxaConversao)
          const base = pctTaxa(etapa.taxaConversaoBase)
          const queda = base > 0 ? base - taxa : 0
          const saudavel = queda <= 3 || etapa.nome === 'Fechado'
          const cor = CORES[i % CORES.length]

          return (
            <div key={etapa.nome}>
              <div className="mb-1.5 flex flex-wrap items-end justify-between gap-2">
                <div>
                  <p className="text-[12px] font-bold text-slate-200">{etapa.nome}</p>
                  <p className="text-[10px] text-slate-500">
                    {etapa.quantidade.toLocaleString('pt-BR')} ops · {formatarMoeda(etapa.valorPotencial)}
                  </p>
                </div>
                <div className="text-right">
                  <p className={`text-[13px] font-black ${saudavel ? 'text-emerald-400' : 'text-amber-400'}`}>
                    {formatarPercentual(taxa, 1)}
                  </p>
                  {base > 0 && (
                    <p className={`text-[9px] font-semibold ${queda > 0 ? 'text-red-400' : 'text-emerald-400'}`}>
                      {queda > 0 ? `▼ ${queda.toFixed(1)}pp` : `▲ ${Math.abs(queda).toFixed(1)}pp`} vs meta
                    </p>
                  )}
                </div>
              </div>

              <div className="relative h-9 overflow-hidden rounded-lg bg-rl-surface">
                <div
                  className="absolute inset-y-0 left-0 flex items-center rounded-lg transition-all duration-700"
                  style={{
                    width: `${largura}%`,
                    background: `linear-gradient(90deg, ${cor}33 0%, ${cor}18 100%)`,
                    borderRight: `2px solid ${cor}`,
                    boxShadow: `0 0 20px ${cor}22`,
                  }}
                >
                  <span className="pl-3 text-[10px] font-bold text-white/90">
                    {((etapa.quantidade / maxQtd) * 100).toFixed(0)}%
                  </span>
                </div>
              </div>

              {i < linhas.length - 1 && (
                <div className="my-1 flex justify-center">
                  <div className="h-3 w-px bg-gradient-to-b from-slate-600 to-transparent" />
                </div>
              )}
            </div>
          )
        })}
      </div>
    </div>
  )
}
