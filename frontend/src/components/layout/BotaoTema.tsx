import { Moon, Sun } from 'lucide-react'
import { useTheme } from '@/hooks/useTheme'

interface Props {
  compacto?: boolean
}

export function BotaoTema({ compacto = false }: Props) {
  const { tema, alternarTema } = useTheme()
  const ehClaro = tema === 'light'

  return (
    <button
      type="button"
      onClick={alternarTema}
      className="btn-ghost flex items-center gap-1.5 px-2 py-1.5 text-[11px] font-semibold text-rl-muted hover:text-rl-heading"
      title={ehClaro ? 'Ativar modo escuro' : 'Ativar modo claro'}
      aria-label={ehClaro ? 'Ativar modo escuro' : 'Ativar modo claro'}
    >
      {ehClaro ? <Moon className="h-4 w-4" /> : <Sun className="h-4 w-4" />}
      {!compacto && <span className="hidden md:inline">{ehClaro ? 'Escuro' : 'Claro'}</span>}
    </button>
  )
}
