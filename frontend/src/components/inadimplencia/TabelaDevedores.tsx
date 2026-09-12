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
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Contas em atraso</p>
      <p className="text-[11px] text-slate-600">Detalhamento por cliente e probabilidade de recuperação</p>

      <div className="mt-4 overflow-x-auto">
        <table className="w-full min-w-[640px] text-left">
          <thead>
            <tr className="border-b border-rl-border text-[9px] font-bold uppercase tracking-wide text-slate-600">
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
                <tr key={r.id} className="border-b border-rl-border/50 text-[11px] last:border-0">
                  <td className="py-2.5 pr-3 font-medium text-slate-300">{r.nomeCliente}</td>
                  <td className="py-2.5 pr-3 font-bold text-red-400">{formatarMoeda(r.valor)}</td>
                  <td className="py-2.5 pr-3 text-slate-400">{r.diasEmAtraso}d</td>
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
