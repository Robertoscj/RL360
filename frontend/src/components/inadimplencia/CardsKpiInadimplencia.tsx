import {
  AlertTriangle,
  CalendarClock,
  ShieldAlert,
  TrendingDown,
  Users,
  Wallet,
} from 'lucide-react'
import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { ResumoInadimplencia } from '@/types/inadimplencia'

interface Props {
  resumo: ResumoInadimplencia
  riscoProximos30Dias?: number
}

export function CardsKpiInadimplencia({ resumo, riscoProximos30Dias }: Props) {
  const cards = [
    {
      rotulo: 'Total em Atraso',
      valor: formatarMoeda(resumo.totalEmAtraso),
      detalhe: `${resumo.quantidadeContas} conta${resumo.quantidadeContas !== 1 ? 's' : ''} aberta${resumo.quantidadeContas !== 1 ? 's' : ''}`,
      detalheCor: 'text-red-400',
      descricao: 'Valor total devido',
      icone: Wallet,
      accent: 'kpi-card-accent-red',
      corIcone: 'text-red-400 bg-red-500/10',
      corValor: 'text-red-400',
    },
    {
      rotulo: 'Perda Projetada',
      valor: formatarMoeda(resumo.perdaProjetada),
      detalhe: resumo.totalEmAtraso > 0
        ? formatarPercentual((resumo.perdaProjetada / resumo.totalEmAtraso) * 100, 1) + ' do total'
        : '—',
      detalheCor: 'text-red-400',
      descricao: 'Risco de perda definitiva',
      icone: TrendingDown,
      accent: 'kpi-card-accent-red',
      corIcone: 'text-red-400 bg-red-500/10',
      corValor: 'text-red-400',
    },
    {
      rotulo: 'Valor Recuperável',
      valor: formatarMoeda(resumo.valorRecuperavel),
      detalhe: formatarPercentual(pctProb(resumo.taxaRecuperacaoMedia), 0) + ' prob. média',
      detalheCor: 'text-emerald-400',
      descricao: 'Potencial de recuperação',
      icone: ShieldAlert,
      accent: 'kpi-card-accent-green',
      corIcone: 'text-emerald-400 bg-emerald-500/10',
      corValor: 'text-emerald-400',
    },
    {
      rotulo: 'Contas Críticas',
      valor: String(resumo.contasCriticas),
      detalhe: resumo.quantidadeContas > 0
        ? `${Math.round((resumo.contasCriticas / resumo.quantidadeContas) * 100)}% do total`
        : 'Nenhuma',
      detalheCor: resumo.contasCriticas > 0 ? 'text-red-400' : 'text-emerald-400',
      descricao: 'Atraso >60d ou baixa recuperação',
      icone: AlertTriangle,
      accent: 'kpi-card-accent-amber',
      corIcone: 'text-amber-400 bg-amber-500/10',
      corValor: resumo.contasCriticas > 0 ? 'text-red-400' : 'text-rl-heading',
    },
    {
      rotulo: 'Dias Médios',
      valor: `${Math.round(resumo.diasMedios)}d`,
      detalhe: 'Tempo médio em atraso',
      detalheCor: resumo.diasMedios > 45 ? 'text-red-400' : 'text-amber-400',
      descricao: 'Aging médio da carteira',
      icone: CalendarClock,
      accent: 'kpi-card-accent-amber',
      corIcone: 'text-amber-400 bg-amber-500/10',
      corValor: 'text-rl-heading',
    },
    {
      rotulo: 'Risco 30 Dias',
      valor: riscoProximos30Dias != null ? formatarMoeda(riscoProximos30Dias) : '—',
      detalhe: resumo.taxaInadimplenciaSobreFaturamento != null
        ? formatarPercentual(resumo.taxaInadimplenciaSobreFaturamento, 1) + ' do faturamento'
        : 'Impacto no radar',
      detalheCor: 'text-red-400',
      descricao: 'Perda estimada se nada for feito',
      icone: Users,
      accent: 'kpi-card-accent-red',
      corIcone: 'text-red-400 bg-red-500/10',
      corValor: 'text-red-400',
    },
  ]

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
      {cards.map(({ rotulo, valor, detalhe, detalheCor, descricao, icone: Icone, accent, corIcone, corValor }) => (
        <div key={rotulo} className={`kpi-card ${accent}`}>
          <div className="flex items-start justify-between">
            <div className="min-w-0 flex-1">
              <p className="section-label">{rotulo}</p>
              <p className={`mt-1.5 text-[22px] font-black leading-none ${corValor}`}>{valor}</p>
              <p className={`mt-1 text-xs font-semibold ${detalheCor}`}>{detalhe}</p>
              <p className="kpi-desc">{descricao}</p>
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

function pctProb(valor: number) {
  return valor <= 1 ? valor * 100 : valor
}
