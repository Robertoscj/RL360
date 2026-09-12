import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { LoadingSpinner } from '@/components/ui/LoadingSpinner'

export function RotaProtegida() {
  const { autenticado, carregando } = useAuth()
  const location = useLocation()

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-rl-bg">
        <LoadingSpinner texto="Carregando sessão..." />
      </div>
    )
  }

  if (!autenticado) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  return <Outlet />
}

export function RotaPublica() {
  const { autenticado, carregando } = useAuth()

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-rl-bg">
        <LoadingSpinner />
      </div>
    )
  }

  if (autenticado) return <Navigate to="/" replace />

  return <Outlet />
}
