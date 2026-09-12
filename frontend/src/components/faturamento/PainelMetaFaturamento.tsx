import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { Faturamento } from '@/types/faturamento'

interface Props {
  dados: Faturamento
}

export function PainelMetaFaturamento({ dados }: Props) {
  const falta = Math.max(0, dados.metaMensal - dados.faturamentoMes)
  const diasRestantes = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate() - new Date().getDate()
  const mediaNecessaria = diasRestantes > 0 ? falta / diasRestantes : 0
  const pct = Math.min(dados.percentualMetaAtingida, 100)

  return (
    <div className="glass-card flex h-full flex-col p-4">
      <p className="section-label">Progresso da meta</p>
      <p className="section-desc">Acompanhamento mensal em tempo real</p>

      <div className="relative mx-auto my-4 flex h-[120px] w-[120px] items-center justify-center">
        <svg className="absolute inset-0 h-full w-full -rotate-90" viewBox="0 0 100 100">
          <circle cx="50" cy="50" r="42" fill="none" stroke="#1e2836" strokeWidth="8" />
          <circle
            cx="50"
            cy="50"
            r="42"
            fill="none"
            stroke="#22c55e"
            strokeWidth="8"
            strokeLinecap="round"
            strokeDasharray={`${pct * 2.64} 264`}
            style={{ filter: 'drop-shadow(0 0 6px rgba(34,197,94,0.4))' }}
          />
        </svg>
        <div className="text-center">
          <p className="text-3xl font-black text-emerald-400">{formatarPercentual(pct, 0)}</p>
          <p className="text-xs font-medium text-rl-body">atingido</p>
        </div>
      </div>

      <div className="space-y-3">
        <div className="flex justify-between text-xs">
          <span className="font-medium text-rl-body">Realizado</span>
          <span className="font-bold text-rl-heading">{formatarMoeda(dados.faturamentoMes)}</span>
        </div>
        <div className="flex justify-between text-xs">
          <span className="font-medium text-rl-body">Meta</span>
          <span className="font-bold text-blue-400">{formatarMoeda(dados.metaMensal)}</span>
        </div>
        <div className="flex justify-between text-xs">
          <span className="font-medium text-rl-body">Falta</span>
          <span className="font-bold text-amber-400">{formatarMoeda(falta)}</span>
        </div>
        <div className="rounded-lg border border-rl-border bg-rl-surface/60 p-3">
          <p className="section-label">Para bater a meta</p>
          <p className="mt-1 text-[13px] font-black text-rl-heading">
            {formatarMoeda(mediaNecessaria)}<span className="text-xs font-medium text-rl-body">/dia</span>
          </p>
          <p className="mt-0.5 text-xs font-medium text-rl-body">{diasRestantes} dias restantes no mês</p>
        </div>
      </div>
    </div>
  )
}
