import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { formatarMoeda } from '@/utils/format'
import { useTheme } from '@/hooks/useTheme'
import { corGridGrafico, corTickGrafico, estilosTooltipGrafico } from '@/utils/estilosGrafico'
import type { RegistroInadimplencia } from '@/types/inadimplencia'

interface Props {
  registros: RegistroInadimplencia[]
}

export function GraficoRecuperacaoVsPerda({ registros }: Props) {
  const { tema } = useTheme()
  const tooltipStyle = estilosTooltipGrafico(tema)
  const gridColor = corGridGrafico(tema)
  const tickColor = corTickGrafico(tema)
  const dados = [...registros]
    .sort((a, b) => b.valor - a.valor)
    .map((r) => ({
      nome: r.nomeCliente.length > 18 ? `${r.nomeCliente.slice(0, 16)}…` : r.nomeCliente,
      recuperavel: r.valor - r.perdaProjetada,
      perda: r.perdaProjetada,
    }))

  return (
    <div className="glass-card p-4">
      <p className="section-label">Recuperação vs perda</p>
      <p className="section-desc">Valor recuperável e perda projetada por cliente</p>

      <div className="mt-4 h-[220px]">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart
            data={dados}
            layout="vertical"
            margin={{ top: 4, right: 8, left: 4, bottom: 0 }}
          >
            <CartesianGrid stroke={gridColor} strokeDasharray="3 3" horizontal={false} />
            <XAxis
              type="number"
              tick={{ fill: tickColor, fontSize: 9 }}
              axisLine={false}
              tickLine={false}
              tickFormatter={(v) => `${(v / 1000).toFixed(0)}k`}
            />
            <YAxis
              type="category"
              dataKey="nome"
              width={100}
              tick={{ fill: tickColor, fontSize: 9 }}
              axisLine={false}
              tickLine={false}
            />
            <Tooltip
              contentStyle={tooltipStyle}
              formatter={(v, name) => [
                formatarMoeda(Number(v)),
                name === 'recuperavel' ? 'Recuperável' : 'Perda projetada',
              ]}
            />
            <Bar dataKey="recuperavel" stackId="a" fill="#22c55e" radius={[0, 0, 0, 0]} barSize={14} />
            <Bar dataKey="perda" stackId="a" fill="#ef4444" radius={[0, 4, 4, 0]} barSize={14} />
          </BarChart>
        </ResponsiveContainer>
      </div>

      <div className="mt-3 flex gap-4 text-xs font-medium text-rl-body">
        <span className="flex items-center gap-1.5">
          <span className="h-2 w-3 rounded-sm bg-emerald-500" /> Recuperável
        </span>
        <span className="flex items-center gap-1.5">
          <span className="h-2 w-3 rounded-sm bg-red-500" /> Perda projetada
        </span>
      </div>
    </div>
  )
}
