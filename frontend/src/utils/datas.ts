export interface IntervaloDatas {
  inicio: Date
  fim: Date
}

export function inicioDoMes(data = new Date()): Date {
  return new Date(data.getFullYear(), data.getMonth(), 1)
}

export function fimDoMes(data = new Date()): Date {
  return new Date(data.getFullYear(), data.getMonth() + 1, 0)
}

export function inicioDoDia(data: Date): Date {
  return new Date(data.getFullYear(), data.getMonth(), data.getDate())
}

export function mesmoDia(a: Date, b: Date): boolean {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  )
}

export function estaNoIntervalo(dia: Date, inicio: Date, fim: Date): boolean {
  const t = inicioDoDia(dia).getTime()
  const i = inicioDoDia(inicio).getTime()
  const f = inicioDoDia(fim).getTime()
  return t >= Math.min(i, f) && t <= Math.max(i, f)
}

export function formatarDataCurta(data: Date): string {
  return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export function formatarPeriodo(inicio: Date, fim: Date): string {
  return `${formatarDataCurta(inicio)} até ${formatarDataCurta(fim)}`
}

export function paraIsoData(data: Date): string {
  const y = data.getFullYear()
  const m = String(data.getMonth() + 1).padStart(2, '0')
  const d = String(data.getDate()).padStart(2, '0')
  return `${y}-${m}-${d}`
}

export function deIsoData(iso: string): Date {
  const [y, m, d] = iso.split('-').map(Number)
  return new Date(y, m - 1, d)
}

export function diasDoCalendario(ano: number, mes: number): (Date | null)[] {
  const primeiro = new Date(ano, mes, 1)
  const ultimo = new Date(ano, mes + 1, 0)
  const offset = primeiro.getDay()
  const cells: (Date | null)[] = Array.from({ length: offset }, () => null)

  for (let d = 1; d <= ultimo.getDate(); d++) {
    cells.push(new Date(ano, mes, d))
  }

  while (cells.length % 7 !== 0) cells.push(null)
  return cells
}

export const NOMES_MESES = [
  'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
  'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro',
]

export const NOMES_DIAS_CURTOS = ['D', 'S', 'T', 'Q', 'Q', 'S', 'S']

export function periodoPredefinido(tipo: string): IntervaloDatas {
  const hoje = inicioDoDia(new Date())

  switch (tipo) {
    case 'hoje':
      return { inicio: hoje, fim: hoje }
    case 'ontem': {
      const ontem = new Date(hoje)
      ontem.setDate(ontem.getDate() - 1)
      return { inicio: ontem, fim: ontem }
    }
    case 'ultimos7': {
      const inicio = new Date(hoje)
      inicio.setDate(inicio.getDate() - 6)
      return { inicio, fim: hoje }
    }
    case 'ultimos30': {
      const inicio = new Date(hoje)
      inicio.setDate(inicio.getDate() - 29)
      return { inicio, fim: hoje }
    }
    case 'ultimos90': {
      const inicio = new Date(hoje)
      inicio.setDate(inicio.getDate() - 89)
      return { inicio, fim: hoje }
    }
    case 'mesAnterior': {
      const ref = new Date(hoje.getFullYear(), hoje.getMonth() - 1, 1)
      return { inicio: inicioDoMes(ref), fim: fimDoMes(ref) }
    }
    case 'esteAno':
      return { inicio: new Date(hoje.getFullYear(), 0, 1), fim: hoje }
    case 'esteMes':
    default:
      return { inicio: inicioDoMes(hoje), fim: fimDoMes(hoje) }
  }
}

export function normalizarIntervalo(inicio: Date, fim: Date): IntervaloDatas {
  const i = inicioDoDia(inicio)
  const f = inicioDoDia(fim)
  return i.getTime() <= f.getTime() ? { inicio: i, fim: f } : { inicio: f, fim: i }
}
