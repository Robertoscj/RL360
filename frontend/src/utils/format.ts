const brl = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
  maximumFractionDigits: 0,
})

const brlCompact = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
  notation: 'compact',
  maximumFractionDigits: 1,
})

export function formatarMoeda(valor: number): string {
  return brl.format(valor)
}

export function formatarMoedaCompacta(valor: number): string {
  if (Math.abs(valor) >= 1_000_000) return brlCompact.format(valor)
  return brl.format(valor)
}

export function formatarMoedaLeitura(valor: number): string {
  const abs = Math.abs(valor)
  if (abs >= 1_000_000) {
    const n = valor / 1_000_000
    const texto = n.toFixed(Math.abs(n) >= 10 ? 0 : 1).replace('.', ',').replace(',0', '')
    const unidade = Math.abs(n) >= 2 ? 'milhões' : 'milhão'
    return `R$ ${texto} ${unidade}`
  }
  if (abs >= 1000) {
    return `R$ ${Math.round(valor / 1000).toLocaleString('pt-BR')} mil`
  }
  return formatarMoeda(valor)
}

export function formatarPercentual(valor: number, casas = 0): string {
  return `${valor.toFixed(casas)}%`
}

export function corImpacto(valor: number): string {
  if (valor > 0) return 'text-emerald-400'
  if (valor < 0) return 'text-red-400'
  return 'text-amber-400'
}

export function corSaude(percentual: number): string {
  if (percentual >= 70) return 'text-emerald-400'
  if (percentual >= 50) return 'text-amber-400'
  return 'text-red-400'
}
