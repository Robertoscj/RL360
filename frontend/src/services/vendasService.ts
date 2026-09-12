import type { RespostaApi } from '@/types/api'
import type { EtapaFunil, Venda } from '@/types/vendas'
import { api } from './api'

export async function obterFunil(): Promise<EtapaFunil[]> {
  const { data } = await api.get<RespostaApi<EtapaFunil[]>>('/api/funnel')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar funil')
  return data.dados
}

export async function obterVendasModulo(): Promise<Venda[]> {
  const { data } = await api.get<RespostaApi<Venda[]>>('/api/sales')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar vendas')
  return data.dados
}
