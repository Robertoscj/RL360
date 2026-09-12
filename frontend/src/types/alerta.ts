export interface Alerta {
  id: string
  titulo: string
  mensagem: string
  severidade: string
  impactoFinanceiro: number
  fatorRelacionado?: string | null
  resolvido: boolean
  criadoEmUtc: string
}
