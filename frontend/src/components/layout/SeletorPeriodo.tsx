import { useCallback, useEffect, useRef, useState } from 'react'
import { Calendar, ChevronDown, ChevronLeft, ChevronRight } from 'lucide-react'
import { usePeriodo } from '@/hooks/usePeriodo'
import {
  NOMES_DIAS_CURTOS,
  NOMES_MESES,
  diasDoCalendario,
  formatarPeriodo,
  inicioDoDia,
  mesmoDia,
  estaNoIntervalo,
  normalizarIntervalo,
  paraIsoData,
} from '@/utils/datas'

const PRESETS = [
  { id: 'hoje', rotulo: 'Hoje' },
  { id: 'ontem', rotulo: 'Ontem' },
  { id: 'ultimos7', rotulo: 'Últimos 7 dias' },
  { id: 'ultimos30', rotulo: 'Últimos 30 dias' },
  { id: 'ultimos90', rotulo: 'Últimos 90 dias' },
  { id: 'esteMes', rotulo: 'Este mês' },
  { id: 'mesAnterior', rotulo: 'Mês anterior' },
  { id: 'esteAno', rotulo: 'Este ano' },
]

export function SeletorPeriodo() {
  const { periodo, aplicarPeriodo, aplicarPredefinido } = usePeriodo()
  const [aberto, setAberto] = useState(false)
  const [mesVisivel, setMesVisivel] = useState(() => periodo.inicio)
  const [selecaoInicio, setSelecaoInicio] = useState<Date | null>(null)
  const [selecaoFim, setSelecaoFim] = useState<Date | null>(null)
  const [hoverDia, setHoverDia] = useState<Date | null>(null)
  const containerRef = useRef<HTMLDivElement>(null)

  const rascunhoInicio = selecaoInicio ?? periodo.inicio
  const rascunhoFim =
    selecaoInicio && !selecaoFim && hoverDia
      ? hoverDia
      : (selecaoFim ?? periodo.fim)

  const abrir = useCallback(() => {
    setSelecaoInicio(periodo.inicio)
    setSelecaoFim(periodo.fim)
    setMesVisivel(periodo.inicio)
    setAberto(true)
  }, [periodo])

  const fechar = useCallback(() => {
    setAberto(false)
    setSelecaoInicio(null)
    setSelecaoFim(null)
    setHoverDia(null)
  }, [])

  const confirmar = useCallback(() => {
    aplicarPeriodo(rascunhoInicio, rascunhoFim)
    fechar()
  }, [aplicarPeriodo, rascunhoInicio, rascunhoFim, fechar])

  const clicarDia = useCallback(
    (dia: Date) => {
      if (!selecaoInicio || selecaoFim) {
        setSelecaoInicio(dia)
        setSelecaoFim(null)
        return
      }
      const { inicio, fim } = normalizarIntervalo(selecaoInicio, dia)
      setSelecaoInicio(inicio)
      setSelecaoFim(fim)
    },
    [selecaoInicio, selecaoFim],
  )

  const aplicarPreset = useCallback(
    (id: string) => {
      aplicarPredefinido(id)
      fechar()
    },
    [aplicarPredefinido, fechar],
  )

  useEffect(() => {
    if (!aberto) return

    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') fechar()
    }
    const onClick = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) fechar()
    }

    document.addEventListener('keydown', onKey)
    document.addEventListener('mousedown', onClick)
    return () => {
      document.removeEventListener('keydown', onKey)
      document.removeEventListener('mousedown', onClick)
    }
  }, [aberto, fechar])

  const ano = mesVisivel.getFullYear()
  const mes = mesVisivel.getMonth()
  const dias = diasDoCalendario(ano, mes)

  const intervaloAtivo = normalizarIntervalo(rascunhoInicio, rascunhoFim)

  return (
    <div ref={containerRef} className="relative">
      <button
        type="button"
        onClick={() => (aberto ? fechar() : abrir())}
        className="btn-ghost flex items-center gap-2 text-xs md:flex"
        aria-expanded={aberto}
        aria-haspopup="dialog"
      >
        <Calendar className="h-3.5 w-3.5 text-rl-body" />
        <span className="hidden font-medium text-rl-body sm:inline">Período:</span>
        <span className="max-w-[180px] truncate font-semibold text-rl-heading sm:max-w-none">
          {formatarPeriodo(periodo.inicio, periodo.fim)}
        </span>
        <ChevronDown className={`h-3.5 w-3.5 text-rl-muted transition ${aberto ? 'rotate-180' : ''}`} />
      </button>

      {aberto && (
        <div
          role="dialog"
          aria-label="Selecionar período"
          className="absolute right-0 top-full z-50 mt-2 flex w-[min(100vw-2rem,640px)] overflow-hidden rounded-xl border border-rl-border bg-rl-popover shadow-2xl"
        >
          {/* Presets */}
          <div className="hidden w-[168px] shrink-0 border-r border-rl-border p-2 sm:block">
            <p className="section-label px-2 py-1.5">
              Atalhos
            </p>
            {PRESETS.map(({ id, rotulo }) => (
              <button
                key={id}
                type="button"
                onClick={() => aplicarPreset(id)}
                className="w-full rounded-lg px-2 py-2 text-left text-xs font-medium text-rl-heading transition hover:bg-rl-card hover:text-emerald-600"
              >
                {rotulo}
              </button>
            ))}
          </div>

          {/* Calendário */}
          <div className="min-w-0 flex-1 p-4">
            <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
              <div>
                <p className="section-label">Período personalizado</p>
                <p className="text-xs font-semibold text-rl-heading">
                  {formatarPeriodo(intervaloAtivo.inicio, intervaloAtivo.fim)}
                </p>
              </div>
              <div className="flex items-center gap-1">
                <button
                  type="button"
                  onClick={() => setMesVisivel(new Date(ano, mes - 1, 1))}
                  className="rounded-lg p-1.5 text-rl-muted hover:bg-rl-card hover:text-rl-heading"
                  aria-label="Mês anterior"
                >
                  <ChevronLeft className="h-4 w-4" />
                </button>
                <span className="min-w-[120px] text-center text-sm font-semibold text-rl-heading">
                  {NOMES_MESES[mes]} {ano}
                </span>
                <button
                  type="button"
                  onClick={() => setMesVisivel(new Date(ano, mes + 1, 1))}
                  className="rounded-lg p-1.5 text-rl-muted hover:bg-rl-card hover:text-rl-heading"
                  aria-label="Próximo mês"
                >
                  <ChevronRight className="h-4 w-4" />
                </button>
              </div>
            </div>

            {/* Presets mobile */}
            <div className="mb-3 flex gap-1 overflow-x-auto pb-1 sm:hidden">
              {PRESETS.slice(0, 5).map(({ id, rotulo }) => (
                <button
                  key={id}
                  type="button"
                  onClick={() => aplicarPreset(id)}
                  className="shrink-0 rounded-full border border-rl-border px-2.5 py-1 text-[10px] text-rl-body hover:border-emerald-500/40 hover:text-emerald-600"
                >
                  {rotulo}
                </button>
              ))}
            </div>

            <div className="grid grid-cols-7 gap-0.5">
              {NOMES_DIAS_CURTOS.map((d, i) => (
                <div key={`${d}-${i}`} className="py-1 text-center text-[11px] font-bold text-rl-body">
                  {d}
                </div>
              ))}
              {dias.map((dia, i) => {
                if (!dia) return <div key={`empty-${i}`} />

                const hoje = mesmoDia(dia, new Date())
                const inicio = intervaloAtivo.inicio
                const fim = intervaloAtivo.fim
                const noIntervalo = estaNoIntervalo(dia, inicio, fim)
                const ehInicio = mesmoDia(dia, inicio)
                const ehFim = mesmoDia(dia, fim)
                const futuro = inicioDoDia(dia).getTime() > inicioDoDia(new Date()).getTime()

                return (
                  <button
                    key={dia.toISOString()}
                    type="button"
                    disabled={futuro}
                    onClick={() => clicarDia(dia)}
                    onMouseEnter={() => selecaoInicio && !selecaoFim && setHoverDia(dia)}
                    onMouseLeave={() => setHoverDia(null)}
                    className={[
                      'relative h-9 rounded-lg text-xs font-medium transition',
                      futuro ? 'cursor-not-allowed text-rl-subtle' : 'text-rl-heading hover:bg-rl-card',
                      noIntervalo && !futuro ? 'bg-emerald-500/15 text-emerald-300' : '',
                      (ehInicio || ehFim) && !futuro ? 'bg-emerald-500 text-white hover:bg-emerald-400' : '',
                      hoje && !noIntervalo ? 'ring-1 ring-emerald-500/40' : '',
                    ].join(' ')}
                  >
                    {dia.getDate()}
                  </button>
                )
              })}
            </div>

            {/* Inputs manuais */}
            <div className="mt-4 grid grid-cols-2 gap-3">
              <label className="block">
                <span className="mb-1 block text-[11px] font-bold uppercase text-rl-heading">De</span>
                <input
                  type="date"
                  value={paraIsoData(intervaloAtivo.inicio)}
                  onChange={(e) => {
                    if (!e.target.value) return
                    const [y, m, d] = e.target.value.split('-').map(Number)
                    setSelecaoInicio(new Date(y, m - 1, d))
                    setSelecaoFim(intervaloAtivo.fim)
                  }}
                  max={new Date().toISOString().split('T')[0]}
                  className="input-field text-xs"
                />
              </label>
              <label className="block">
                <span className="mb-1 block text-[11px] font-bold uppercase text-rl-heading">Até</span>
                <input
                  type="date"
                  value={paraIsoData(intervaloAtivo.fim)}
                  onChange={(e) => {
                    if (!e.target.value) return
                    const [y, m, d] = e.target.value.split('-').map(Number)
                    setSelecaoInicio(intervaloAtivo.inicio)
                    setSelecaoFim(new Date(y, m - 1, d))
                  }}
                  max={new Date().toISOString().split('T')[0]}
                  className="input-field text-xs"
                />
              </label>
            </div>

            <div className="mt-4 flex justify-end gap-2 border-t border-rl-border pt-3">
              <button type="button" onClick={fechar} className="btn-ghost px-4 py-2 text-xs">
                Cancelar
              </button>
              <button type="button" onClick={confirmar} className="btn-primary px-4 py-2 text-xs">
                Aplicar período
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
