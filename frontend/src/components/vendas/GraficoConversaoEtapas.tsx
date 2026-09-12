import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { formatarPercentual } from '@/utils/format'
import { useTheme } from '@/hooks/useTheme'
import { corGridGrafico, estilosTooltipGrafico } from '@/utils/estilosGrafico'
import type { EtapaFunil } from '@/types/vendas'

interface Props {
  etapas: EtapaFunil[]
}

function pctTaxa(valor: number) {
  return valor <= 1 ? valor * 100 : valor
}

export function GraficoConversaoEtapas({ etapas }: Props) {
  const { tema } = useTheme()
  const tooltipStyle = estilosTooltipGrafico(tema)
  const gridColor = corGridGrafico(tema)
  const dados = [...etapas]
    .sort((a, b) => a.ordem - b.ordem)
    .map((e) => ({
      nome: e.nome,
      atual: pctTaxa(e.taxaConversao),
      meta: pctTaxa(e.taxaConversaoBase),
      queda: pctTaxa(e.taxaConversaoBase) - pctTaxa(e.taxaConversao),
    }))

  return (
    <div className="glass-card p-4">
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Conversão por etapa</p>
      <p className="text-[11px] text-slate-600">Taxa atual vs meta de conversão</p>

      <div className="mt-4 h-[220px]">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={dados} margin={{ top: 8, right: 8, left: -20, bottom: 0 }}>
            <CartesianGrid stroke={gridColor} strokeDasharray="3 3" vertical={false} />
            <XAxis
              dataKey="nome"
              tick={{ fill: '#64748b', fontSize: 9 }}
              axisLine={false}
              tickLine={false}
            />
            <YAxis
              tick={{ fill: '#64748b', fontSize: 9 }}
              axisLine={false}
              tickLine={false}
              tickFormatter={(v) => `${v}%`}
            />
            <Tooltip
              contentStyle={tooltipStyle}
              formatter={(v, name) => [
                formatarPercentual(Number(v ?? 0), 1),
                name === 'atual' ? 'Atual' : 'Meta',
              ]}
            />
            <Bar dataKey="meta" fill="#334155" radius={[4, 4, 0, 0]} barSize={16} name="meta" />
            <Bar dataKey="atual" radius={[4, 4, 0, 0]} barSize={16} name="atual">
              {dados.map((d) => (
                <Cell key={d.nome} fill={d.queda > 3 ? '#f59e0b' : '#22c55e'} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>

      <div className="mt-3 flex gap-4 text-[10px] text-slate-500">
        <span className="flex items-center gap-1.5">
          <span className="h-2 w-3 rounded-sm bg-emerald-500" /> Atual (saudável)
        </span>
        <span className="flex items-center gap-1.5">
          <span className="h-2 w-3 rounded-sm bg-amber-500" /> Abaixo da meta
        </span>
        <span className="flex items-center gap-1.5">
          <span className="h-2 w-3 rounded-sm bg-slate-600" /> Meta
        </span>
      </div>
    </div>
  )
}
