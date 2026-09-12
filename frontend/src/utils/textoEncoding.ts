export function corrigirTexto(texto: string): string {
  if (!/[ÃÂ][\u0080-\u00BF]/.test(texto)) return texto

  try {
    const bytes = Uint8Array.from([...texto], (c) => c.charCodeAt(0) & 0xff)
    return new TextDecoder('utf-8').decode(bytes)
  } catch {
    return texto
  }
}

export function corrigirTextos<T>(valor: T): T {
  if (typeof valor === 'string') return corrigirTexto(valor) as T
  if (Array.isArray(valor)) return valor.map((item) => corrigirTextos(item)) as T
  if (valor && typeof valor === 'object') {
    const saida: Record<string, unknown> = {}
    for (const [chave, item] of Object.entries(valor)) {
      saida[chave] = corrigirTextos(item)
    }
    return saida as T
  }
  return valor
}
