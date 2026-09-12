export interface RequisicaoLogin {
  email: string
  senha: string
}

export interface Usuario {
  id: string
  idEmpresa: string
  nome: string
  email: string
  perfil: string
  nomeEmpresa: string
}

export interface RespostaAutenticacao {
  token: string
  expiraEmUtc: string
  refreshToken: string
  refreshExpiraEmUtc: string
  usuario: Usuario
}

export interface SessaoAuth {
  token: string
  refreshToken: string
  expiraEmUtc: string
  refreshExpiraEmUtc: string
  usuario: Usuario
}
