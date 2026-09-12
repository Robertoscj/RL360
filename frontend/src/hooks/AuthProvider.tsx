import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { AuthContext, type AuthContextValue } from '@/hooks/authContext'
import type { SessaoAuth } from '@/types/auth'
import { configurarInterceptorsAuth } from '@/services/api'
import { entrar as apiEntrar, obterUsuarioAtual, renovarToken } from '@/services/authService'

const CHAVE_SESSAO = 'rl360_sessao'

function carregarSessao(): SessaoAuth | null {
  try {
    const raw = localStorage.getItem(CHAVE_SESSAO)
    return raw ? (JSON.parse(raw) as SessaoAuth) : null
  } catch {
    return null
  }
}

function salvarSessao(sessao: SessaoAuth | null) {
  if (sessao) localStorage.setItem(CHAVE_SESSAO, JSON.stringify(sessao))
  else localStorage.removeItem(CHAVE_SESSAO)
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [sessao, setSessao] = useState<SessaoAuth | null>(() => carregarSessao())
  const [carregando, setCarregando] = useState(true)

  const sair = useCallback(() => {
    setSessao(null)
    salvarSessao(null)
  }, [])

  const aplicarResposta = useCallback((resposta: {
    token: string
    refreshToken: string
    expiraEmUtc: string
    refreshExpiraEmUtc: string
    usuario: SessaoAuth['usuario']
  }) => {
    const nova: SessaoAuth = {
      token: resposta.token,
      refreshToken: resposta.refreshToken,
      expiraEmUtc: resposta.expiraEmUtc,
      refreshExpiraEmUtc: resposta.refreshExpiraEmUtc,
      usuario: resposta.usuario,
    }
    setSessao(nova)
    salvarSessao(nova)
    return nova
  }, [])

  const entrar = useCallback(async (email: string, senha: string) => {
    const resposta = await apiEntrar({ email, senha })
    aplicarResposta(resposta)
  }, [aplicarResposta])

  const renovar = useCallback(async (): Promise<string | null> => {
    const atual = carregarSessao()
    if (!atual?.refreshToken) return null
    try {
      const resposta = await renovarToken(atual.refreshToken)
      aplicarResposta(resposta)
      return resposta.token
    } catch {
      sair()
      return null
    }
  }, [aplicarResposta, sair])

  useEffect(() => {
    configurarInterceptorsAuth(
      () => carregarSessao()?.token ?? null,
      renovar,
      sair,
    )

    const validar = async () => {
      const local = carregarSessao()
      if (!local?.token) {
        setCarregando(false)
        return
      }
      try {
        const usuario = await obterUsuarioAtual()
        setSessao((s) => (s ? { ...s, usuario } : s))
      } catch {
        sair()
      } finally {
        setCarregando(false)
      }
    }

    void validar()
  }, [renovar, sair])

  const value = useMemo<AuthContextValue>(
    () => ({
      sessao,
      carregando,
      autenticado: !!sessao?.token,
      entrar,
      sair,
    }),
    [sessao, carregando, entrar, sair],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
