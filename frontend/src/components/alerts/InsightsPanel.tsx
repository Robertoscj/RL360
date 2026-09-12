import { useCallback, useEffect, useMemo, useRef, useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { Loader2, Radar, Send, Sparkles, Trash2 } from 'lucide-react'
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
    rotulo: 'ia-rotulo-urgente text-red-400',
  },
  Gargalo: {
    borda: 'border-l-amber-500',
    fundo: 'bg-amber-500/[0.07]',
    rotulo: 'ia-rotulo-gargalo text-amber-400',
  },
  Oportunidade: {
    borda: 'border-l-emerald-500',
    fundo: 'bg-emerald-500/[0.07]',
    rotulo: 'ia-rotulo-oportunidade text-emerald-400',
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

function TextoComNegrito({ texto }: { texto: string }) {
  const partes = texto.split(/(\*\*[^*]+\*\*)/g)
  return partes.map((parte, i) =>
    parte.startsWith('**') && parte.endsWith('**') ? (
      <strong key={i} className="font-semibold text-rl-heading">
        {parte.slice(2, -2)}
      </strong>
    ) : (
      <span key={i}>{parte}</span>
    ),
  )
}

export function InsightsPanel({ insights }: InsightsPanelProps) {
  const navigate = useNavigate()
  const { periodo } = usePeriodo()
  const [pergunta, setPergunta] = useState('')
  const [mensagens, setMensagens] = useState<MensagemIa[]>([])
  const [idConversa, setIdConversa] = useState<string | null>(obterIdConversaSalvo)
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
        const historico = await obterHistoricoIa(12)
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
        scrollChat()
      } catch {
        /* histórico opcional */
      }
    })()
  }, [scrollChat])

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
      setErro('')
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao limpar histórico')
    }
  }

  const visiveis = useMemo(() => mensagens.slice(-8), [mensagens])

  const onSubmit = (e: FormEvent) => {
    e.preventDefault()
    void enviar(pergunta)
  }

  return (
    <div className="glass-card flex h-[460px] max-h-[460px] flex-col overflow-hidden p-4">
      <div className="mb-3 flex shrink-0 items-center gap-2 border-b border-rl-border pb-3">
        <Sparkles className="h-4 w-4 text-emerald-400" />
        <p className="section-label">Insights Inteligentes</p>
      </div>

      <div className="min-h-0 flex-1 space-y-2 overflow-y-auto">
        {insights.map((insight) => {
          const e = estilos[insight.tipo] ?? estilos.Oportunidade
          return (
            <div
              key={insight.id}
              className={`rounded-lg border border-rl-border border-l-[3px] ${e.borda} ${e.fundo} p-2.5`}
            >
              <p className={`text-[9px] font-bold uppercase tracking-wide ${e.rotulo}`}>
                {tituloTipo(insight.tipo)}
              </p>
              <p className="mt-1 text-[11px] font-semibold leading-snug text-rl-heading">
                {insight.titulo}
              </p>
              <p className="mt-1 text-xs font-medium leading-relaxed text-rl-body">{insight.descricao}</p>
              <button
                type="button"
                onClick={() => insight.rotaAcao && navigate(insight.rotaAcao)}
                className={`mt-1.5 text-[10px] font-bold ${e.rotulo} hover:underline`}
              >
                {insight.textoBotao} →
              </button>
            </div>
          )
        })}
      </div>

      <div className="mt-3 flex shrink-0 flex-col border-t border-rl-border pt-3">
        <div className="mb-2 flex items-center justify-between">
          <p className="text-[10px] font-semibold text-rl-body">Conversa com a IA</p>
          <div className="flex items-center gap-2">
            <span
              className={`inline-flex h-6 w-6 items-center justify-center rounded-lg bg-emerald-500/15 text-emerald-400 ${
                carregando ? 'ia-radar-falando' : ''
              }`}
              title={carregando ? 'Respondendo...' : 'Radar'}
              aria-label={carregando ? 'IA respondendo' : 'Radar'}
            >
              <Radar className="h-3.5 w-3.5" strokeWidth={2.2} />
            </span>
            {mensagens.length > 0 && (
              <button
                type="button"
                onClick={() => void limparChat()}
                className="rounded p-1 text-rl-muted hover:bg-rl-surface hover:text-red-500"
                title="Limpar histórico"
              >
                <Trash2 className="h-3 w-3" />
              </button>
            )}
          </div>
        </div>

        <div ref={chatRef} className="ia-chat-area mb-2 h-[120px] space-y-2 overflow-y-auto overflow-x-hidden rounded-lg p-2">
          {visiveis.length === 0 && (
            <p className="px-1 py-4 text-center text-[10px] text-rl-muted">
              Pergunte sobre lucro, inadimplência ou conversão.
            </p>
          )}
          {visiveis
            .filter((m) => m.texto.length > 0 || m.papel === 'usuario' || (carregando && m.papel === 'assistente'))
            .map((msg) => (
            <div
              key={msg.id}
              className={msg.papel === 'usuario' ? 'ia-msg-usuario' : 'ia-msg-assistente'}
            >
              {msg.texto.split('\n').map((linha, i) => (
                <p key={i} className={i > 0 ? 'mt-1.5' : ''}>
                  <TextoComNegrito texto={linha} />
                  {carregando && msg.papel === 'assistente' && msg.texto === '' && i === 0 && (
                    <span className="ml-1 inline-block h-3 w-1 animate-pulse bg-emerald-400" />
                  )}
                </p>
              ))}
            </div>
          ))}
        </div>

        <div className="mb-2 flex flex-wrap gap-1">
          {SUGESTOES.map((s) => (
            <button
              key={s}
              type="button"
              onClick={() => void enviar(s)}
              disabled={carregando}
              className="ia-chip"
            >
              {s}
            </button>
          ))}
        </div>

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
