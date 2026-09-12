import { formatarMoeda } from '@/utils/format'
import type { Venda } from '@/types/faturamento'

interface Props {
  vendas: Venda[]
}

function formatarData(iso: string) {
  return new Date(iso).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })
}

export function TabelaVendasRecentes({ vendas }: Props) {
  const ordenadas = [...vendas].sort(
    (a, b) => new Date(b.fechadaEmUtc).getTime() - new Date(a.fechadaEmUtc).getTime(),
  )

  return (
    <div className="glass-card flex h-full flex-col p-4">
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Vendas recentes</p>
      <p className="text-[11px] text-slate-600">Últimas operações registradas</p>

      {ordenadas.length === 0 ? (
        <p className="mt-8 text-center text-[12px] text-slate-500">Nenhuma venda no período.</p>
      ) : (
      <div className="mt-4 overflow-x-auto">
        <table className="w-full text-left">
          <thead>
            <tr className="border-b border-rl-border text-[9px] font-bold uppercase tracking-wide text-slate-600">
              <th className="pb-2 pr-3">Canal</th>
              <th className="pb-2 pr-3">Valor</th>
              <th className="pb-2 pr-3">Margem</th>
              <th className="pb-2 pr-3">Data</th>
              <th className="pb-2">Tipo</th>
            </tr>
          </thead>
          <tbody>
            {ordenadas.map((v) => (
              <tr key={v.id} className="border-b border-rl-border/50 text-[11px] last:border-0">
                <td className="py-2.5 pr-3 font-medium text-slate-300">{v.canal}</td>
                <td className="py-2.5 pr-3 font-bold text-emerald-400">{formatarMoeda(v.valor)}</td>
                <td className="py-2.5 pr-3 text-slate-400">{formatarMoeda(v.margem)}</td>
                <td className="py-2.5 pr-3 text-slate-500">{formatarData(v.fechadaEmUtc)}</td>
                <td className="py-2.5">
                  {v.clienteNovo ? (
                    <span className="rounded-full bg-emerald-500/15 px-2 py-0.5 text-[9px] font-bold text-emerald-400">
                      Novo
                    </span>
                  ) : (
                    <span className="rounded-full bg-slate-500/15 px-2 py-0.5 text-[9px] font-bold text-slate-400">
                      Recorrência
                    </span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      )}
    </div>
  )
}
