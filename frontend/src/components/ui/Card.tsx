import type { ReactNode } from 'react'

interface CardProps {
  children: ReactNode
  className?: string
  titulo?: string
  subtitulo?: string
}

export function Card({ children, className = '', titulo, subtitulo }: CardProps) {
  return (
    <div className={`glass-card p-5 ${className}`}>
      {(titulo || subtitulo) && (
        <div className="mb-4">
          {titulo && <h3 className="text-sm font-semibold text-slate-200">{titulo}</h3>}
          {subtitulo && <p className="mt-0.5 text-xs text-slate-500">{subtitulo}</p>}
        </div>
      )}
      {children}
    </div>
  )
}
