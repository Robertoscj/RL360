import { Bar, BarChart, Cell, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'
import { formatarMoeda } from '@/utils/format'
import { useTheme } from '@/hooks/useTheme'
import { estilosTooltipGrafico } from '@/utils/estilosGrafico'
import type { Venda } from '@/types/faturamento'

interface Props {
  vendas: Venda[]
}

const CORES = ['#22c55e', '#14b8a6', '#3b82f6', '#8b5cf6', '#f59e0b', '#64748b']

export function FaturamentoPorCanal({ vendas }: Props) {
  const { tema } = useTheme()
  const tooltipStyle = estilosTooltipGrafico(tema)
  const porCanal = Object.entries(
    vendas.reduce<Record<string, number>>((acc, v) => {
      acc[v.canal] = (acc[v.canal] ?? 0) + v.valor
      return acc
    }, {}),
  )
    .map(([canal, valor]) => ({ canal, valor }))
    .sort((a, b) => b.valor - a.valor)

  const total = porCanal.reduce((s, c) => s + c.valor, 0)

  return (
    <div className="glass-card flex h-full flex-col p-4">
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Faturamento por canal</p>
      <p className="text-[11px] text-slate-600">Distribuição da receita no mês</p>

      <div className="mt-4 h-[200px]">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={porCanal} layout="vertical" margin={{ left: 0, right: 16 }}>
            <XAxis type="number" hide />
            <YAxis
              type="category"
              dataKey="canal"
              width={88}
              tick={{ fill: '#94a3b8', fontSize: 10 }}
              axisLine={false}
              tickLine={false}
            />
            <Tooltip
              contentStyle={tooltipStyle}
              formatter={(v) => [formatarMoeda(Number(v ?? 0)), 'Receita']}
            />
            <Bar dataKey="valor" radius={[0, 4, 4, 0]} barSize={14}>
              {porCanal.map((_, i) => (
                <Cell key={i} fill={CORES[i % CORES.length]} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>

      <div className="mt-3 space-y-2 border-t border-rl-border pt-3">
        {porCanal.map(({ canal, valor }, i) => (
          <div key={canal} className="flex items-center justify-between text-[11px]">
            <span className="flex items-center gap-2 text-slate-400">
              <span className="h-2 w-2 rounded-full" style={{ background: CORES[i % CORES.length] }} />
              {canal}
            </span>
            <span className="font-bold text-slate-200">
              {formatarMoeda(valor)}
              <span className="ml-1 text-[9px] font-normal text-slate-600">
                ({total > 0 ? ((valor / total) * 100).toFixed(0) : 0}%)
              </span>
            </span>
          </div>
        ))}
      </div>
    </div>
  )
}
