import {
  Activity,
  CircleDollarSign,
  Target,
  TrendingDown,
  TrendingUp,
  Wallet,
} from 'lucide-react'
import { formatarMoeda, formatarMoedaCompacta, formatarPercentual } from '@/utils/format'
import type { Faturamento } from '@/types/faturamento'

interface Props {
  dados: Faturamento
  variacaoDia?: number
}

export function CardsKpiFaturamento({ dados, variacaoDia = 0 }: Props) {
  const margem = dados.faturamentoMes > 0 ? (dados.lucro / dados.faturamentoMes) * 100 : 0
  const faltaMeta = Math.max(0, dados.metaMensal - dados.faturamentoMes)
  const positivo = variacaoDia >= 0
  const pctCusto = dados.faturamentoMes > 0 ? (dados.custoMes / dados.faturamentoMes) * 100 : 0

  const cards = [
    {
      rotulo: 'Faturamento hoje',
      valor: formatarMoedaCompacta(dados.faturamentoDia),
      detalhe: `${positivo ? '+' : ''}${variacaoDia.toFixed(1)}% vs ontem`,
      detalheCor: positivo ? 'text-emerald-400' : 'text-red-400',
      icone: Activity,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-emerald-400',
    },
    {
      rotulo: 'Faturamento mês',
      valor: formatarMoedaCompacta(dados.faturamentoMes),
      detalhe: `${formatarPercentual(dados.percentualMetaAtingida, 1)} da meta`,
      detalheCor: dados.percentualMetaAtingida >= 80 ? 'text-emerald-400' : 'text-amber-400',
      icone: Wallet,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-rl-heading',
    },
    {
      rotulo: 'Meta mensal',
      valor: formatarMoedaCompacta(dados.metaMensal),
      detalhe: faltaMeta > 0 ? `Faltam ${formatarMoedaCompacta(faltaMeta)}` : 'Meta atingida',
      detalheCor: faltaMeta > 0 ? 'text-slate-400' : 'text-emerald-400',
      icone: Target,
      accent: 'kpi-card-accent-blue',
      corIcone: 'text-blue-400 bg-blue-500/10',
      corValor: 'text-blue-400',
    },
    {
      rotulo: 'Lucro do mês',
      valor: formatarMoedaCompacta(dados.lucro),
      detalhe: `Margem ${formatarPercentual(margem, 1)}`,
      detalheCor: 'text-emerald-400',
      icone: TrendingUp,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-emerald-400',
    },
    {
      rotulo: 'Custo do mês',
      valor: formatarMoedaCompacta(dados.custoMes),
      detalhe: `${formatarPercentual(pctCusto, 1)} do faturamento`,
      detalheCor: 'text-red-400/80',
      icone: TrendingDown,
      accent: 'kpi-card-accent-red',
      corIcone: 'text-red-400 bg-red-500/10',
      corValor: 'text-red-400',
    },
    {
      rotulo: 'Ticket médio',
      valor: formatarMoeda(dados.ticketMedio),
      detalhe: `${dados.quantidadeOperacoes} ${dados.quantidadeOperacoes === 1 ? 'operação' : 'operações'}`,
      detalheCor: 'text-slate-400',
      icone: CircleDollarSign,
      accent: 'kpi-card-accent-amber',
      corIcone: 'text-amber-400 bg-amber-500/10',
      corValor: 'text-rl-heading',
    },
  ]

  return (
    <div className="grid grid-cols-2 gap-3 lg:grid-cols-3 xl:grid-cols-6">
      {cards.map(({ rotulo, valor, detalhe, detalheCor, icone: Icone, accent, corIcone, corValor }) => (
        <div key={rotulo} className={`kpi-card flex h-full flex-col justify-between p-3.5 ${accent}`}>
          <div className="flex items-start justify-between gap-2">
            <p className="section-label leading-tight">{rotulo}</p>
            <div className={`shrink-0 rounded-lg p-1.5 ${corIcone}`}>
              <Icone className="h-4 w-4" />
            </div>
          </div>
          <p className={`mt-3 text-[17px] font-black leading-none tracking-tight sm:text-[18px] ${corValor}`}>
            {valor}
          </p>
          <p className={`mt-2 text-xs font-semibold leading-snug ${detalheCor}`}>{detalhe}</p>
        </div>
      ))}
    </div>
  )
}
