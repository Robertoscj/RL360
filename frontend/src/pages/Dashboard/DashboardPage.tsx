import { useCallback, useEffect, useMemo, useState } from 'react'
import { Radio } from 'lucide-react'
import { Topbar } from '@/components/layout/Topbar'
import { RodapeBanner } from '@/components/layout/RodapeBanner'
import { CardsTopoDashboard } from '@/components/cards/CardsTopoDashboard'
import { MetricasSparkline } from '@/components/cards/MetricasSparkline'
import { RadarLucroHexagonal } from '@/components/radar/RadarLucroHexagonal'
import { PrevisaoCard } from '@/components/charts/PrevisaoCard'
import { FluxoCaixaCard } from '@/components/charts/FluxoCaixaCard'
import { InsightsPanel } from '@/components/alerts/InsightsPanel'
import { AlertasCriticosRow } from '@/components/alerts/AlertasCriticosRow'
import { LoadingSpinner } from '@/components/ui/LoadingSpinner'
import { useDashboardTempoReal } from '@/hooks/useDashboardTempoReal'
import { obterResumoDashboard, recalcularRadar } from '@/services/dashboardService'
import { obterFaturamento, obterVendas } from '@/services/faturamentoService'
import { usePeriodo } from '@/hooks/usePeriodo'
import type { ResumoDashboard, SnapshotRadar } from '@/types/dashboard'
import type { Faturamento, Venda } from '@/types/faturamento'
import { calcularVariacaoLucroHub } from '@/utils/dashboardMetricas'
import { paraIsoData, type IntervaloDatas } from '@/utils/datas'

function chavePeriodo(periodo: IntervaloDatas) {
  return `${paraIsoData(periodo.inicio)}_${paraIsoData(periodo.fim)}`
}

function mesclarRadar(resumo: ResumoDashboard, radar: SnapshotRadar): ResumoDashboard {
  return {
    ...resumo,
    radar,
    cardsTopo: {
      riscoProximos30Dias: radar.riscoProximos30Dias,
      valorOportunidade: radar.valorOportunidade,
      gargalosCriticos: radar.gargalosCriticos,
      saudeEmpresaPercentual: radar.saudeEmpresaPercentual,
      statusSaude: radar.statusSaude,
    },
  }
}

function formatarHorario(iso: string) {
  return new Date(iso).toLocaleTimeString('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  })
}

export function DashboardPage() {
  const { periodo } = usePeriodo()
  const [resumo, setResumo] = useState<ResumoDashboard | null>(null)
  const [faturamento, setFaturamento] = useState<Faturamento | null>(null)
  const [vendas, setVendas] = useState<Venda[]>([])
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(true)
  const [atualizando, setAtualizando] = useState(false)
  const [versaoAnimacao, setVersaoAnimacao] = useState(0)
  const [periodoCarregado, setPeriodoCarregado] = useState(() => chavePeriodo(periodo))

  const chaveAtual = chavePeriodo(periodo)
  if (chaveAtual !== periodoCarregado) {
    setPeriodoCarregado(chaveAtual)
    setCarregando(true)
  }

  useEffect(() => {
    let cancelado = false

    void (async () => {
      try {
        const [dados, fat, vds] = await Promise.all([
          obterResumoDashboard(false, periodo),
          obterFaturamento(),
          obterVendas(),
        ])
        if (cancelado) return
        setResumo(dados)
        setFaturamento(fat)
        setVendas(vds)
        setErro('')
      } catch (err) {
        if (!cancelado) {
          setErro(err instanceof Error ? err.message : 'Erro ao carregar dashboard')
        }
      } finally {
        if (!cancelado) setCarregando(false)
      }
    })()

    return () => {
      cancelado = true
    }
  }, [periodo])

  const atualizar = useCallback(async () => {
    try {
      setErro('')
      setAtualizando(true)

      try {
        await recalcularRadar()
      } catch {
        /* fallback: summary com atualizar=true recalcula radar no servidor */
      }

      const [dados, fat, vds] = await Promise.all([
        obterResumoDashboard(true, periodo),
        obterFaturamento(),
        obterVendas(),
      ])

      setResumo(dados)
      setFaturamento(fat)
      setVendas(vds)
      setVersaoAnimacao((v) => v + 1)
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao carregar dashboard')
    } finally {
      setAtualizando(false)
    }
  }, [periodo])

  useEffect(() => {
    const intervalo = setInterval(() => void atualizar(), 60_000)
    return () => clearInterval(intervalo)
  }, [atualizar])

  const onDashboardTempoReal = useCallback((dados: ResumoDashboard) => {
    setResumo(dados)
    void obterFaturamento().then(setFaturamento).catch(() => {})
    void obterVendas().then(setVendas).catch(() => {})
  }, [])

  const onRadarTempoReal = useCallback((radar: SnapshotRadar) => {
    setResumo((atual) => (atual ? mesclarRadar(atual, radar) : atual))
  }, [])

  useDashboardTempoReal({
    onDashboard: onDashboardTempoReal,
    onRadar: onRadarTempoReal,
  })

  const variacaoLucro = useMemo(() => {
    if (!resumo) return null
    return calcularVariacaoLucroHub(resumo.radar.faturamentoDia, resumo.radar.faturamentoMes)
  }, [resumo])

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-rl-bg">
        <LoadingSpinner texto="Carregando radar de lucro..." />
      </div>
    )
  }

  if (erro || !resumo) {
    return (
      <div className="min-h-screen bg-rl-bg">
        <Topbar onAtualizar={() => void atualizar()} atualizando={atualizando} />
        <div className="p-5">
          <div className="rounded-xl border border-red-500/30 bg-red-500/10 p-6 text-red-300">
            {erro || 'Dados indisponíveis'}
            <button type="button" onClick={() => void atualizar()} className="btn-ghost mt-4">
              Tentar novamente
            </button>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-rl-bg">
      <Topbar
        onAtualizar={() => void atualizar()}
        atualizando={atualizando}
      />

      <div className="relative space-y-4 p-4">
        {atualizando && (
          <div className="pointer-events-none absolute inset-0 z-10 rounded-lg bg-rl-bg/40 backdrop-blur-[1px]" />
        )}

        <div className="flex flex-wrap items-center gap-2">
          <span className="inline-flex items-center gap-1.5 rounded-full border border-emerald-500/30 bg-emerald-500/10 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wide text-emerald-400">
            <Radio className={`h-3 w-3 ${atualizando ? 'animate-spin' : 'animate-pulse'}`} />
            {atualizando ? 'Atualizando radar...' : 'Radar ao vivo'}
          </span>
          <span className="text-[10px] text-rl-muted">
            Última atualização: {formatarHorario(resumo.geradoEmUtc)}
            {' · '}
            automática a cada 60s
          </span>
        </div>

        <div
          key={versaoAnimacao}
          className={`space-y-4 transition-opacity duration-300 ${atualizando ? 'opacity-70' : 'opacity-100'}`}
        >
          <CardsTopoDashboard cards={resumo.cardsTopo} versaoAnimacao={versaoAnimacao} />

          <div className="grid grid-cols-1 gap-4 xl:grid-cols-12">
            <div className="xl:col-span-2">
              <PrevisaoCard previsao={resumo.previsaoResultado} />
            </div>

            <div className="xl:col-span-5">
              <RadarLucroHexagonal
                lucroAtual={resumo.radar.lucroAtual}
                variacaoPercentual={variacaoLucro}
                fatores={resumo.radar.fatores}
                versaoAnimacao={versaoAnimacao}
              />
            </div>

            <div className="xl:col-span-2">
              <FluxoCaixaCard fluxo={resumo.fluxoCaixaFuturo} />
            </div>

            <div className="h-[460px] xl:col-span-3">
              <InsightsPanel insights={resumo.insights} />
            </div>
          </div>

          <AlertasCriticosRow alertas={resumo.alertasCriticos} />
          <MetricasSparkline
            radar={resumo.radar}
            faturamento={faturamento}
            vendas={vendas}
            versaoAnimacao={versaoAnimacao}
          />
          <RodapeBanner
            mensagem={resumo.rodape.mensagem}
            quantidadeAcoes={resumo.rodape.quantidadeAcoesPlano}
            planoAcao={
              resumo.rodape.planoAcao.length > 0
                ? resumo.rodape.planoAcao
                : resumo.radar.planoAcao
            }
          />
        </div>
      </div>
    </div>
  )
}
