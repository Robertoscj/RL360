import type { RespostaApi } from '@/types/api'
import type { RegistroInadimplencia } from '@/types/inadimplencia'
import { api } from './api'

export async function obterInadimplencia(): Promise<RegistroInadimplencia[]> {
  const { data } = await api.get<RespostaApi<RegistroInadimplencia[]>>('/api/delinquency')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar inadimplência')
  return data.dados
}
