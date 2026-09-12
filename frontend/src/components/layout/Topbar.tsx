import { LogOut, RefreshCw } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { BotaoTema } from '@/components/layout/BotaoTema'
import { CentralNotificacoes } from '@/components/layout/CentralNotificacoes'
import { SeletorPeriodo } from '@/components/layout/SeletorPeriodo'

interface TopbarProps {
  titulo?: string
  subtitulo?: string
  onAtualizar?: () => void
  atualizando?: boolean
}

export function Topbar({
  titulo = 'Radar de Resultados',
  subtitulo = 'Visão em tempo real do que impacta seu lucro',
  onAtualizar,
  atualizando,
}: TopbarProps) {
  const { sessao, sair } = useAuth()
  const navigate = useNavigate()
  const usuario = sessao?.usuario

  const handleSair = () => {
    sair()
    navigate('/login', { replace: true })
  }

  return (
    <header className="flex h-[60px] items-center justify-between border-b border-rl-border bg-rl-bg px-5">
      <div>
        <h1 className="text-sm font-bold uppercase tracking-wide text-rl-heading">{titulo}</h1>
        <p className="text-xs font-medium text-rl-body">{subtitulo}</p>
      </div>

      <div className="flex items-center gap-3">
        <BotaoTema compacto />
        {onAtualizar && (
          <button
            type="button"
            onClick={onAtualizar}
            disabled={atualizando}
            className="btn-ghost p-2"
            title="Atualizar"
          >
            <RefreshCw className={`h-4 w-4 ${atualizando ? 'animate-spin' : ''}`} />
          </button>
        )}

        <CentralNotificacoes />

        <div className="hidden items-center gap-2 sm:flex">
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-gradient-to-br from-emerald-500 to-teal-600 text-xs font-bold text-white">
            {usuario?.nome.charAt(0) ?? '?'}
          </div>
          <div>
            <p className="text-xs font-semibold text-rl-body">{usuario?.nome}</p>
            <p className="text-[10px] text-rl-muted">{usuario?.perfil ?? 'Diretor Executivo'}</p>
          </div>
        </div>

        <button
          type="button"
          onClick={handleSair}
          className="btn-ghost flex items-center gap-1.5 px-2 py-1.5 text-[11px] font-semibold text-slate-400 hover:text-red-400"
          title="Sair da aplicação"
        >
          <LogOut className="h-4 w-4" />
          <span className="hidden md:inline">Sair</span>
        </button>

        <SeletorPeriodo />
      </div>
    </header>
  )
}
