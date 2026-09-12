export interface EtapaFunil {
  nome: string
  ordem: number
  quantidade: number
  valorPotencial: number
  taxaConversao: number
  taxaConversaoBase: number
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

export interface ResumoVendas {
  totalVendas: number
  totalMargem: number
  quantidadeVendas: number
  novosClientes: number
  ticketMedio: number
}
