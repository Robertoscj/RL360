import { useCallback, useEffect, useMemo, useState } from 'react'
import { Radio } from 'lucide-react'
import { Topbar } from '@/components/layout/Topbar'
import { LoadingSpinner } from '@/components/ui/LoadingSpinner'
import { FaturamentoPorCanal } from '@/components/faturamento/FaturamentoPorCanal'
import { TabelaVendasRecentes } from '@/components/faturamento/TabelaVendasRecentes'
import { CardsKpiVendas } from '@/components/vendas/CardsKpiVendas'
import { FunilAoVivo } from '@/components/vendas/FunilAoVivo'
import { GraficoConversaoEtapas } from '@/components/vendas/GraficoConversaoEtapas'
import { PainelGargalosFunil } from '@/components/vendas/PainelGargalosFunil'
import { obterFunil, obterVendasModulo } from '@/services/vendasService'
import { usePeriodo } from '@/hooks/usePeriodo'
import type { EtapaFunil, ResumoVendas, Venda } from '@/types/vendas'

function calcularResumo(vendas: Venda[]): ResumoVendas {
  const totalVendas = vendas.reduce((s, v) => s + v.valor, 0)
  const totalMargem = vendas.reduce((s, v) => s + v.margem, 0)
  const novosClientes = vendas.filter((v) => v.clienteNovo).length
  return {
    totalVendas,
    totalMargem,
    quantidadeVendas: vendas.length,
    novosClientes,
    ticketMedio: vendas.length > 0 ? totalVendas / vendas.length : 0,
  }
}

function calcularConversao(funil: EtapaFunil[]) {
  const ordenado = [...funil].sort((a, b) => a.ordem - b.ordem)
  if (ordenado.length < 2) return { conversaoGeral: 0, quedaConversao: 0 }

  const primeira = ordenado[0]
  const ultima = ordenado[ordenado.length - 1]
  const conversaoGeral = primeira.quantidade > 0 ? ultima.quantidade / primeira.quantidade : 0

  const quedas = ordenado.map((e) => {
    const atual = e.taxaConversao <= 1 ? e.taxaConversao : e.taxaConversao / 100
    const base = e.taxaConversaoBase <= 1 ? e.taxaConversaoBase : e.taxaConversaoBase / 100
    return (base - atual) * 100
  })
  const quedaConversao = quedas.reduce((s, q) => s + Math.max(0, q), 0) / ordenado.length

  return { conversaoGeral, quedaConversao }
}

export function VendasPage() {
  const { periodo } = usePeriodo()
  const [funil, setFunil] = useState<EtapaFunil[]>([])
  const [vendas, setVendas] = useState<Venda[]>([])
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(true)
  const [atualizando, setAtualizando] = useState(false)

  const carregar = useCallback(async (forcar = false) => {
    try {
      setErro('')
      if (forcar) setAtualizando(true)
      else setCarregando(true)

      const [f, v] = await Promise.all([obterFunil(), obterVendasModulo(periodo)])
      setFunil(f)
      setVendas(v)
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao carregar vendas')
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

  const resumo = useMemo(() => calcularResumo(vendas), [vendas])
  const { conversaoGeral, quedaConversao } = useMemo(() => calcularConversao(funil), [funil])

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-rl-bg">
        <LoadingSpinner texto="Carregando funil de vendas..." />
      </div>
    )
  }

  if (erro || funil.length === 0) {
    return (
      <div className="min-h-screen bg-rl-bg">
        <Topbar titulo="Vendas" subtitulo="Funil ao vivo" />
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

  return (
    <div className="min-h-screen bg-rl-bg">
      <Topbar
        titulo="Vendas"
        subtitulo="Funil ao vivo"
        onAtualizar={() => {
          setAtualizando(true)
          void carregar(true)
        }}
        atualizando={atualizando}
      />

      <div className="space-y-4 p-4">
        <div className="flex items-center gap-2">
          <span className="inline-flex items-center gap-1.5 rounded-full border border-emerald-500/30 bg-emerald-500/10 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wide text-emerald-400">
            <Radio className="h-3 w-3 animate-pulse" />
            Funil ao vivo
          </span>
          <span className="text-xs font-medium text-rl-body">Atualização automática a cada 60s</span>
        </div>

        <CardsKpiVendas
          resumo={resumo}
          funil={funil}
          conversaoGeral={conversaoGeral}
          quedaConversao={quedaConversao}
        />

        <div className="grid grid-cols-1 gap-4 xl:grid-cols-12">
          <div className="xl:col-span-7">
            <FunilAoVivo
              etapas={funil}
              fechados={{ quantidade: resumo.quantidadeVendas, valor: resumo.totalVendas }}
            />
          </div>
          <div className="xl:col-span-5">
            <PainelGargalosFunil etapas={funil} />
          </div>
        </div>

        <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
          <GraficoConversaoEtapas etapas={funil} />
          <FaturamentoPorCanal vendas={vendas} />
        </div>

        <TabelaVendasRecentes vendas={vendas} />
      </div>
    </div>
  )
}
