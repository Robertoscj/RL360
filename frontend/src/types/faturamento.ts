export interface PontoSerieTemporal {
  rotulo: string
  valor: number
  meta?: number
}

export interface Faturamento {
  faturamentoDia: number
  faturamentoMes: number
  metaMensal: number
  percentualMetaAtingida: number
  custoMes: number
  lucro: number
  quantidadeOperacoes: number
  ticketMedio: number
  serieDiaria: PontoSerieTemporal[]
}

export interface Venda {
  id: string
  canal: string
  valor: number
  margem: number
  fechadaEmUtc: string
  clienteNovo: boolean
  nomeCliente?: string
}
