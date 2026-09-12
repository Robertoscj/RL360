import {
  Filter,
  Target,
  TrendingDown,
  TrendingUp,
  UserPlus,
  Zap,
} from 'lucide-react'
import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { EtapaFunil, ResumoVendas } from '@/types/vendas'

interface Props {
  resumo: ResumoVendas
  funil: EtapaFunil[]
  conversaoGeral: number
  quedaConversao: number
}

export function CardsKpiVendas({ resumo, funil, conversaoGeral, quedaConversao }: Props) {
  const pipeline = funil.reduce((s, e) => s + e.valorPotencial, 0)
  const oportunidades = funil.reduce((s, e) => s + e.quantidade, 0)

  const cards = [
    {
      rotulo: 'Vendas Fechadas',
      valor: formatarMoeda(resumo.totalVendas),
      detalhe: `${resumo.quantidadeVendas} operações`,
      detalheCor: 'text-emerald-400',
      descricao: 'Receita confirmada no mês',
      icone: TrendingUp,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-emerald-400',
    },
    {
      rotulo: 'Pipeline Ativo',
      valor: formatarMoeda(pipeline),
      detalhe: `${oportunidades.toLocaleString('pt-BR')} oportunidades`,
      detalheCor: 'text-blue-400',
      descricao: 'Valor potencial no funil',
      icone: Filter,
      accent: 'kpi-card-accent-blue',
      corIcone: 'text-blue-400 bg-blue-500/10',
      corValor: 'text-rl-heading',
    },
    {
      rotulo: 'Conversão Geral',
      valor: formatarPercentual(conversaoGeral * 100, 1),
      detalhe: `${quedaConversao >= 0 ? '-' : '+'}${Math.abs(quedaConversao).toFixed(1)}pp vs meta`,
      detalheCor: quedaConversao > 0 ? 'text-red-400' : 'text-emerald-400',
      descricao: 'Lead até negociação',
      icone: Target,
      accent: 'kpi-card-accent-amber',
      corIcone: 'text-amber-400 bg-amber-500/10',
      corValor: 'text-amber-400',
    },
    {
      rotulo: 'Margem Total',
      valor: formatarMoeda(resumo.totalMargem),
      detalhe: resumo.totalVendas > 0
        ? formatarPercentual((resumo.totalMargem / resumo.totalVendas) * 100, 1) + ' de margem'
        : '—',
      detalheCor: 'text-emerald-400',
      descricao: 'Lucro bruto das vendas',
      icone: Zap,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-rl-heading',
    },
    {
      rotulo: 'Novos Clientes',
      valor: String(resumo.novosClientes),
      detalhe: `${resumo.quantidadeVendas > 0 ? Math.round((resumo.novosClientes / resumo.quantidadeVendas) * 100) : 0}% das vendas`,
      detalheCor: 'text-emerald-400',
      descricao: 'Aquisições no período',
      icone: UserPlus,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-rl-heading',
    },
    {
      rotulo: 'Ticket Médio',
      valor: formatarMoeda(resumo.ticketMedio),
      detalhe: 'Por operação fechada',
      detalheCor: 'text-slate-400',
      descricao: 'Valor médio por venda',
      icone: TrendingDown,
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
            <div className="min-w-0 flex-1">
              <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">{rotulo}</p>
              <p className={`mt-1.5 text-[22px] font-black leading-none ${corValor}`}>{valor}</p>
              <p className={`mt-1 text-[11px] font-semibold ${detalheCor}`}>{detalhe}</p>
              <p className="mt-2 text-[10px] leading-snug text-slate-600">{descricao}</p>
            </div>
            <div className={`shrink-0 rounded-lg p-2 ${corIcone}`}>
              <Icone className="h-5 w-5" />
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}
