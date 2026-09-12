import type {
  FaixaAtraso,
  RegistroInadimplencia,
  ResumoInadimplencia,
  SeveridadeInadimplencia,
} from '@/types/inadimplencia'

const FAIXAS: { rotulo: string; min: number; max: number; cor: string }[] = [
  { rotulo: '1–30 dias', min: 1, max: 30, cor: '#22c55e' },
  { rotulo: '31–60 dias', min: 31, max: 60, cor: '#f59e0b' },
  { rotulo: '61–90 dias', min: 61, max: 90, cor: '#f97316' },
  { rotulo: '90+ dias', min: 91, max: Infinity, cor: '#ef4444' },
]

export function calcularResumo(
  registros: RegistroInadimplencia[],
  faturamentoMes?: number,
): ResumoInadimplencia {
  const totalEmAtraso = registros.reduce((s, r) => s + r.valor, 0)
  const perdaProjetada = registros.reduce((s, r) => s + r.perdaProjetada, 0)
  const valorRecuperavel = totalEmAtraso - perdaProjetada
  const diasMedios =
    registros.length > 0
      ? registros.reduce((s, r) => s + r.diasEmAtraso, 0) / registros.length
      : 0
  const taxaRecuperacaoMedia =
    registros.length > 0
      ? registros.reduce((s, r) => s + r.probabilidadeRecuperacao, 0) / registros.length
      : 0
  const contasCriticas = registros.filter((r) => classificarSeveridade(r) === 'critico').length

  return {
    totalEmAtraso,
    perdaProjetada,
    valorRecuperavel,
    quantidadeContas: registros.length,
    contasCriticas,
    diasMedios,
    taxaRecuperacaoMedia,
    taxaInadimplenciaSobreFaturamento:
      faturamentoMes && faturamentoMes > 0 ? (totalEmAtraso / faturamentoMes) * 100 : null,
  }
}

export function agruparPorFaixa(registros: RegistroInadimplencia[]): FaixaAtraso[] {
  return FAIXAS.map(({ rotulo, min, max, cor }) => {
    const itens = registros.filter((r) => r.diasEmAtraso >= min && r.diasEmAtraso <= max)
    return {
      rotulo,
      valor: itens.reduce((s, r) => s + r.valor, 0),
      quantidade: itens.length,
      cor,
    }
  })
}

export function classificarSeveridade(registro: RegistroInadimplencia): SeveridadeInadimplencia {
  const prob = registro.probabilidadeRecuperacao <= 1
    ? registro.probabilidadeRecuperacao
    : registro.probabilidadeRecuperacao / 100

  if (registro.diasEmAtraso > 60 || prob < 0.35) return 'critico'
  if (registro.diasEmAtraso > 45 || prob < 0.5) return 'alto'
  if (registro.diasEmAtraso > 30 || prob < 0.65) return 'medio'
  return 'baixo'
}

export function rotuloSeveridade(severidade: SeveridadeInadimplencia): string {
  const mapa: Record<SeveridadeInadimplencia, string> = {
    critico: 'Crítico',
    alto: 'Alto',
    medio: 'Médio',
    baixo: 'Baixo',
  }
  return mapa[severidade]
}

export function corBadgeSeveridade(severidade: SeveridadeInadimplencia): string {
  const mapa: Record<SeveridadeInadimplencia, string> = {
    critico: 'bg-red-500/15 text-red-400 border-red-500/30',
    alto: 'bg-orange-500/15 text-orange-400 border-orange-500/30',
    medio: 'bg-amber-500/15 text-amber-400 border-amber-500/30',
    baixo: 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30',
  }
  return mapa[severidade]
}

export function pctProbabilidade(valor: number): number {
  return valor <= 1 ? valor * 100 : valor
}
