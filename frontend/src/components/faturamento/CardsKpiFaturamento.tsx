import {
  Activity,
  CircleDollarSign,
  Target,
  TrendingDown,
  TrendingUp,
  Wallet,
} from 'lucide-react'
import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { Faturamento } from '@/types/faturamento'

interface Props {
  dados: Faturamento
  variacaoDia?: number
}

export function CardsKpiFaturamento({ dados, variacaoDia = 0 }: Props) {
  const margem = dados.faturamentoMes > 0 ? (dados.lucro / dados.faturamentoMes) * 100 : 0
  const faltaMeta = Math.max(0, dados.metaMensal - dados.faturamentoMes)
  const positivo = variacaoDia >= 0

  const cards = [
    {
      rotulo: 'Faturamento Hoje',
      valor: formatarMoeda(dados.faturamentoDia),
      detalhe: `${positivo ? '+' : ''}${variacaoDia.toFixed(1)}% vs ontem`,
      detalheCor: positivo ? 'text-emerald-400' : 'text-red-400',
      descricao: 'Receita consolidada do dia',
      icone: Activity,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-emerald-400',
    },
    {
      rotulo: 'Faturamento Mês',
      valor: formatarMoeda(dados.faturamentoMes),
      detalhe: formatarPercentual(dados.percentualMetaAtingida, 1) + ' da meta',
      detalheCor: dados.percentualMetaAtingida >= 80 ? 'text-emerald-400' : 'text-amber-400',
      descricao: 'Acumulado no período atual',
      icone: Wallet,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-rl-heading',
    },
    {
      rotulo: 'Meta Mensal',
      valor: formatarMoeda(dados.metaMensal),
      detalhe: `Faltam ${formatarMoeda(faltaMeta)}`,
      detalheCor: 'text-slate-400',
      descricao: 'Objetivo de receita do mês',
      icone: Target,
      accent: 'kpi-card-accent-blue',
      corIcone: 'text-blue-400 bg-blue-500/10',
      corValor: 'text-blue-400',
    },
    {
      rotulo: 'Lucro do Mês',
      valor: formatarMoeda(dados.lucro),
      detalhe: `Margem ${formatarPercentual(margem, 1)}`,
      detalheCor: 'text-emerald-400',
      descricao: 'Receita menos custos variáveis',
      icone: TrendingUp,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-emerald-400',
    },
    {
      rotulo: 'Custo do Mês',
      valor: formatarMoeda(dados.custoMes),
      detalhe: `${formatarPercentual(dados.faturamentoMes > 0 ? (dados.custoMes / dados.faturamentoMes) * 100 : 0, 1)} do faturamento`,
      detalheCor: 'text-red-400/80',
      descricao: 'Despesas operacionais acumuladas',
      icone: TrendingDown,
      accent: 'kpi-card-accent-red',
      corIcone: 'text-red-400 bg-red-500/10',
      corValor: 'text-red-400',
    },
    {
      rotulo: 'Ticket Médio Dia',
      valor: formatarMoeda(dados.faturamentoDia > 0 ? Math.round(dados.faturamentoDia / 47) : 0),
      detalhe: '47 transações hoje',
      detalheCor: 'text-slate-400',
      descricao: 'Valor médio por operação',
      icone: CircleDollarSign,
      accent: 'kpi-card-accent-amber',
      corIcone: 'text-amber-400 bg-amber-500/10',
      corValor: 'text-rl-heading',
    },
  ]

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
      {cards.map(({ rotulo, valor, detalhe, detalheCor, descricao, icone: Icone, accent, corIcone, corValor }) => (
        <div key={rotulo} className={`kpi-card ${accent}`}>
          <div className="flex items-start justify-between">
            <div className="flex-1 min-w-0">
              <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">{rotulo}</p>
              <p className={`mt-1.5 text-[22px] font-black leading-none ${corValor}`}>{valor}</p>
              <p className={`mt-1 text-[11px] font-semibold ${detalheCor}`}>{detalhe}</p>
              <p className="mt-2 text-[10px] leading-snug text-slate-600">{descricao}</p>
            </div>
            <div className={`rounded-lg p-2 shrink-0 ${corIcone}`}>
              <Icone className="h-5 w-5" />
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}
