import type { RespostaApi } from '@/types/api'
import type { Faturamento, Venda } from '@/types/faturamento'
import { paraIsoData } from '@/utils/datas'
import { api } from './api'

export interface FiltroPeriodo {
  inicio: Date
  fim: Date
}

function paramsPeriodo(periodo?: FiltroPeriodo) {
  if (!periodo) return undefined
  return { inicio: paraIsoData(periodo.inicio), fim: paraIsoData(periodo.fim) }
}

export async function obterFaturamento(periodo?: FiltroPeriodo): Promise<Faturamento> {
  const { data } = await api.get<RespostaApi<Faturamento>>('/api/revenue', {
    params: paramsPeriodo(periodo),
  })
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar faturamento')
  return data.dados
}

export async function obterVendas(periodo?: FiltroPeriodo): Promise<Venda[]> {
  const { data } = await api.get<RespostaApi<Venda[]>>('/api/sales', {
    params: paramsPeriodo(periodo),
  })
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar vendas')
  return data.dados
}
