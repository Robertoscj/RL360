class RequisicaoLogin {
  const RequisicaoLogin({required this.email, required this.senha});

  final String email;
  final String senha;

  Map<String, dynamic> toJson() => {'email': email, 'senha': senha};
}

class Usuario {
  const Usuario({
    required this.id,
    required this.idEmpresa,
    required this.nome,
    required this.email,
    required this.perfil,
    required this.nomeEmpresa,
  });

  final String id;
  final String idEmpresa;
  final String nome;
  final String email;
  final String perfil;
  final String nomeEmpresa;

  factory Usuario.fromJson(Map<String, dynamic> json) {
    return Usuario(
      id: json['id'] as String,
      idEmpresa: json['idEmpresa'] as String,
      nome: json['nome'] as String? ?? '',
      email: json['email'] as String? ?? '',
      perfil: json['perfil'] as String? ?? '',
      nomeEmpresa: json['nomeEmpresa'] as String? ?? '',
    );
  }
}

class RespostaAutenticacao {
  const RespostaAutenticacao({
    required this.token,
    required this.expiraEmUtc,
    required this.refreshToken,
    required this.refreshExpiraEmUtc,
    required this.usuario,
  });

  final String token;
  final DateTime expiraEmUtc;
  final String refreshToken;
  final DateTime refreshExpiraEmUtc;
  final Usuario usuario;

  factory RespostaAutenticacao.fromJson(Map<String, dynamic> json) {
    return RespostaAutenticacao(
      token: json['token'] as String,
      expiraEmUtc: DateTime.parse(json['expiraEmUtc'] as String),
      refreshToken: json['refreshToken'] as String,
      refreshExpiraEmUtc: DateTime.parse(json['refreshExpiraEmUtc'] as String),
      usuario: Usuario.fromJson(json['usuario'] as Map<String, dynamic>),
    );
  }
}
