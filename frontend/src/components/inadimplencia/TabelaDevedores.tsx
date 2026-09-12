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

export function TabelaDevedores({ registros }: Props) {
  const ordenados = [...registros].sort((a, b) => b.valor - a.valor)

  return (
    <div className="glass-card p-4">
      <p className="section-label">Contas em atraso</p>
      <p className="section-desc">Detalhamento por cliente e probabilidade de recuperação</p>

      <div className="mt-4 overflow-x-auto">
        <table className="w-full min-w-[640px] text-left">
          <thead>
            <tr className="border-b border-rl-border text-[11px] font-bold uppercase tracking-wide text-rl-heading">
              <th className="pb-2 pr-3">Cliente</th>
              <th className="pb-2 pr-3">Valor</th>
              <th className="pb-2 pr-3">Dias</th>
              <th className="pb-2 pr-3">Recuperação</th>
              <th className="pb-2 pr-3">Perda proj.</th>
              <th className="pb-2">Risco</th>
            </tr>
          </thead>
          <tbody>
            {ordenados.map((r) => {
              const severidade = classificarSeveridade(r)
              return (
                <tr key={r.id} className="border-b border-rl-border/50 text-xs last:border-0">
                  <td className="py-2.5 pr-3 font-semibold text-rl-heading">{r.nomeCliente}</td>
                  <td className="py-2.5 pr-3 font-bold text-red-500">{formatarMoeda(r.valor)}</td>
                  <td className="py-2.5 pr-3 text-rl-body">{r.diasEmAtraso}d</td>
                  <td className="py-2.5 pr-3 text-emerald-400">
                    {formatarPercentual(pctProbabilidade(r.probabilidadeRecuperacao), 0)}
                  </td>
                  <td className="py-2.5 pr-3 font-semibold text-red-400">
                    {formatarMoeda(r.perdaProjetada)}
                  </td>
                  <td className="py-2.5">
                    <span
                      className={`inline-flex rounded-full border px-2 py-0.5 text-[9px] font-bold ${corBadgeSeveridade(severidade)}`}
                    >
                      {rotuloSeveridade(severidade)}
                    </span>
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}
