import {
  Area,
  AreaChart,
  CartesianGrid,
  Line,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { formatarMoeda } from '@/utils/format'
import { useTheme } from '@/hooks/useTheme'
import { corGridGrafico, estilosTooltipGrafico } from '@/utils/estilosGrafico'
import type { PontoSerieTemporal } from '@/types/faturamento'

interface Props {
  serie: PontoSerieTemporal[]
}

export function GraficoFaturamentoDiario({ serie }: Props) {
  const { tema } = useTheme()
  const tooltipStyle = estilosTooltipGrafico(tema)
  const gridColor = corGridGrafico(tema)
  const dados = serie.slice(-30)

  return (
    <div className="glass-card flex h-full flex-col p-4">
      <div className="mb-4 flex flex-wrap items-start justify-between gap-2">
        <div>
          <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">
            Evolução diária
          </p>
          <p className="text-[11px] text-slate-600">Faturamento vs meta diária no período</p>
        </div>
        <div className="flex items-center gap-4 text-[10px]">
          <span className="flex items-center gap-1.5 text-slate-500">
            <span className="h-0.5 w-4 rounded bg-emerald-500" />
            Faturamento
          </span>
          <span className="flex items-center gap-1.5 text-slate-500">
            <span className="h-0.5 w-4 rounded border-t border-dashed border-slate-500" />
            Meta/dia
          </span>
        </div>
      </div>

      <div className="min-h-[260px] flex-1">
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={dados} margin={{ top: 8, right: 8, left: -16, bottom: 0 }}>
            <defs>
              <linearGradient id="fatGrad" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="#22c55e" stopOpacity={0.25} />
                <stop offset="100%" stopColor="#22c55e" stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid stroke={gridColor} strokeDasharray="3 3" vertical={false} />
            <XAxis
              dataKey="rotulo"
              tick={{ fill: '#475569', fontSize: 9 }}
              axisLine={false}
              tickLine={false}
              interval="preserveStartEnd"
            />
            <YAxis
              tick={{ fill: '#475569', fontSize: 9 }}
              axisLine={false}
              tickLine={false}
              tickFormatter={(v) => `${(Number(v) / 1000).toFixed(0)}k`}
            />
            <Tooltip
              contentStyle={tooltipStyle}
              formatter={(v, name) => [
                formatarMoeda(Number(v ?? 0)),
                name === 'valor' ? 'Faturamento' : 'Meta',
              ]}
            />
            <Area
              type="monotone"
              dataKey="valor"
              stroke="#22c55e"
              strokeWidth={2}
              fill="url(#fatGrad)"
              dot={false}
              activeDot={{ r: 4, fill: '#22c55e' }}
            />
            <Line
              type="monotone"
              dataKey="meta"
              stroke="#64748b"
              strokeWidth={1.5}
              strokeDasharray="4 4"
              dot={false}
            />
          </AreaChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}
