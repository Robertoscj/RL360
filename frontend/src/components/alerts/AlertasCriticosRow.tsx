import {
  AlertTriangle,
  Clock,
  TrendingDown,
  TrendingUp,
  Zap,
} from 'lucide-react'
import { formatarMoeda, corImpacto } from '@/utils/format'
import type { AlertaCritico } from '@/types/dashboard'

interface AlertasCriticosRowProps {
  alertas: AlertaCritico[]
}

const icones = [AlertTriangle, Clock, TrendingDown, TrendingUp, Zap]

const descricoes: Record<string, string> = {
  'Inadimplência acima do normal': 'Taxa 18% acima da média histórica',
  'Aprovação muito lenta': 'Tempo médio de 3h45m por proposta',
  'Queda na conversão': 'Conversão caiu 3,2pp este mês',
  'Oportunidade na região Sul': 'Pipeline com alto potencial',
  'Produto em alta': 'Cross-sell com 70% de conversão',
}

function corBorda(alerta: AlertaCritico) {
  if (alerta.impactoFinanceiro >= 0) return 'border-t-emerald-500 bg-emerald-500/[0.04]'
  if (alerta.titulo.toLowerCase().includes('inadimplência')) return 'border-t-red-500 bg-red-500/[0.04]'
  return 'border-t-amber-500 bg-amber-500/[0.04]'
}

export function AlertasCriticosRow({ alertas }: AlertasCriticosRowProps) {
  const idPiorAlerta = [...alertas]
    .filter((alerta) => alerta.impactoFinanceiro < 0)
    .sort((a, b) => a.impactoFinanceiro - b.impactoFinanceiro)[0]?.id

  return (
    <div>
      <p className="section-label mb-3">
        Alertas Críticos <span className="font-semibold text-rl-body">({alertas.length})</span>
      </p>
      <div className="hide-scrollbar flex gap-3 overflow-x-auto pb-1">
        {alertas.map((alerta, i) => {
          const Icone = icones[i % icones.length]
          const emPiora = alerta.id === idPiorAlerta
          return (
            <div
              key={alerta.id}
              className={`glass-card min-w-[200px] flex-shrink-0 border-t-[3px] p-3.5 ${corBorda(alerta)}`}
            >
              <div className="flex items-start gap-2.5">
                <div className={`mt-0.5 rounded-md p-1.5 ${corImpacto(alerta.impactoFinanceiro)} bg-rl-surface ${emPiora ? 'rl-icone-alerta' : ''}`}>
                  <Icone className="h-3.5 w-3.5" />
                </div>
                <div className="min-w-0">
                  <p className="text-[13px] font-bold leading-tight text-rl-heading">{alerta.titulo}</p>
                  <p className="mt-1 text-xs font-medium leading-snug text-rl-body">
                    {descricoes[alerta.titulo] ?? alerta.severidade}
                  </p>
                  <p className={`mt-2 text-sm font-black ${corImpacto(alerta.impactoFinanceiro)}`}>
                    Impacto: {alerta.impactoFinanceiro >= 0 ? '+' : ''}
                    {formatarMoeda(alerta.impactoFinanceiro)}
                  </p>
                  <p className="mt-1 text-[11px] font-medium text-rl-body">há {1 + i * 2}h</p>
                </div>
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}
