import { useMemo } from 'react'
import { AreaChart, Area, ResponsiveContainer } from 'recharts'
import { MoedaAnimada, NumeroAnimado } from '@/components/ui/ValorAnimado'
import {
  calcularDeltaPercentual,
  deltaSerie,
  deltaSeriePeriodo,
  resumoVendas,
  serieParaSparkline,
} from '@/utils/dashboardMetricas'
import { formatarMoeda, formatarPercentual } from '@/utils/format'
import type { SnapshotRadar } from '@/types/dashboard'
import type { Faturamento, Venda } from '@/types/faturamento'

interface MetricasSparklineProps {
  radar: SnapshotRadar
  faturamento?: Faturamento | null
  vendas?: Venda[]
  versaoAnimacao?: number
}

export function MetricasSparkline({
  radar,
  faturamento,
  vendas = [],
  versaoAnimacao,
}: MetricasSparklineProps) {
  const metricas = useMemo(() => {
    const serie = faturamento?.serieDiaria ?? []
    const resumo = resumoVendas(vendas)
    const taxaConversaoPct = radar.taxaConversaoAtual <= 1
      ? radar.taxaConversaoAtual * 100
      : radar.taxaConversaoAtual
    const quedaPct = radar.quedaConversao <= 1
      ? radar.quedaConversao * 100
      : radar.quedaConversao

    return [
      {
        rotulo: 'Faturamento Hoje',
        valorNode: <MoedaAnimada valor={radar.faturamentoDia} className="text-[15px] font-black text-rl-heading" reiniciarChave={versaoAnimacao} />,
        delta: deltaSerie(serie) ?? calcularDeltaPercentual(radar.faturamentoDia, radar.faturamentoMes / 30),
        positivo: true,
        dados: serie.length > 0 ? serieParaSparkline(serie) : [{ v: radar.faturamentoDia }],
        cor: '#22c55e',
        barra: false,
      },
      {
        rotulo: 'Faturamento Mês',
        valorNode: <MoedaAnimada valor={radar.faturamentoMes} className="text-[15px] font-black text-rl-heading" reiniciarChave={versaoAnimacao} />,
        delta: deltaSeriePeriodo(serie),
        positivo: (deltaSeriePeriodo(serie)?.startsWith('+') ?? radar.faturamentoMes > 0),
        dados: serie.length > 0 ? serieParaSparkline(serie) : [{ v: radar.faturamentoMes }],
        cor: '#22c55e',
        barra: false,
      },
      {
        rotulo: 'Ticket Médio',
        valorNode: <MoedaAnimada valor={resumo.ticketMedio} className="text-[15px] font-black text-rl-heading" reiniciarChave={versaoAnimacao} />,
        delta: resumo.quantidade > 0
          ? calcularDeltaPercentual(resumo.ticketMedio, radar.faturamentoMes / Math.max(resumo.quantidade, 1))
          : null,
        positivo: true,
        dados: resumo.quantidade > 0
          ? vendas.slice(-12).map((v) => ({ v: v.valor }))
          : [{ v: resumo.ticketMedio }],
        cor: '#22c55e',
        barra: false,
      },
      {
        rotulo: 'Novos Clientes',
        valorNode: <NumeroAnimado valor={resumo.novosClientes} className="text-[15px] font-black text-rl-heading" reiniciarChave={versaoAnimacao} />,
        delta: resumo.quantidade > 0
          ? calcularDeltaPercentual(resumo.novosClientes, resumo.quantidade - resumo.novosClientes)
          : null,
        positivo: resumo.novosClientes > 0,
        dados: vendas.filter((v) => v.clienteNovo).slice(-12).map((v) => ({ v: v.valor }))
          .concat(resumo.novosClientes > 0 ? [] : [{ v: resumo.novosClientes }]),
        cor: '#22c55e',
        barra: false,
      },
      {
        rotulo: 'Conversão Geral',
        valorNode: (
          <span className="text-[15px] font-black text-rl-heading">
            {formatarPercentual(taxaConversaoPct, 1)}
          </span>
        ),
        delta: quedaPct > 0
          ? `-${quedaPct.toFixed(1).replace('.', ',')}%`
          : calcularDeltaPercentual(taxaConversaoPct, taxaConversaoPct + quedaPct),
        positivo: quedaPct <= 0,
        dados: [{ v: taxaConversaoPct }, { v: Math.max(0, taxaConversaoPct - quedaPct) }, { v: taxaConversaoPct }],
        cor: quedaPct > 0 ? '#ef4444' : '#22c55e',
        barra: false,
      },
      {
        rotulo: 'Meta do Mês',
        valorNode: (
          <span className="text-[15px] font-black text-rl-heading">
            {formatarPercentual(radar.percentualMetaAtingida, 0)}
          </span>
        ),
        delta: calcularDeltaPercentual(radar.percentualMetaAtingida, 100),
        positivo: radar.percentualMetaAtingida >= 100,
        dados: [{ v: radar.percentualMetaAtingida }],
        cor: radar.percentualMetaAtingida >= 100 ? '#22c55e' : '#f59e0b',
        barra: true,
        progresso: radar.percentualMetaAtingida,
      },
    ]
  }, [radar, faturamento, vendas, versaoAnimacao])

  return (
    <div key={versaoAnimacao} className="grid grid-cols-2 gap-3 sm:grid-cols-3 xl:grid-cols-6">
      {metricas.map(({ rotulo, valorNode, delta, positivo, dados, cor, barra, progresso }) => (
        <div key={rotulo} className="glass-card p-3 transition-shadow duration-500 hover:shadow-[0_0_20px_rgba(34,197,94,0.06)]">
          <p className="text-[9px] font-bold uppercase tracking-wide text-slate-500">{rotulo}</p>
          <div className="mt-1.5 flex items-baseline justify-between gap-1">
            {valorNode}
            {delta && (
              <span className={`text-[10px] font-bold ${positivo ? 'text-emerald-400' : 'text-red-400'}`}>
                {delta}
              </span>
            )}
          </div>
          {barra && progresso !== undefined ? (
            <div className="mt-2">
              <div className="h-1.5 overflow-hidden rounded-full bg-rl-surface">
                <div
                  className="h-full rounded-full bg-emerald-500 transition-all duration-700"
                  style={{ width: `${Math.min(progresso, 100)}%` }}
                />
              </div>
              <p className="mt-1 text-[9px] text-slate-600">
                {formatarMoeda(radar.faturamentoMes)} / {formatarMoeda(radar.metaMensal)}
              </p>
            </div>
          ) : (
            <div className="mt-2 h-9">
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={dados.length > 0 ? dados : [{ v: 0 }]}>
                  <Area
                    type="monotone"
                    dataKey="v"
                    stroke={cor}
                    fill={cor}
                    fillOpacity={0.12}
                    strokeWidth={1.5}
                    dot={false}
                    isAnimationActive
                  />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          )}
        </div>
      ))}
    </div>
  )
}
