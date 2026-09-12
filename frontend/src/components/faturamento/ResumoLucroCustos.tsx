import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { Faturamento } from '@/types/faturamento'

interface Props {
  dados: Faturamento
}

export function ResumoLucroCustos({ dados }: Props) {
  const margem = dados.faturamentoMes > 0 ? (dados.lucro / dados.faturamentoMes) * 100 : 0
  const pctCusto = dados.faturamentoMes > 0 ? (dados.custoMes / dados.faturamentoMes) * 100 : 0
  const pctLucro = 100 - pctCusto
  const lucroPorReal = dados.faturamentoMes > 0 ? dados.lucro / dados.faturamentoMes : 0
  const lucroPorRealTexto = lucroPorReal.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })

  return (
    <div className="glass-card flex h-full flex-col p-4">
      <p className="section-label">Lucro vs custo</p>
      <p className="section-desc">Composição do resultado mensal</p>

      <div className="mt-5">
        <div className="flex h-3 overflow-hidden rounded-full">
          <div className="bg-emerald-500" style={{ width: `${pctLucro}%` }} />
          <div className="bg-red-500/70" style={{ width: `${pctCusto}%` }} />
        </div>
        <div className="mt-2 flex justify-between text-[11px] font-medium text-rl-body">
          <span>Lucro {formatarPercentual(pctLucro, 0)}</span>
          <span>Custo {formatarPercentual(pctCusto, 0)}</span>
        </div>
      </div>

      <div className="mt-5 grid grid-cols-2 gap-3">
        <div className="rounded-lg border border-emerald-500/20 bg-emerald-500/[0.06] p-3">
          <p className="text-[9px] font-bold uppercase text-emerald-400/80">Lucro</p>
          <p className="mt-1 text-lg font-black text-emerald-400">{formatarMoeda(dados.lucro)}</p>
          <p className="text-xs font-medium text-rl-body">Margem {formatarPercentual(margem, 1)}</p>
        </div>
        <div className="rounded-lg border border-red-500/20 bg-red-500/[0.06] p-3">
          <p className="text-[9px] font-bold uppercase text-red-400/80">Custo</p>
          <p className="mt-1 text-lg font-black text-red-400">{formatarMoeda(dados.custoMes)}</p>
          <p className="text-xs font-medium text-rl-body">{formatarPercentual(pctCusto, 1)} do faturamento</p>
        </div>
      </div>

      <div className="mt-4 rounded-lg border border-rl-border bg-rl-surface/50 p-3">
        <p className="text-xs font-medium leading-relaxed text-rl-body">
          Cada <strong className="text-emerald-400">R$ 1,00</strong> de faturamento gera{' '}
          <strong className="text-rl-heading">{lucroPorRealTexto}</strong> de lucro líquido neste período.
        </p>
      </div>
    </div>
  )
}
