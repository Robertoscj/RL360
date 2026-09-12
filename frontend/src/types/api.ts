export interface RespostaApi<T> {
  sucesso: boolean
  mensagem: string
  dados: T | null
  erros: string[]
}
