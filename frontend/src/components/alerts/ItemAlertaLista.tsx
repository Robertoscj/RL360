import { ChevronRight } from 'lucide-react'
import { corImpacto, formatarMoeda } from '@/utils/format'
import { estiloSeveridadeAlerta, tempoRelativoAlerta } from '@/utils/alertas'
import type { Alerta } from '@/types/alerta'

interface Props {
  alerta: Alerta
  compacto?: boolean
  naoLido?: boolean
  onClick?: () => void
}

export function ItemAlertaLista({ alerta, compacto = false, naoLido = false, onClick }: Props) {
  const estilo = estiloSeveridadeAlerta(alerta.severidade)

  return (
    <button
      type="button"
      onClick={onClick}
      className={`flex w-full items-start gap-3 border-l-[3px] px-3 py-2.5 text-left transition hover:bg-rl-card-hover ${estilo.borda} ${estilo.fundo} ${naoLido ? 'bg-rl-card/80' : ''}`}
    >
      <div className="min-w-0 flex-1">
        <div className="flex items-start justify-between gap-2">
          <p className={`text-[12px] font-bold leading-snug text-rl-heading ${compacto ? 'line-clamp-1' : ''}`}>
            {alerta.titulo}
          </p>
          {naoLido && (
            <span className="mt-1 h-2 w-2 shrink-0 rounded-full bg-red-500" aria-hidden />
          )}
        </div>
        {!compacto && (
          <p className="mt-0.5 line-clamp-2 text-[11px] leading-snug text-rl-muted">{alerta.mensagem}</p>
        )}
        <div className="mt-1.5 flex flex-wrap items-center gap-x-2 gap-y-0.5 text-[10px]">
          <span className={`font-bold ${corImpacto(alerta.impactoFinanceiro)}`}>
            {alerta.impactoFinanceiro >= 0 ? '+' : ''}
            {formatarMoeda(alerta.impactoFinanceiro)}
          </span>
          <span className="text-rl-subtle">·</span>
          <span className={`font-semibold uppercase tracking-wide ${estilo.rotulo}`}>
            {alerta.severidade}
          </span>
          <span className="text-rl-subtle">·</span>
          <span className="text-rl-subtle">{tempoRelativoAlerta(alerta.criadoEmUtc)}</span>
        </div>
      </div>
      <ChevronRight className="mt-0.5 h-4 w-4 shrink-0 text-rl-muted" />
    </button>
  )
}
