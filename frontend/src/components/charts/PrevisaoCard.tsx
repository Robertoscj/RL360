import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
  BarChart,
  Bar,
  Cell,
} from 'recharts'
import { MoedaAnimada } from '@/components/ui/ValorAnimado'
import { useTheme } from '@/hooks/useTheme'
import { formatarMoeda } from '@/utils/format'
import { corGridGrafico, estilosTooltipGrafico } from '@/utils/estilosGrafico'
import type { PrevisaoResultado } from '@/types/dashboard'

interface PrevisaoCardProps {
  previsao: PrevisaoResultado
}

export function PrevisaoCard({ previsao }: PrevisaoCardProps) {
  const { tema } = useTheme()
  const tooltipStyle = estilosTooltipGrafico(tema)
  const gridColor = corGridGrafico(tema)
  const dados = previsao.serie.slice(-12)
  const abaixoMeta = previsao.percentualAbaixoMeta > 0
  const corValor = abaixoMeta ? 'text-red-400' : 'text-emerald-400'
  const comparativo = [
    { nome: 'Meta', valor: previsao.meta, cor: '#475569' },
    { nome: 'Estimado', valor: previsao.cenarioMaisProvavel, cor: abaixoMeta ? '#ef4444' : '#22c55e' },
  ]

  return (
    <div className="glass-card flex h-full min-h-[460px] flex-col p-4">
      <p className="text-[10px] font-bold uppercase tracking-wide text-slate-500">Previsão de Resultado</p>
      <p className="text-[11px] text-slate-600">Próximos {previsao.dias} dias</p>

      <p className={`mt-4 text-[22px] font-black ${corValor}`}>
        <MoedaAnimada valor={previsao.cenarioMaisProvavel} />
      </p>
      <p className={`mt-1 text-xs font-bold ${corValor}`}>
        {abaixoMeta
          ? `-${previsao.percentualAbaixoMeta}% abaixo da meta`
          : 'Na meta ou acima do esperado'}
      </p>

      <div className="mt-4 min-h-[160px] flex-1">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={dados} margin={{ top: 8, right: 8, left: -24, bottom: 0 }}>
            <CartesianGrid stroke={gridColor} strokeDasharray="3 3" vertical={false} />
            <XAxis
              dataKey="rotulo"
              tick={{ fill: '#475569', fontSize: 8 }}
              axisLine={false}
              tickLine={false}
              interval="preserveStartEnd"
            />
            <YAxis hide domain={['auto', 'auto']} />
            <Tooltip
              contentStyle={tooltipStyle}
              formatter={(v) => [formatarMoeda(Number(v ?? 0)), '']}
            />
            <Line type="monotone" dataKey="meta" stroke="#64748b" strokeWidth={1.5} strokeDasharray="4 4" dot={false} />
            <Line type="monotone" dataKey="valor" stroke={abaixoMeta ? '#ef4444' : '#22c55e'} strokeWidth={2.5} dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </div>

      <div className="mt-3 border-t border-rl-border pt-3">
        <div className="h-16">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={comparativo} layout="vertical" margin={{ left: 0, right: 8 }}>
              <XAxis type="number" hide />
              <YAxis type="category" dataKey="nome" tick={{ fill: '#64748b', fontSize: 9 }} width={52} axisLine={false} tickLine={false} />
              <Bar dataKey="valor" radius={[0, 4, 4, 0]} barSize={10}>
                {comparativo.map((entry) => (
                  <Cell key={entry.nome} fill={entry.cor} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
        <div className="mt-1 flex justify-between text-[9px] text-slate-600">
          <span>Meta: {formatarMoeda(previsao.meta)}</span>
          <span className={corValor}>Estimado: {formatarMoeda(previsao.cenarioMaisProvavel)}</span>
        </div>
      </div>
    </div>
  )
}
