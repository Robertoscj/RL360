import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts'
import { formatarMoeda } from '@/utils/format'
import type { FluxoCaixaFuturo } from '@/types/dashboard'

interface FluxoCaixaCardProps {
  fluxo: FluxoCaixaFuturo
}

export function FluxoCaixaCard({ fluxo }: FluxoCaixaCardProps) {
  const itens = [
    { rotulo: 'A receber', valor: fluxo.aReceber, cor: '#22c55e', text: 'text-emerald-400' },
    { rotulo: 'Em risco', valor: fluxo.emRisco, cor: '#f59e0b', text: 'text-amber-400' },
    { rotulo: 'Atrasado', valor: fluxo.atrasado, cor: '#ef4444', text: 'text-red-400' },
  ]

  return (
    <div className="glass-card p-4">
      <p className="section-label">Fluxo de Caixa Futuro</p>
      <p className="section-desc">Próximos {fluxo.dias} dias</p>

      <div className="relative mx-auto mt-4 h-[140px] w-[140px]">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={fluxo.segmentos}
              dataKey="valor"
              innerRadius={48}
              outerRadius={66}
              paddingAngle={2}
              strokeWidth={0}
            >
              {fluxo.segmentos.map((seg) => (
                <Cell key={seg.rotulo} fill={seg.cor} />
              ))}
            </Pie>
          </PieChart>
        </ResponsiveContainer>
        <div className="absolute inset-0 flex flex-col items-center justify-center">
          <span className="text-2xl font-black text-emerald-400">{fluxo.percentualSaudavel}%</span>
          <span className="text-xs font-semibold text-rl-body">Saudável</span>
        </div>
      </div>

      <div className="mt-4 space-y-2.5">
        {itens.map(({ rotulo, valor, cor, text }) => (
          <div key={rotulo} className="flex items-center justify-between text-xs">
            <span className="flex items-center gap-2 font-medium text-rl-heading">
              <span className="h-2 w-2 rounded-full" style={{ background: cor }} />
              {rotulo}
            </span>
            <span className={`font-bold ${text}`}>{formatarMoeda(valor)}</span>
          </div>
        ))}
      </div>
    </div>
  )
}
