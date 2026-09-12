import type { Alerta } from '@/types/alerta'

export function rotaParaAlerta(alerta: Pick<Alerta, 'titulo' | 'fatorRelacionado'>): string {
  const fator = alerta.fatorRelacionado?.toLowerCase() ?? ''
  if (fator.includes('inadimpl')) return '/inadimplencia'
  if (fator.includes('gargalo')) return '/gargalos'
  if (fator.includes('conversao')) return '/vendas'
  if (fator.includes('expansao')) return '/clientes'
  if (fator.includes('vendasnovas')) return '/vendas'
  if (fator.includes('produtividade')) return '/equipe'

  const titulo = alerta.titulo.toLowerCase()
  if (titulo.includes('inadimpl') || titulo.includes('risco de perda')) return '/inadimplencia'
  if (titulo.includes('conversão') || titulo.includes('conversao')) return '/vendas'
  if (titulo.includes('aprovação') || titulo.includes('gargalo')) return '/gargalos'
  if (titulo.includes('oportunidade') || titulo.includes('região')) return '/clientes'
  if (titulo.includes('produto')) return '/vendas'
  return '/'
}

export function tempoRelativoAlerta(iso: string): string {
  const diffMs = Date.now() - new Date(iso).getTime()
  const minutos = Math.floor(diffMs / 60_000)
  if (minutos < 1) return 'agora'
  if (minutos < 60) return `há ${minutos} min`
  const horas = Math.floor(minutos / 60)
  if (horas < 24) return `há ${horas}h`
  const dias = Math.floor(horas / 24)
  return `há ${dias}d`
}

export function estiloSeveridadeAlerta(severidade: string) {
  const s = severidade.toLowerCase()
  if (s.includes('critico') || s.includes('crítico')) {
    return { borda: 'border-l-red-500', fundo: 'bg-red-500/[0.06]', rotulo: 'text-red-500' }
  }
  if (s.includes('info') || s.includes('positivo')) {
    return { borda: 'border-l-emerald-500', fundo: 'bg-emerald-500/[0.06]', rotulo: 'text-emerald-600' }
  }
  return { borda: 'border-l-amber-500', fundo: 'bg-amber-500/[0.06]', rotulo: 'text-amber-600' }
}
