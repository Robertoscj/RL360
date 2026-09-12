import type { RespostaApi } from '@/types/api'
import type { RequisicaoLogin, RespostaAutenticacao, Usuario } from '@/types/auth'
import { api } from './api'

export async function entrar(requisicao: RequisicaoLogin): Promise<RespostaAutenticacao> {
  const { data } = await api.post<RespostaApi<RespostaAutenticacao>>('/api/auth/login', requisicao)
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || data.erros.join('; ') || 'Falha no login')
  return data.dados
}

export async function renovarToken(refreshToken: string): Promise<RespostaAutenticacao> {
  const { data } = await api.post<RespostaApi<RespostaAutenticacao>>('/api/auth/refresh', { refreshToken })
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Refresh token inválido')
  return data.dados
}

export async function obterUsuarioAtual(): Promise<Usuario> {
  const { data } = await api.get<RespostaApi<Usuario>>('/api/auth/me')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Não autenticado')
  return data.dados
}
