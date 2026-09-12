export interface RegistroInadimplencia {
  id: string
  nomeCliente: string
  valor: number
  diasEmAtraso: number
  probabilidadeRecuperacao: number
  perdaProjetada: number
}

export interface ResumoInadimplencia {
  totalEmAtraso: number
  perdaProjetada: number
  valorRecuperavel: number
  quantidadeContas: number
  contasCriticas: number
  diasMedios: number
  taxaRecuperacaoMedia: number
  taxaInadimplenciaSobreFaturamento: number | null
}

export interface FaixaAtraso {
  rotulo: string
  valor: number
  quantidade: number
  cor: string
}

export type SeveridadeInadimplencia = 'critico' | 'alto' | 'medio' | 'baixo'
