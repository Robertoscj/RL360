export interface CardsTopo {
  riscoProximos30Dias: number
  valorOportunidade: number
  gargalosCriticos: number
  saudeEmpresaPercentual: number
  statusSaude: string
}

export interface FatorRadar {
  fator: string
  rotulo: string
  impactoFinanceiro: number
  direcao: string
  descricao: string
}

export interface SnapshotRadar {
  idEmpresa: string
  nomeEmpresa: string
  geradoEmUtc: string
  lucroAtual: number
  lucroEmRisco: number
  riscoProximos30Dias: number
  valorOportunidade: number
  gargalosCriticos: number
  saudeEmpresaPercentual: number
  statusSaude: string
  faturamentoDia: number
  faturamentoMes: number
  metaMensal: number
  percentualMetaAtingida: number
  inadimplenciaAtual: number
  inadimplenciaProjetada: number
  taxaConversaoAtual: number
  quedaConversao: number
  fluxoCaixaFuturo: number
  fatores: FatorRadar[]
  planoAcao: ItemPlanoAcao[]
}

export interface PontoSerieTemporal {
  rotulo: string
  valor: number
  meta?: number
}

export interface PrevisaoResultado {
  dias: number
  cenarioMaisProvavel: number
  meta: number
  percentualAbaixoMeta: number
  serie: PontoSerieTemporal[]
}

export interface SegmentoDonut {
  rotulo: string
  valor: number
  cor: string
}

export interface FluxoCaixaFuturo {
  dias: number
  aReceber: number
  emRisco: number
  atrasado: number
  percentualSaudavel: number
  segmentos: SegmentoDonut[]
}

export interface InsightInteligente {
  id: string
  tipo: string
  titulo: string
  descricao: string
  textoBotao: string
  rotaAcao: string
  impactoFinanceiro?: number
}

export interface AlertaCritico {
  id: string
  titulo: string
  tipo: string
  impactoFinanceiro: number
  severidade: string
}

export interface ItemPlanoAcao {
  titulo: string
  justificativa: string
  impactoEsperado: number
  prioridade: string
}

export interface RodapeDashboard {
  mensagem: string
  quantidadeAcoesPlano: number
  planoAcao: ItemPlanoAcao[]
}

export interface ResumoDashboard {
  idEmpresa: string
  nomeEmpresa: string
  geradoEmUtc: string
  cardsTopo: CardsTopo
  radar: SnapshotRadar
  previsaoResultado: PrevisaoResultado
  fluxoCaixaFuturo: FluxoCaixaFuturo
  insights: InsightInteligente[]
  alertasCriticos: AlertaCritico[]
  rodape: RodapeDashboard
}
