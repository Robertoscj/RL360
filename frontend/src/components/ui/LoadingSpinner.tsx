interface LoadingSpinnerProps {
  texto?: string
}

export function LoadingSpinner({ texto }: LoadingSpinnerProps) {
  return (
    <div className="flex flex-col items-center gap-3">
      <div className="h-10 w-10 animate-spin rounded-full border-2 border-emerald-500/30 border-t-emerald-400" />
      {texto && <p className="text-sm text-slate-400">{texto}</p>}
    </div>
  )
}
