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
import { formatarMoeda } from '@/utils/format'
import { useTheme } from '@/hooks/useTheme'
import { corGridGrafico, estilosTooltipGrafico } from '@/utils/estilosGrafico'
import type { FaixaAtraso } from '@/types/inadimplencia'

interface Props {
  faixas: FaixaAtraso[]
}

export function GraficoFaixasAtraso({ faixas }: Props) {
  const { tema } = useTheme()
  const tooltipStyle = estilosTooltipGrafico(tema)
  const gridColor = corGridGrafico(tema)
  const total = faixas.reduce((s, f) => s + f.valor, 0)

  return (
    <div className="glass-card p-4">
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Aging da carteira</p>
      <p className="text-[11px] text-slate-600">Distribuição por faixa de atraso</p>

      <div className="mt-4 h-[220px]">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={faixas} margin={{ top: 8, right: 8, left: -10, bottom: 0 }}>
            <CartesianGrid stroke={gridColor} strokeDasharray="3 3" vertical={false} />
            <XAxis
              dataKey="rotulo"
              tick={{ fill: '#64748b', fontSize: 9 }}
              axisLine={false}
              tickLine={false}
            />
            <YAxis
              tick={{ fill: '#64748b', fontSize: 9 }}
              axisLine={false}
              tickLine={false}
              tickFormatter={(v) => `${(v / 1000).toFixed(0)}k`}
            />
            <Tooltip
              contentStyle={tooltipStyle}
              formatter={(v, _name, item) => {
                const payload = item.payload as FaixaAtraso
                const pct = total > 0 ? ((Number(v) / total) * 100).toFixed(1) : '0'
                return [
                  `${formatarMoeda(Number(v))} · ${payload.quantidade} conta${payload.quantidade !== 1 ? 's' : ''} (${pct}%)`,
                  'Valor',
                ]
              }}
            />
            <Bar dataKey="valor" radius={[4, 4, 0, 0]} barSize={36}>
              {faixas.map((f) => (
                <Cell key={f.rotulo} fill={f.cor} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>

      <div className="mt-3 flex flex-wrap gap-3 text-[10px] text-slate-500">
        {faixas.map((f) => (
          <span key={f.rotulo} className="flex items-center gap-1.5">
            <span className="h-2 w-3 rounded-sm" style={{ background: f.cor }} />
            {f.rotulo} ({f.quantidade})
          </span>
        ))}
      </div>
    </div>
  )
}
