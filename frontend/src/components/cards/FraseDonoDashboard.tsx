import { Link } from 'react-router-dom'
import { ChevronRight } from 'lucide-react'
import { montarFraseDono } from '@/utils/dashboardMetricas'

interface FraseDonoDashboardProps {
  risco: number
  oportunidade: number
  quantidadeAcoes: number
}

const estilos = {
  risco: 'border-red-500/25 bg-red-500/[0.06]',
  oportunidade: 'border-emerald-500/25 bg-emerald-500/[0.06]',
  neutro: 'border-rl-border bg-rl-surface/60',
}

const estilosAcao = {
  risco: 'text-red-400 hover:text-red-300',
  oportunidade: 'text-emerald-400 hover:text-emerald-300',
  neutro: 'text-rl-body hover:text-rl-heading',
}

export function FraseDonoDashboard({ risco, oportunidade, quantidadeAcoes }: FraseDonoDashboardProps) {
  const frase = montarFraseDono({ risco, oportunidade, quantidadeAcoes })

  return (
    <div className={`flex flex-wrap items-center justify-between gap-3 rounded-xl border px-4 py-3 ${estilos[frase.tom]}`}>
      <p className="max-w-3xl text-[14px] font-bold leading-snug text-rl-heading">{frase.texto}</p>
      <Link
        to={frase.rota}
        className={`inline-flex shrink-0 items-center gap-0.5 text-[12px] font-bold ${estilosAcao[frase.tom]}`}
      >
        {frase.acao} <ChevronRight className="h-3.5 w-3.5" />
      </Link>
    </div>
  )
}
