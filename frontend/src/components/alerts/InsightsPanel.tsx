import { useCallback, useEffect, useRef, useState, type FormEvent } from 'react'
import { Loader2, Send, Sparkles, Trash2 } from 'lucide-react'
import { usePeriodo } from '@/hooks/usePeriodo'
import {
  limparHistoricoIa,
  obterHistoricoIa,
  obterIdConversaSalvo,
  perguntarIaStream,
} from '@/services/iaService'
import { paraIsoData } from '@/utils/datas'
import type { InsightInteligente } from '@/types/dashboard'
import type { MensagemIa } from '@/types/ia'

interface InsightsPanelProps {
  insights: InsightInteligente[]
}

const estilos: Record<string, { borda: string; fundo: string; rotulo: string }> = {
  AcaoUrgente: {
    borda: 'border-l-red-500',
    fundo: 'bg-red-500/[0.07]',
    rotulo: 'text-red-400',
  },
  Gargalo: {
    borda: 'border-l-amber-500',
    fundo: 'bg-amber-500/[0.07]',
    rotulo: 'text-amber-400',
  },
  Oportunidade: {
    borda: 'border-l-emerald-500',
    fundo: 'bg-emerald-500/[0.07]',
    rotulo: 'text-emerald-400',
  },
}

const SUGESTOES = [
  'Por que a conversão caiu?',
  'Como reduzir a inadimplência?',
  'Qual o maior risco agora?',
]

function tituloTipo(tipo: string) {
  if (tipo === 'AcaoUrgente') return 'Ação urgente recomendada'
  if (tipo === 'Gargalo') return 'Gargalo identificado'
  if (tipo === 'Oportunidade') return 'Oportunidade real'
  return tipo
}

function novoId() {
  return crypto.randomUUID?.() ?? `${Date.now()}-${Math.random()}`
}

function rotuloModo(modo?: string) {
  if (modo === 'AzureOpenAI') return 'Azure GPT'
  if (modo === 'OpenAI') return 'GPT'
  if (modo === 'Demo') return 'Demo'
  return modo ?? ''
}

export function InsightsPanel({ insights }: InsightsPanelProps) {
  const { periodo } = usePeriodo()
  const [pergunta, setPergunta] = useState('')
  const [mensagens, setMensagens] = useState<MensagemIa[]>([])
  const [idConversa, setIdConversa] = useState<string | null>(obterIdConversaSalvo)
  const [modoAtual, setModoAtual] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')
  const chatRef = useRef<HTMLDivElement>(null)
  const abortRef = useRef<AbortController | null>(null)

  const scrollChat = useCallback(() => {
    requestAnimationFrame(() => {
      chatRef.current?.scrollTo({ top: chatRef.current.scrollHeight, behavior: 'smooth' })
    })
  }, [])

  useEffect(() => {
    void (async () => {
      try {
        const historico = await obterHistoricoIa(40)
        if (historico.mensagens.length === 0) return

        setIdConversa(historico.idConversa || obterIdConversaSalvo())
        setMensagens(
          historico.mensagens.map((m) => ({
            id: m.id,
            idConversa: m.idConversa,
            papel: m.papel === 'usuario' ? 'usuario' : 'assistente',
            texto: m.conteudo,
            modo: m.modo !== 'Usuario' ? m.modo : undefined,
            criadoEmUtc: m.criadoEmUtc,
          })),
        )
        const ultimoModo = historico.mensagens.findLast((m) => m.modo && m.modo !== 'Usuario')?.modo
        if (ultimoModo) setModoAtual(ultimoModo)
      } catch {
        /* histórico opcional */
      }
    })()
  }, [])

  const enviar = async (texto: string) => {
    const perguntaLimpa = texto.trim()
    if (!perguntaLimpa || carregando) return

    abortRef.current?.abort()
    abortRef.current = new AbortController()

    setErro('')
    setPergunta('')
    setCarregando(true)

    const msgUsuario: MensagemIa = { id: novoId(), idConversa: idConversa ?? '', papel: 'usuario', texto: perguntaLimpa }
    const msgAssistenteId = novoId()
    setMensagens((prev) => [...prev, msgUsuario, { id: msgAssistenteId, idConversa: idConversa ?? '', papel: 'assistente', texto: '' }])
    scrollChat()

    try {
      await perguntarIaStream(
        {
          pergunta: perguntaLimpa,
          inicio: paraIsoData(periodo.inicio),
          fim: paraIsoData(periodo.fim),
          idConversa: idConversa ?? undefined,
        },
        {
          onDelta: (delta) => {
            setMensagens((prev) =>
              prev.map((m) => (m.id === msgAssistenteId ? { ...m, texto: m.texto + delta } : m)),
            )
            scrollChat()
          },
          onDone: ({ modo, idConversa: novoIdConversa }) => {
            setIdConversa(novoIdConversa)
            setModoAtual(modo)
            setMensagens((prev) =>
              prev.map((m) => (m.id === msgAssistenteId ? { ...m, modo, idConversa: novoIdConversa } : m)),
            )
          },
          onError: (mensagem) => setErro(mensagem),
        },
        abortRef.current.signal,
      )
    } catch (err) {
      if (err instanceof Error && err.name !== 'AbortError') {
        setErro(err.message || 'Não foi possível obter resposta da IA')
      }
      setMensagens((prev) => prev.filter((m) => m.id !== msgAssistenteId || m.texto.length > 0))
    } finally {
      setCarregando(false)
    }
  }

  const limparChat = async () => {
    try {
      await limparHistoricoIa()
      setMensagens([])
      setIdConversa(null)
      setModoAtual(null)
      setErro('')
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao limpar histórico')
    }
  }

  const onSubmit = (e: FormEvent) => {
    e.preventDefault()
    void enviar(pergunta)
  }

  return (
    <div className="glass-card flex min-h-[460px] flex-col p-4">
      <div className="mb-4 flex items-center gap-2 border-b border-rl-border pb-3">
        <Sparkles className="h-4 w-4 text-emerald-400" />
        <p className="text-[10px] font-bold uppercase tracking-wide text-slate-400">Insights Inteligentes</p>
      </div>

      <div className="flex-1 space-y-3 overflow-y-auto">
        {insights.map((insight) => {
          const e = estilos[insight.tipo] ?? estilos.Oportunidade
          return (
            <div
              key={insight.id}
              className={`rounded-lg border border-rl-border border-l-[3px] ${e.borda} ${e.fundo} p-3`}
            >
              <p className={`text-[9px] font-bold uppercase tracking-wide ${e.rotulo}`}>
                {tituloTipo(insight.tipo)}
              </p>
              <p className="mt-1.5 text-[11px] font-semibold leading-snug text-slate-200">
                {insight.titulo}
              </p>
              <p className="mt-1 text-[10px] leading-relaxed text-slate-500">{insight.descricao}</p>
              <button type="button" className={`mt-2 text-[10px] font-bold ${e.rotulo} hover:underline`}>
                {insight.textoBotao} →
              </button>
            </div>
          )
        })}
      </div>

      <div className="mt-4 border-t border-rl-border pt-3">
        <div className="mb-2 flex items-center justify-between">
          <p className="text-[10px] font-semibold text-slate-500">Pergunte para a IA</p>
          <div className="flex items-center gap-2">
            {modoAtual && (
              <span className="rounded bg-rl-surface px-1.5 py-0.5 text-[9px] text-slate-500">
                {rotuloModo(modoAtual)}
              </span>
            )}
            {mensagens.length > 0 && (
              <button
                type="button"
                onClick={() => void limparChat()}
                className="rounded p-1 text-slate-600 hover:bg-rl-surface hover:text-red-400"
                title="Limpar histórico"
              >
                <Trash2 className="h-3 w-3" />
              </button>
            )}
          </div>
        </div>

        {mensagens.length > 0 && (
          <div ref={chatRef} className="mb-3 max-h-40 space-y-2 overflow-y-auto rounded-lg bg-rl-surface/60 p-2">
            {mensagens.filter((m) => m.texto.length > 0 || m.papel === 'usuario').map((msg) => (
              <div
                key={msg.id}
                className={`rounded-lg px-2.5 py-2 text-[10px] leading-relaxed ${
                  msg.papel === 'usuario'
                    ? 'ml-4 bg-emerald-500/10 text-emerald-100'
                    : 'mr-2 bg-rl-card text-slate-300'
                }`}
              >
                {msg.texto.split('\n').map((linha, i) => (
                  <p key={i} className={i > 0 ? 'mt-1' : ''}>
                    {linha}
                    {carregando && msg.papel === 'assistente' && msg.texto === '' && i === 0 && (
                      <span className="ml-1 inline-block h-3 w-1 animate-pulse bg-emerald-400" />
                    )}
                  </p>
                ))}
              </div>
            ))}
          </div>
        )}

        {mensagens.length === 0 && (
          <div className="mb-2 flex flex-wrap gap-1">
            {SUGESTOES.map((s) => (
              <button
                key={s}
                type="button"
                onClick={() => void enviar(s)}
                disabled={carregando}
                className="rounded-full border border-rl-border px-2 py-0.5 text-[9px] text-slate-500 transition hover:border-emerald-500/40 hover:text-emerald-400 disabled:opacity-50"
              >
                {s}
              </button>
            ))}
          </div>
        )}

        <form onSubmit={onSubmit} className="relative">
          <input
            type="text"
            value={pergunta}
            onChange={(e) => setPergunta(e.target.value)}
            placeholder="Ex: Por que a conversão caiu?"
            className="input-field pr-10 text-[11px]"
            disabled={carregando}
            maxLength={500}
          />
          <button
            type="submit"
            disabled={carregando || !pergunta.trim()}
            className="absolute right-2 top-1/2 -translate-y-1/2 rounded-md bg-emerald-500 p-1.5 text-white transition hover:bg-emerald-400 disabled:opacity-40"
            aria-label="Enviar pergunta"
          >
            {carregando ? <Loader2 className="h-3 w-3 animate-spin" /> : <Send className="h-3 w-3" />}
          </button>
        </form>

        {erro && <p className="mt-2 text-[10px] text-red-400">{erro}</p>}
      </div>
    </div>
  )
}
