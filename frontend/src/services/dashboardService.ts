import type { RespostaApi } from '@/types/api'
import type { ResumoDashboard } from '@/types/dashboard'
import { paraIsoData } from '@/utils/datas'
import { api } from './api'

export interface FiltroPeriodo {
  inicio: Date
  fim: Date
}

export async function recalcularRadar(): Promise<void> {
  await api.post('/api/radar/recalcular')
}

export async function obterResumoDashboard(
  atualizar = false,
  periodo?: FiltroPeriodo,
): Promise<ResumoDashboard> {
  const params: Record<string, string | boolean> = { atualizar }
  if (periodo) {
    params.inicio = paraIsoData(periodo.inicio)
    params.fim = paraIsoData(periodo.fim)
  }

  const { data } = await api.get<RespostaApi<ResumoDashboard>>('/api/dashboard/summary', {
    params,
  })
  if (!data.sucesso || !data.dados) throw new Error(data.mensagem || 'Erro ao carregar dashboard')
  return data.dados
}
