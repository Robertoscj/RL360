import type { RespostaApi } from '@/types/api'
import type { Faturamento, Venda } from '@/types/faturamento'
import { api } from './api'

export async function obterFaturamento(): Promise<Faturamento> {
  const { data } = await api.get<RespostaApi<Faturamento>>('/api/revenue')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar faturamento')
  return data.dados
}

export async function obterVendas(): Promise<Venda[]> {
  const { data } = await api.get<RespostaApi<Venda[]>>('/api/sales')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar vendas')
  return data.dados
}
