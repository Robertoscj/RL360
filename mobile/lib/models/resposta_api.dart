class RespostaApi<T> {
  const RespostaApi({
    required this.sucesso,
    required this.mensagem,
    this.dados,
    this.erros = const [],
  });

  final bool sucesso;
  final String mensagem;
  final T? dados;
  final List<String> erros;

  factory RespostaApi.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) {
    return RespostaApi(
      sucesso: json['sucesso'] as bool? ?? false,
      mensagem: json['mensagem'] as String? ?? '',
      dados: json['dados'] != null ? fromJsonT(json['dados']) : null,
      erros: (json['erros'] as List<dynamic>?)
              ?.map((e) => e.toString())
              .toList() ??
          const [],
    );
  }

  String mensagemErro() {
    if (erros.isNotEmpty) return erros.join('; ');
    return mensagem.isNotEmpty ? mensagem : 'Erro desconhecido';
  }
}

class ApiException implements Exception {
  ApiException(this.mensagem);
  final String mensagem;

  @override
  String toString() => mensagem;
}
