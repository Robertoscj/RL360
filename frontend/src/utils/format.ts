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
