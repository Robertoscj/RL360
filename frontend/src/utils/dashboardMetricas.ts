import type { PontoSerieTemporal } from '@/types/dashboard'
import type { Venda } from '@/types/faturamento'

export function calcularDeltaPercentual(valorAtual: number, valorReferencia: number): string | null {
  if (valorReferencia === 0) return null
  const pct = ((valorAtual - valorReferencia) / Math.abs(valorReferencia)) * 100
  const sinal = pct >= 0 ? '+' : ''
  return `${sinal}${pct.toFixed(1).replace('.', ',')}%`
}

export function serieParaSparkline(pontos: PontoSerieTemporal[] | { valor: number }[]): { v: number }[] {
  if (pontos.length === 0) return [{ v: 0 }]
  return pontos.map((p) => ({ v: 'valor' in p ? p.valor : 0 }))
}

export function deltaSerie(pontos: PontoSerieTemporal[]): string | null {
  if (pontos.length < 2) return null
  const ultimo = pontos[pontos.length - 1].valor
  const anterior = pontos[pontos.length - 2].valor
  return calcularDeltaPercentual(ultimo, anterior)
}

export function deltaSeriePeriodo(pontos: PontoSerieTemporal[]): string | null {
  if (pontos.length < 4) return null
  const metade = Math.floor(pontos.length / 2)
  const mediaRecente = pontos.slice(metade).reduce((s, p) => s + p.valor, 0) / (pontos.length - metade)
  const mediaAnterior = pontos.slice(0, metade).reduce((s, p) => s + p.valor, 0) / metade
  return calcularDeltaPercentual(mediaRecente, mediaAnterior)
}

export function calcularVariacaoLucroHub(
  faturamentoDia: number,
  faturamentoMes: number,
): number | null {
  if (faturamentoMes <= 0) return null
  const mediaDiaria = faturamentoMes / 30
  if (mediaDiaria <= 0) return null
  return ((faturamentoDia - mediaDiaria) / mediaDiaria) * 100
}

export function resumoVendas(vendas: Venda[]) {
  const total = vendas.reduce((s, v) => s + v.valor, 0)
  const novosClientes = vendas.filter((v) => v.clienteNovo).length
  return {
    quantidade: vendas.length,
    total,
    novosClientes,
    ticketMedio: vendas.length > 0 ? total / vendas.length : 0,
  }
}

export function badgeStatusSaude(statusSaude: string, percentual: number) {
  const normalizado = statusSaude.toLowerCase()
  if (normalizado.includes('crit') || percentual < 50) {
    return { rotulo: 'Crítico', classe: 'border-red-500/30 bg-red-500/15 text-red-400' }
  }
  if (normalizado.includes('aten') || percentual < 75) {
    return { rotulo: 'Atenção', classe: 'border-amber-500/30 bg-amber-500/15 text-amber-400' }
  }
  return { rotulo: 'Saudável', classe: 'border-emerald-500/30 bg-emerald-500/15 text-emerald-400' }
}

export function metaSaudePercentual(percentual: number) {
  return Math.min(100, Math.max(percentual + 14, 85))
}
