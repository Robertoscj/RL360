import { Topbar } from '@/components/layout/Topbar'
import { Card } from '@/components/ui/Card'
import { Construction } from 'lucide-react'

interface PaginaPlaceholderProps {
  titulo: string
  descricao: string
}

export function PaginaPlaceholder({ titulo, descricao }: PaginaPlaceholderProps) {
  return (
    <div className="min-h-screen bg-rl-bg">
      <Topbar titulo={titulo} subtitulo={descricao} />
      <div className="p-5">
        <Card className="flex flex-col items-center justify-center py-20 text-center">
          <Construction className="mb-4 h-12 w-12 text-slate-600" />
          <h2 className="text-lg font-semibold text-slate-300">{titulo}</h2>
          <p className="mt-2 max-w-md text-sm text-slate-500">{descricao}</p>
          <p className="mt-4 text-xs text-emerald-500/70">Em construção — Parte 7</p>
        </Card>
      </div>
    </div>
  )
}
