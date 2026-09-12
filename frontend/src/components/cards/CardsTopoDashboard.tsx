import { Link } from 'react-router-dom'
import { AlertTriangle, ChevronRight, HeartPulse, Target, Zap } from 'lucide-react'
import { MoedaAnimada, NumeroAnimado, PercentualAnimado } from '@/components/ui/ValorAnimado'
import { badgeStatusSaude, metaSaudePercentual } from '@/utils/dashboardMetricas'
import { formatarPercentual } from '@/utils/format'
import type { CardsTopo } from '@/types/dashboard'

interface CardsTopoProps {
  cards: CardsTopo
  versaoAnimacao?: number
}

export function CardsTopoDashboard({ cards, versaoAnimacao }: CardsTopoProps) {
  const badge = badgeStatusSaude(cards.statusSaude, cards.saudeEmpresaPercentual)
  const metaSaude = metaSaudePercentual(cards.saudeEmpresaPercentual)

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-4">
      <div className="kpi-card kpi-card-accent-red">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <p className="text-[10px] font-bold uppercase tracking-wide text-red-400/90">
              Atenção: Risco de Perda
            </p>
            <p className="mt-1.5 text-[26px] font-black leading-none text-red-400">
              <MoedaAnimada valor={cards.riscoProximos30Dias} reiniciarChave={versaoAnimacao} />
            </p>
            <p className="mt-2 text-[11px] leading-snug text-slate-500">
              Principais ameaças identificadas nos próximos 30 dias
            </p>
            <Link
              to="/inadimplencia"
              className="mt-3 inline-flex items-center gap-0.5 text-[11px] font-semibold text-red-400 hover:text-red-300"
            >
              Ver detalhes <ChevronRight className="h-3 w-3" />
            </Link>
          </div>
          <div className="rounded-lg bg-red-500/10 p-2 text-red-400">
            <AlertTriangle className="h-5 w-5" />
          </div>
        </div>
      </div>

      <div className="kpi-card kpi-card-accent-green">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <p className="text-[10px] font-bold uppercase tracking-wide text-emerald-400/90">
              Oportunidade Identificada
            </p>
            <p className="mt-1.5 text-[26px] font-black leading-none text-emerald-400">
              <MoedaAnimada valor={cards.valorOportunidade} reiniciarChave={versaoAnimacao} />
            </p>
            <p className="mt-2 text-[11px] leading-snug text-slate-500">
              Potencial de ganho com ações recomendadas
            </p>
            <Link
              to="/vendas"
              className="mt-3 inline-flex items-center gap-0.5 text-[11px] font-semibold text-emerald-400 hover:text-emerald-300"
            >
              Ver oportunidades <ChevronRight className="h-3 w-3" />
            </Link>
          </div>
          <div className="rounded-lg bg-emerald-500/10 p-2 text-emerald-400">
            <Target className="h-5 w-5" />
          </div>
        </div>
      </div>

      <div className="kpi-card kpi-card-accent-amber">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <p className="text-[10px] font-bold uppercase tracking-wide text-amber-400/90">
              Gargalos Críticos
            </p>
            <p className="mt-1.5 text-[26px] font-black leading-none text-amber-400">
              <NumeroAnimado valor={cards.gargalosCriticos} reiniciarChave={versaoAnimacao} />
            </p>
            <p className="mt-2 text-[11px] leading-snug text-slate-500">
              Pontos críticos que reduzem sua margem
            </p>
            <Link
              to="/gargalos"
              className="mt-3 inline-flex items-center gap-0.5 text-[11px] font-semibold text-amber-400 hover:text-amber-300"
            >
              Ver gargalos <ChevronRight className="h-3 w-3" />
            </Link>
          </div>
          <div className="rounded-lg bg-amber-500/10 p-2 text-amber-400">
            <Zap className="h-5 w-5" />
          </div>
        </div>
      </div>

      <div className="kpi-card kpi-card-accent-blue">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <p className="text-[10px] font-bold uppercase tracking-wide text-blue-400/90">
              Saúde da Empresa
            </p>
            <div className="mt-1.5 flex items-center gap-2">
              <p className="text-[26px] font-black leading-none text-rl-heading">
                <PercentualAnimado valor={cards.saudeEmpresaPercentual} reiniciarChave={versaoAnimacao} />
              </p>
              <span className={`rounded border px-1.5 py-0.5 text-[9px] font-bold uppercase ${badge.classe}`}>
                {badge.rotulo}
              </span>
            </div>
            <div className="mt-3">
              <div className="relative h-2.5 overflow-hidden rounded-full">
                <div className="absolute inset-0 bg-gradient-to-r from-red-500 via-amber-400 to-emerald-500" />
                <div
                  className="absolute top-1/2 h-4 w-0.5 -translate-y-1/2 bg-white shadow-md transition-all duration-700"
                  style={{ left: `${cards.saudeEmpresaPercentual}%`, transform: 'translate(-50%, -50%)' }}
                />
              </div>
              <div className="mt-1 flex justify-between text-[9px] text-slate-600">
                <span>0%</span>
                <span className="font-medium text-slate-400">Meta: {formatarPercentual(metaSaude)}</span>
                <span>100%</span>
              </div>
            </div>
          </div>
          <div className="rounded-lg bg-blue-500/10 p-2 text-blue-400">
            <HeartPulse className="h-5 w-5" />
          </div>
        </div>
      </div>
    </div>
  )
}
