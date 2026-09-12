export interface RequisicaoIaPergunta {
  pergunta: string
  inicio?: string
  fim?: string
  idConversa?: string
}

export interface RespostaIaPergunta {
  resposta: string
  modo: string
  idConversa: string
  geradoEmUtc: string
}

export interface MensagemIa {
  id: string
  idConversa: string
  papel: 'usuario' | 'assistente'
  texto: string
  modo?: string
  criadoEmUtc?: string
}

export interface HistoricoIa {
  idConversa: string
  mensagens: Array<{
    id: string
    idConversa: string
    papel: string
    conteudo: string
    modo: string
    criadoEmUtc: string
  }>
}

export interface EventoStreamIa {
  tipo: 'delta' | 'done' | 'erro'
  delta?: string
  modo?: string
  idConversa?: string
  erro?: string
}
