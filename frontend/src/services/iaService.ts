import type { RespostaApi } from '@/types/api'
import type {
  EventoStreamIa,
  HistoricoIa,
  RequisicaoIaPergunta,
  RespostaIaPergunta,
} from '@/types/ia'
import { api } from './api'

const CHAVE_CONVERSA = 'rl360:ia:conversa'

function baseUrl() {
  return import.meta.env.VITE_API_URL ?? ''
}

function obterHeadersAuth(): Record<string, string> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' }
  try {
    const raw = localStorage.getItem('rl360_sessao')
    if (raw) {
      const sessao = JSON.parse(raw) as { token: string }
      headers.Authorization = `Bearer ${sessao.token}`
    }
  } catch {
    /* ignora */
  }
  return headers
}

export function obterIdConversaSalvo(): string | null {
  return localStorage.getItem(CHAVE_CONVERSA)
}

export function salvarIdConversa(id: string) {
  localStorage.setItem(CHAVE_CONVERSA, id)
}

export function limparIdConversaSalvo() {
  localStorage.removeItem(CHAVE_CONVERSA)
}

export async function obterHistoricoIa(limite = 50): Promise<HistoricoIa> {
  const { data } = await api.get<RespostaApi<HistoricoIa>>('/api/ia/historico', { params: { limite } })
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar histórico')
  return data.dados
}

export async function limparHistoricoIa(): Promise<void> {
  const { data } = await api.delete<RespostaApi<boolean>>('/api/ia/historico')
  if (!data.sucesso) throw new Error(data.mensagem || 'Erro ao limpar histórico')
  limparIdConversaSalvo()
}

export async function perguntarIa(requisicao: RequisicaoIaPergunta): Promise<RespostaIaPergunta> {
  const { data } = await api.post<RespostaApi<RespostaIaPergunta>>('/api/ia/perguntar', requisicao)
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao consultar IA')
  salvarIdConversa(data.dados.idConversa)
  return data.dados
}

export async function perguntarIaStream(
  requisicao: RequisicaoIaPergunta,
  callbacks: {
    onDelta: (texto: string) => void
    onDone: (meta: { modo: string; idConversa: string }) => void
    onError: (mensagem: string) => void
  },
  signal?: AbortSignal,
): Promise<void> {
  const response = await fetch(`${baseUrl()}/api/ia/perguntar/stream`, {
    method: 'POST',
    headers: obterHeadersAuth(),
    body: JSON.stringify(requisicao),
    signal,
  })

  if (!response.ok) {
    const erro = await response.text()
    callbacks.onError(erro || `Erro ${response.status}`)
    return
  }

  const reader = response.body?.getReader()
  if (!reader) {
    callbacks.onError('Streaming indisponível')
    return
  }

  const decoder = new TextDecoder()
  let buffer = ''

  while (true) {
    const { done, value } = await reader.read()
    if (done) break

    buffer += decoder.decode(value, { stream: true })
    const linhas = buffer.split('\n')
    buffer = linhas.pop() ?? ''

    for (const linha of linhas) {
      if (!linha.startsWith('data: ')) continue
      const payload = linha.slice(6).trim()
      if (!payload) continue

      let evt: EventoStreamIa
      try {
        evt = JSON.parse(payload) as EventoStreamIa
      } catch {
        continue
      }

      if (evt.tipo === 'delta' && evt.delta) callbacks.onDelta(evt.delta)
      if (evt.tipo === 'erro') callbacks.onError(evt.erro ?? 'Erro na IA')
      if (evt.tipo === 'done' && evt.idConversa) {
        salvarIdConversa(evt.idConversa)
        callbacks.onDone({ modo: evt.modo ?? 'Demo', idConversa: evt.idConversa })
      }
    }
  }
}
