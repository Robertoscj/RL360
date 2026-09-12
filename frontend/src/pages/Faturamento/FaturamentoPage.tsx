import { useCallback, useEffect, useState } from 'react'
import { Radio } from 'lucide-react'
import { Topbar } from '@/components/layout/Topbar'
import { LoadingSpinner } from '@/components/ui/LoadingSpinner'
import { CardsKpiFaturamento } from '@/components/faturamento/CardsKpiFaturamento'
import { GraficoFaturamentoDiario } from '@/components/faturamento/GraficoFaturamentoDiario'
import { PainelMetaFaturamento } from '@/components/faturamento/PainelMetaFaturamento'
import { ResumoLucroCustos } from '@/components/faturamento/ResumoLucroCustos'
import { FaturamentoPorCanal } from '@/components/faturamento/FaturamentoPorCanal'
import { TabelaVendasRecentes } from '@/components/faturamento/TabelaVendasRecentes'
import { obterFaturamento, obterVendas } from '@/services/faturamentoService'
import { usePeriodo } from '@/hooks/usePeriodo'
import type { Faturamento, Venda } from '@/types/faturamento'

export function FaturamentoPage() {
  const { periodo } = usePeriodo()
  const [faturamento, setFaturamento] = useState<Faturamento | null>(null)
  const [vendas, setVendas] = useState<Venda[]>([])
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(true)
  const [atualizando, setAtualizando] = useState(false)

  const carregar = useCallback(async (forcar = false) => {
    try {
      setErro('')
      if (forcar) setAtualizando(true)
      else setCarregando(true)

      const [fat, vds] = await Promise.all([obterFaturamento(periodo), obterVendas(periodo)])
      setFaturamento(fat)
      setVendas(vds)
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao carregar faturamento')
    } finally {
      setCarregando(false)
      setAtualizando(false)
    }
  }, [periodo])

  useEffect(() => {
    void carregar()
    const intervalo = setInterval(() => void carregar(true), 60_000)
    return () => clearInterval(intervalo)
  }, [carregar])

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-rl-bg">
        <LoadingSpinner texto="Carregando faturamento..." />
      </div>
    )
  }

  if (erro || !faturamento) {
    return (
      <div className="min-h-screen bg-rl-bg">
        <Topbar titulo="Faturamento" subtitulo="Em tempo real" />
        <div className="p-5">
          <div className="rounded-xl border border-red-500/30 bg-red-500/10 p-6 text-red-300">
            {erro || 'Dados indisponíveis'}
            <button type="button" onClick={() => void carregar(true)} className="btn-ghost mt-4">
              Tentar novamente
            </button>
          </div>
        </div>
      </div>
    )
  }

  const serie = faturamento.serieDiaria
  const ontem = serie.length >= 2 ? serie[serie.length - 2].valor : 0
  const variacaoDia =
    ontem > 0 ? ((faturamento.faturamentoDia - ontem) / ontem) * 100 : 0

  return (
    <div className="min-h-screen bg-rl-bg">
      <Topbar
        titulo="Faturamento"
        subtitulo="Em tempo real"
        onAtualizar={() => {
          setAtualizando(true)
          void carregar(true)
        }}
        atualizando={atualizando}
      />

      <div className="space-y-4 p-4">
        {/* Badge ao vivo */}
        <div className="flex items-center gap-2">
          <span className="inline-flex items-center gap-1.5 rounded-full border border-emerald-500/30 bg-emerald-500/10 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wide text-emerald-400">
            <Radio className="h-3 w-3 animate-pulse" />
            Ao vivo
          </span>
          <span className="text-xs font-medium text-rl-body">Atualização automática a cada 60s</span>
        </div>

        <CardsKpiFaturamento dados={faturamento} variacaoDia={variacaoDia} />

        <div className="grid grid-cols-1 items-stretch gap-4 xl:grid-cols-12">
          <div className="xl:col-span-8">
            <GraficoFaturamentoDiario serie={faturamento.serieDiaria} />
          </div>
          <div className="xl:col-span-4">
            <PainelMetaFaturamento dados={faturamento} />
          </div>
        </div>

        <div className="grid grid-cols-1 items-stretch gap-4 lg:grid-cols-2 xl:grid-cols-3">
          <ResumoLucroCustos dados={faturamento} />
          <FaturamentoPorCanal vendas={vendas} />
          <TabelaVendasRecentes vendas={vendas} />
        </div>
      </div>
    </div>
  )
}
