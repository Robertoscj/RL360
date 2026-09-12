import { useCallback, useEffect, useMemo, useState } from 'react'
import { Radio } from 'lucide-react'
import { Topbar } from '@/components/layout/Topbar'
import { LoadingSpinner } from '@/components/ui/LoadingSpinner'
import { FluxoCaixaCard } from '@/components/charts/FluxoCaixaCard'
import { CardsKpiInadimplencia } from '@/components/inadimplencia/CardsKpiInadimplencia'
import { GraficoFaixasAtraso } from '@/components/inadimplencia/GraficoFaixasAtraso'
import { GraficoRecuperacaoVsPerda } from '@/components/inadimplencia/GraficoRecuperacaoVsPerda'
import { PainelContasCriticas } from '@/components/inadimplencia/PainelContasCriticas'
import { TabelaDevedores } from '@/components/inadimplencia/TabelaDevedores'
import { obterResumoDashboard } from '@/services/dashboardService'
import { obterInadimplencia } from '@/services/inadimplenciaService'
import type { FluxoCaixaFuturo } from '@/types/dashboard'
import type { RegistroInadimplencia } from '@/types/inadimplencia'
import { agruparPorFaixa, calcularResumo } from '@/utils/inadimplencia'

export function InadimplenciaPage() {
  const [registros, setRegistros] = useState<RegistroInadimplencia[]>([])
  const [riscoProximos30Dias, setRiscoProximos30Dias] = useState<number | undefined>()
  const [faturamentoMes, setFaturamentoMes] = useState<number | undefined>()
  const [fluxoCaixa, setFluxoCaixa] = useState<FluxoCaixaFuturo | null>(null)
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(true)
  const [atualizando, setAtualizando] = useState(false)

  const carregar = useCallback(async (forcar = false) => {
    try {
      setErro('')
      if (forcar) setAtualizando(true)
      else setCarregando(true)

      const [inad, dashboard] = await Promise.all([
        obterInadimplencia(),
        obterResumoDashboard(forcar),
      ])

      setRegistros(inad)
      setRiscoProximos30Dias(dashboard.cardsTopo.riscoProximos30Dias)
      setFaturamentoMes(dashboard.radar.faturamentoMes)
      setFluxoCaixa(dashboard.fluxoCaixaFuturo)
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao carregar inadimplência')
    } finally {
      setCarregando(false)
      setAtualizando(false)
    }
  }, [])

  useEffect(() => {
    void carregar()
    const intervalo = setInterval(() => void carregar(true), 60_000)
    return () => clearInterval(intervalo)
  }, [carregar])

  const resumo = useMemo(
    () => calcularResumo(registros, faturamentoMes),
    [registros, faturamentoMes],
  )
  const faixas = useMemo(() => agruparPorFaixa(registros), [registros])

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-rl-bg">
        <LoadingSpinner texto="Carregando inadimplência..." />
      </div>
    )
  }

  if (erro) {
    return (
      <div className="min-h-screen bg-rl-bg">
        <Topbar titulo="Inadimplência" subtitulo="Risco e previsão" />
        <div className="p-5">
          <div className="rounded-xl border border-red-500/30 bg-red-500/10 p-6 text-red-300">
            {erro}
            <button type="button" onClick={() => void carregar(true)} className="btn-ghost mt-4">
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
        titulo="Inadimplência"
        subtitulo="Risco e previsão"
        onAtualizar={() => {
          setAtualizando(true)
          void carregar(true)
        }}
        atualizando={atualizando}
      />

      <div className="space-y-4 p-4">
        <div className="flex items-center gap-2">
          <span className="inline-flex items-center gap-1.5 rounded-full border border-red-500/30 bg-red-500/10 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wide text-red-400">
            <Radio className="h-3 w-3 animate-pulse" />
            Monitoramento ativo
          </span>
          <span className="text-[10px] text-slate-600">Atualização automática a cada 60s</span>
        </div>

        <CardsKpiInadimplencia resumo={resumo} riscoProximos30Dias={riscoProximos30Dias} />

        <div className="grid grid-cols-1 gap-4 xl:grid-cols-12">
          <div className="xl:col-span-7">
            <GraficoFaixasAtraso faixas={faixas} />
          </div>
          <div className="xl:col-span-5">
            {fluxoCaixa ? <FluxoCaixaCard fluxo={fluxoCaixa} /> : null}
          </div>
        </div>

        <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
          <GraficoRecuperacaoVsPerda registros={registros} />
          <PainelContasCriticas registros={registros} />
        </div>

        <TabelaDevedores registros={registros} />
      </div>
    </div>
  )
}
