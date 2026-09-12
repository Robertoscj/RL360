import type { RespostaApi } from '@/types/api'
import type { EtapaFunil, Venda } from '@/types/vendas'
import { paraIsoData } from '@/utils/datas'
import { api } from './api'

export interface FiltroPeriodoVendas {
  inicio: Date
  fim: Date
}

export async function obterFunil(): Promise<EtapaFunil[]> {
  const { data } = await api.get<RespostaApi<EtapaFunil[]>>('/api/funnel')
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar funil')
  return data.dados
}

export async function obterVendasModulo(periodo?: FiltroPeriodoVendas): Promise<Venda[]> {
  const params = periodo
    ? { inicio: paraIsoData(periodo.inicio), fim: paraIsoData(periodo.fim) }
    : undefined
  const { data } = await api.get<RespostaApi<Venda[]>>('/api/sales', { params })
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar vendas')
  return data.dados
}
