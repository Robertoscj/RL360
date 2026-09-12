import { useState, type FormEvent } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { Radar } from 'lucide-react'
import { useAuth } from '@/hooks/useAuth'
import { BotaoTema } from '@/components/layout/BotaoTema'

export function LoginPage() {
  const { entrar, autenticado } = useAuth()
  const location = useLocation()
  const [email, setEmail] = useState('ceo@rl360.com')
  const [senha, setSenha] = useState('rl360@2026')
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)

  const destino = (location.state as { from?: { pathname: string } })?.from?.pathname ?? '/'

  if (autenticado) return <Navigate to={destino} replace />

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setErro('')
    setCarregando(true)
    try {
      await entrar(email, senha)
    } catch (err) {
      setErro(err instanceof Error ? err.message : 'Erro ao entrar')
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="relative flex min-h-screen items-center justify-center bg-rl-bg p-4">
      <div className="absolute right-4 top-4">
        <BotaoTema />
      </div>
      <div className="w-full max-w-md">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-2xl bg-gradient-to-br from-emerald-500 to-teal-600 shadow-xl shadow-emerald-500/25">
            <Radar className="h-8 w-8 text-white" />
          </div>
          <h1 className="text-2xl font-black tracking-tight text-rl-heading">
            LUCRO<span className="gradient-text">360</span>
          </h1>
          <p className="mt-1 text-sm text-rl-muted">Radar de Lucro em Tempo Real</p>
        </div>

        <form onSubmit={handleSubmit} className="glass-card space-y-4 p-6">
          <div>
            <label htmlFor="email" className="mb-1.5 block text-xs font-medium text-rl-muted">
              Usuário
            </label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="input-field"
              required
              autoComplete="email"
            />
          </div>

          <div>
            <label htmlFor="senha" className="mb-1.5 block text-xs font-medium text-rl-muted">
              Senha
            </label>
            <input
              id="senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              className="input-field"
              required
              autoComplete="current-password"
            />
          </div>

          {erro && (
            <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
              {erro}
            </div>
          )}

          <button type="submit" disabled={carregando} className="btn-primary w-full">
            {carregando ? 'Entrando...' : 'Entrar'}
          </button>

          <p className="text-center text-[11px] text-rl-subtle">
            Demo: ceo@rl360.com / rl360@2026
          </p>
        </form>
      </div>
    </div>
  )
}
