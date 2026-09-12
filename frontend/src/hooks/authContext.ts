import { createContext } from 'react'
import type { SessaoAuth } from '@/types/auth'

export interface AuthContextValue {
  sessao: SessaoAuth | null
  carregando: boolean
  autenticado: boolean
  entrar: (email: string, senha: string) => Promise<void>
  sair: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)
