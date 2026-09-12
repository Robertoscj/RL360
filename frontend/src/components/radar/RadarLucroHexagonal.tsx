import {
  AlertTriangle,
  BarChart3,
  Clock,
  Cog,
  Filter,
  ShoppingCart,
  type LucideIcon,
} from 'lucide-react'
import { MoedaAnimada } from '@/components/ui/ValorAnimado'
import { useTheme } from '@/hooks/useTheme'
import { formatarMoeda } from '@/utils/format'
import { corHubRadar } from '@/utils/estilosGrafico'
import type { FatorRadar } from '@/types/dashboard'

interface RadarLucroProps {
  lucroAtual: number
  variacaoPercentual?: number | null
  fatores: FatorRadar[]
  versaoAnimacao?: number
}

const ORDEM: { fator: string; angulo: number; rotulo: string; Icone: LucideIcon }[] = [
  { fator: 'Inadimplencia', angulo: -90, rotulo: 'INADIMPLÊNCIA', Icone: AlertTriangle },
  { fator: 'GargaloComercial', angulo: -30, rotulo: 'GARGALO COMERCIAL', Icone: Clock },
  { fator: 'ExpansaoClientes', angulo: 30, rotulo: 'EXPANSÃO CLIENTES', Icone: BarChart3 },
  { fator: 'Produtividade', angulo: 90, rotulo: 'PRODUTIVIDADE', Icone: Cog },
  { fator: 'VendasNovas', angulo: 150, rotulo: 'VENDAS NOVAS', Icone: ShoppingCart },
  { fator: 'Conversao', angulo: 210, rotulo: 'CONVERSÃO', Icone: Filter },
]

type EstiloNo = {
  cor: string
  corBg: string
  corBorda: string
  corGlow: string
  corIcone: string
  corLinha: string
}

function estiloNo(fator: FatorRadar): EstiloNo {
  if (fator.fator === 'Inadimplencia') {
    return {
      cor: '#f87171',
      corBg: 'rgba(239,68,68,0.14)',
      corBorda: 'rgba(239,68,68,0.55)',
      corGlow: 'rgba(239,68,68,0.45)',
      corIcone: '#ef4444',
      corLinha: '#ef4444',
    }
  }
  if (fator.direcao === 'positivo' || fator.impactoFinanceiro >= 0) {
    return {
      cor: '#4ade80',
      corBg: 'rgba(34,197,94,0.14)',
      corBorda: 'rgba(34,197,94,0.55)',
      corGlow: 'rgba(34,197,94,0.45)',
      corIcone: '#22c55e',
      corLinha: '#22c55e',
    }
  }
  return {
    cor: '#fbbf24',
    corBg: 'rgba(245,158,11,0.14)',
    corBorda: 'rgba(245,158,11,0.55)',
    corGlow: 'rgba(245,158,11,0.45)',
    corIcone: '#f59e0b',
    corLinha: '#f59e0b',
  }
}

function hexPoints(cx: number, cy: number, r: number) {
  return Array.from({ length: 6 }, (_, i) => {
    const rad = ((60 * i - 90) * Math.PI) / 180
    return { x: cx + r * Math.cos(rad), y: cy + r * Math.sin(rad) }
  })
}

function hexPath(points: { x: number; y: number }[]) {
  return `M ${points.map((p) => `${p.x},${p.y}`).join(' L ')} Z`
}

function formatarValorComSinal(valor: number) {
  const sinal = valor >= 0 ? '+' : '-'
  return `${sinal}${formatarMoeda(Math.abs(valor))}`
}

function posicaoNo(cx: number, cy: number, angulo: number, distancia: number) {
  const rad = (angulo * Math.PI) / 180
  return { x: cx + distancia * Math.cos(rad), y: cy + distancia * Math.sin(rad) }
}

export function RadarLucroHexagonal({
  lucroAtual,
  variacaoPercentual,
  fatores,
  versaoAnimacao,
}: RadarLucroProps) {
  const { tema } = useTheme()
  const corHub = corHubRadar(tema)
  const mapa = new Map(fatores.map((f) => [f.fator, f]))
  const nos = ORDEM.map((pos) => ({
    ...pos,
    dados: mapa.get(pos.fator) ?? {
      fator: pos.fator,
      rotulo: pos.rotulo,
      impactoFinanceiro: 0,
      direcao: 'negativo',
      descricao: 'Sem dados no período',
    },
  }))

  const W = 560
  const H = 420
  const cx = W / 2
  const cy = H / 2
  const raioExterno = 168
  const raioHub = 58
  const raioNo = 168
  const vertices = hexPoints(cx, cy, raioExterno)
  const hubHex = hexPoints(cx, cy, raioHub)
  const grids = [0.32, 0.52, 0.72, 0.92].map((s) => hexPoints(cx, cy, raioExterno * s))

  const variacaoPositiva = (variacaoPercentual ?? 0) >= 0
  const variacaoTexto = variacaoPercentual != null
    ? `${variacaoPositiva ? '+' : ''}${variacaoPercentual.toLocaleString('pt-BR', { minimumFractionDigits: 1, maximumFractionDigits: 1 })}% vs média do mês`
    : 'Atualizado em tempo real'

  return (
    <div className="glass-card relative flex min-h-[460px] flex-col overflow-hidden p-3">
      <p className="section-label mb-1 px-1">Radar de Lucro</p>

      <div
        className="@container relative mx-auto w-full flex-1"
        style={{ maxWidth: W, aspectRatio: `${W} / ${H}` }}
      >
        <svg
          width="100%"
          height="100%"
          viewBox={`0 0 ${W} ${H}`}
          preserveAspectRatio="xMidYMid meet"
          aria-hidden
        >
          <defs>
            <filter id="radar-glow-strong" x="-80%" y="-80%" width="260%" height="260%">
              <feGaussianBlur stdDeviation="4" result="blur" />
              <feMerge>
                <feMergeNode in="blur" />
                <feMergeNode in="SourceGraphic" />
              </feMerge>
            </filter>
            <filter id="radar-glow-soft" x="-50%" y="-50%" width="200%" height="200%">
              <feGaussianBlur stdDeviation="2.5" result="blur" />
              <feMerge>
                <feMergeNode in="blur" />
                <feMergeNode in="SourceGraphic" />
              </feMerge>
            </filter>
            <filter id="hub-glow" x="-100%" y="-100%" width="300%" height="300%">
              <feGaussianBlur stdDeviation="6" result="blur" />
              <feMerge>
                <feMergeNode in="blur" />
                <feMergeNode in="SourceGraphic" />
              </feMerge>
            </filter>
          </defs>

          {grids.map((pts, gi) => (
            <path
              key={`grid-${gi}`}
              d={hexPath(pts)}
              fill="none"
              stroke="#22c55e"
              strokeOpacity={0.06 + gi * 0.03}
              strokeWidth="1"
            />
          ))}

          <path
            d={hexPath(vertices)}
            fill="none"
            stroke="#22c55e"
            strokeOpacity="0.2"
            strokeWidth="1.2"
          />

          {vertices.map((v, i) => {
            const next = vertices[(i + 1) % 6]
            const noAtual = nos[i]
            const estilo = estiloNo(noAtual.dados)
            const noProx = nos[(i + 1) % 6]
            const estiloProx = estiloNo(noProx.dados)
            const corMeio =
              estilo.corLinha === estiloProx.corLinha ? estilo.corLinha : '#22c55e'
            return (
              <line
                key={`perim-${i}`}
                x1={v.x}
                y1={v.y}
                x2={next.x}
                y2={next.y}
                stroke={corMeio}
                strokeOpacity="0.55"
                strokeWidth="2"
                filter="url(#radar-glow-soft)"
              />
            )
          })}

          {nos.map(({ angulo, dados }) => {
            const estilo = estiloNo(dados)
            const pos = posicaoNo(cx, cy, angulo, raioNo)
            return (
              <line
                key={`spoke-${dados.fator}`}
                x1={cx}
                y1={cy}
                x2={pos.x}
                y2={pos.y}
                stroke={estilo.corLinha}
                strokeOpacity="0.65"
                strokeWidth="2"
                filter="url(#radar-glow-soft)"
              />
            )
          })}

          <path
            d={hexPath(hubHex)}
            fill="none"
            stroke="#22c55e"
            strokeOpacity="0.25"
            strokeWidth="6"
            filter="url(#hub-glow)"
          />
          <path
            d={hexPath(hubHex)}
            fill={corHub}
            stroke="#22c55e"
            strokeWidth="2"
            filter="url(#radar-glow-soft)"
          />

          {nos.map(({ angulo, dados }) => {
            const estilo = estiloNo(dados)
            const pos = posicaoNo(cx, cy, angulo, raioNo)
            const noHex = hexPoints(pos.x, pos.y, 52)

            return (
              <g key={dados.fator}>
                <path
                  d={hexPath(noHex)}
                  fill={estilo.corBg}
                  stroke={estilo.corBorda}
                  strokeWidth="1.5"
                  filter="url(#radar-glow-soft)"
                />
                <path
                  d={hexPath(noHex)}
                  fill="none"
                  stroke={estilo.corGlow}
                  strokeOpacity="0.4"
                  strokeWidth="3"
                />
              </g>
            )
          })}
        </svg>

        <div className="pointer-events-none absolute inset-0 z-20 flex items-center justify-center">
          <div className="flex w-[22%] min-w-[92px] max-w-[132px] -translate-y-px flex-col items-center justify-center text-center">
            <p className="w-full text-center text-[clamp(7px,2.2cqi,9px)] font-bold uppercase tracking-[0.12em] text-slate-500">
              Lucro Atual
            </p>
            <p
              className="mt-1 w-full text-center text-[clamp(13px,4.4cqi,21px)] font-black leading-none text-rl-heading transition-all duration-500"
              style={{ textShadow: '0 0 24px rgba(34,197,94,0.35)' }}
            >
              <MoedaAnimada
                valor={lucroAtual}
                reiniciarChave={versaoAnimacao}
                className="block w-full text-center"
              />
            </p>
            <p
              className={`mt-1.5 w-full text-center text-[clamp(8px,2.1cqi,10px)] font-semibold leading-tight ${
                variacaoPositiva ? 'text-emerald-400' : 'text-red-400'
              }`}
            >
              {variacaoTexto}
            </p>
          </div>
        </div>

        {nos.map(({ angulo, dados, rotulo, Icone }) => {
          const estilo = estiloNo(dados)
          const pos = posicaoNo(cx, cy, angulo, raioNo)
          const leftPct = (pos.x / W) * 100
          const topPct = (pos.y / H) * 100

          return (
            <div
              key={`content-${dados.fator}`}
              className="pointer-events-none absolute z-10 -translate-x-1/2 -translate-y-1/2"
              style={{ left: `${leftPct}%`, top: `${topPct}%`, width: 108 }}
            >
              <div className="flex items-start gap-1.5 px-2 py-1">
                <div
                  className="flex h-6 w-6 shrink-0 items-center justify-center rounded-md"
                  style={{ background: `${estilo.corIcone}22`, color: estilo.corIcone }}
                >
                  <Icone size={12} strokeWidth={2.2} />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-[7px] font-bold uppercase tracking-wide text-slate-400">
                    {dados.rotulo || rotulo}
                  </p>
                  <p className="text-[12px] font-black leading-tight" style={{ color: estilo.cor }}>
                    {formatarValorComSinal(dados.impactoFinanceiro)}
                  </p>
                  <p className="mt-0.5 line-clamp-2 text-[7px] leading-tight text-slate-500">
                    {dados.descricao}
                  </p>
                </div>
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}
