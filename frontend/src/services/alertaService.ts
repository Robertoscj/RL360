import type { RespostaApi } from '@/types/api'
import type { Alerta } from '@/types/alerta'
import { api } from './api'

export async function obterAlertas(): Promise<Alerta[]> {
  const { data } = await api.get<RespostaApi<Alerta[]>>('/api/alerts')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar alertas')
  return data.dados.filter((a) => !a.resolvido)
}

export async function resolverAlerta(id: string): Promise<void> {
  const { data } = await api.put<RespostaApi<boolean>>(`/api/alerts/${id}/resolve`)
  if (!data.sucesso) throw new Error(data.mensagem || 'Erro ao resolver alerta')
}
