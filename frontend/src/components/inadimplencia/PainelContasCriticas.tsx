import { AlertCircle, Phone, Scale } from 'lucide-react'
import { formatarMoeda, formatarPercentual } from '@/utils/format'
import {
  classificarSeveridade,
  corBadgeSeveridade,
  pctProbabilidade,
  rotuloSeveridade,
} from '@/utils/inadimplencia'
import type { RegistroInadimplencia } from '@/types/inadimplencia'

interface Props {
  registros: RegistroInadimplencia[]
}

const ACOES: Record<string, string> = {
  critico: 'Acionar jurídico e renegociar imediatamente',
  alto: 'Contato direto do gestor comercial',
  medio: 'Follow-up intensivo e proposta de parcelamento',
  baixo: 'Lembrete automático e negociação padrão',
}

export function PainelContasCriticas({ registros }: Props) {
  const criticas = [...registros]
    .map((r) => ({ ...r, severidade: classificarSeveridade(r) }))
    .filter((r) => r.severidade === 'critico' || r.severidade === 'alto')
    .sort((a, b) => b.perdaProjetada - a.perdaProjetada)
    .slice(0, 4)

  const perdaTotal = criticas.reduce((s, r) => s + r.perdaProjetada, 0)

  return (
    <div className="glass-card p-4">
      <div className="flex items-start justify-between gap-2">
        <div>
          <p className="section-label">Contas em risco</p>
          <p className="section-desc">Prioridade de cobrança e recuperação</p>
        </div>
        {criticas.length > 0 && (
          <span className="shrink-0 rounded-full border border-red-500/30 bg-red-500/10 px-2 py-0.5 text-[9px] font-bold text-red-400">
            {formatarMoeda(perdaTotal)} em risco
          </span>
        )}
      </div>

      <div className="mt-4 space-y-3">
        {criticas.length === 0 ? (
          <p className="rounded-lg border border-emerald-500/20 bg-emerald-500/5 p-4 text-[11px] text-emerald-400">
            Nenhuma conta em nível crítico ou alto no momento.
          </p>
        ) : (
          criticas.map((r) => (
            <div
              key={r.id}
              className="rounded-lg border border-rl-border/80 bg-rl-surface p-3"
            >
              <div className="flex items-start justify-between gap-2">
                <div className="min-w-0">
                  <p className="truncate text-[13px] font-bold text-rl-heading">{r.nomeCliente}</p>
                  <p className="mt-0.5 text-xs font-medium text-rl-body">
                    {r.diasEmAtraso} dias · {formatarPercentual(pctProbabilidade(r.probabilidadeRecuperacao), 0)} recuperação
                  </p>
                </div>
                <span
                  className={`shrink-0 rounded-full border px-2 py-0.5 text-[9px] font-bold ${corBadgeSeveridade(r.severidade)}`}
                >
                  {rotuloSeveridade(r.severidade)}
                </span>
              </div>

              <div className="mt-2 flex items-center justify-between text-[11px]">
                <span className="font-bold text-red-400">{formatarMoeda(r.valor)}</span>
                <span className="text-rl-body">
                  Perda: <span className="font-semibold text-red-500">{formatarMoeda(r.perdaProjetada)}</span>
                </span>
              </div>

              <p className="mt-2 flex items-center gap-1.5 text-xs font-medium text-amber-400">
                {r.severidade === 'critico' ? (
                  <Scale className="h-3 w-3 shrink-0" />
                ) : (
                  <Phone className="h-3 w-3 shrink-0" />
                )}
                {ACOES[r.severidade]}
              </p>
            </div>
          ))
        )}
      </div>

      {criticas.length > 0 && (
        <div className="mt-4 flex items-start gap-2 rounded-lg border border-amber-500/20 bg-amber-500/5 p-3">
          <AlertCircle className="mt-0.5 h-4 w-4 shrink-0 text-amber-400" />
          <p className="text-xs font-medium leading-relaxed text-amber-200/80">
            Foque nas contas com maior perda projetada e menor probabilidade de recuperação.
            Cada dia de atraso reduz a chance de recebimento.
          </p>
        </div>
      )}
    </div>
  )
}
