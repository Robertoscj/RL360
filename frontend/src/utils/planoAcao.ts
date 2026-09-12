import type { ItemPlanoAcao } from '@/types/dashboard'

export function rotaParaPlanoAcao(item: Pick<ItemPlanoAcao, 'titulo' | 'justificativa'>): string {
  const texto = `${item.titulo} ${item.justificativa}`.toLowerCase()

  if (texto.includes('inadimpl') || texto.includes('cobrança') || texto.includes('cobranca')) {
    return '/inadimplencia'
  }
  if (texto.includes('funil') || texto.includes('conversão') || texto.includes('conversao') || texto.includes('cross-sell') || texto.includes('campanha') || texto.includes('produto')) {
    return '/vendas'
  }
  if (texto.includes('expansão') || texto.includes('expansao') || texto.includes('clientes')) {
    return '/clientes'
  }
  if (texto.includes('gargalo') || texto.includes('crédito') || texto.includes('credito') || texto.includes('proposta')) {
    return '/gargalos'
  }
  if (texto.includes('sdr') || texto.includes('equipe') || texto.includes('região') || texto.includes('regiao')) {
    return '/equipe'
  }
  return '/'
}

export function rotuloRota(rota: string): string {
  const rotulos: Record<string, string> = {
    '/inadimplencia': 'Ver inadimplência',
    '/vendas': 'Ver vendas',
    '/clientes': 'Ver clientes',
    '/gargalos': 'Ver gargalos',
    '/equipe': 'Ver equipe',
    '/': 'Ver dashboard',
  }
  return rotulos[rota] ?? 'Ver detalhes'
}

export function estiloPrioridadePlano(prioridade: string) {
  const p = prioridade.toLowerCase()
  if (p.includes('urgent')) {
    return {
      badge: 'bg-red-500/15 text-red-500 border-red-500/30',
      borda: 'border-l-red-500',
      fundo: 'bg-red-500/[0.04]',
    }
  }
  if (p.includes('alta')) {
    return {
      badge: 'bg-amber-500/15 text-amber-600 border-amber-500/30',
      borda: 'border-l-amber-500',
      fundo: 'bg-amber-500/[0.04]',
    }
  }
  return {
    badge: 'bg-emerald-500/15 text-emerald-600 border-emerald-500/30',
    borda: 'border-l-emerald-500',
    fundo: 'bg-emerald-500/[0.04]',
  }
}

export function rotuloPrioridadePlano(prioridade: string): string {
  const p = prioridade.toLowerCase()
  if (p.includes('urgent')) return 'Urgente'
  if (p.includes('alta')) return 'Alta'
  if (p.includes('media') || p.includes('média')) return 'Média'
  return prioridade
}

export function ordenarPlanoAcao(itens: ItemPlanoAcao[]): ItemPlanoAcao[] {
  const peso = (prioridade: string) => {
    const p = prioridade.toLowerCase()
    if (p.includes('urgent')) return 3
    if (p.includes('alta')) return 2
    return 1
  }
  return [...itens].sort((a, b) => {
    const diff = peso(b.prioridade) - peso(a.prioridade)
    if (diff !== 0) return diff
    return b.impactoEsperado - a.impactoEsperado
  })
}
