import 'package:lucro360_mobile/core/api/api_client.dart';

class RespostaIa {
  const RespostaIa({required this.resposta, required this.modo});

  final String resposta;
  final String modo;

  factory RespostaIa.fromJson(Map<String, dynamic> json) {
    return RespostaIa(
      resposta: json['resposta'] as String? ?? '',
      modo: json['modo'] as String? ?? '',
    );
  }
}

class IaService {
  IaService(this._api);

  final ApiClient _api;

  Future<RespostaIa> perguntar(String pergunta) async {
    return _api.postDados(
      '/api/ia/perguntar',
      data: {'pergunta': pergunta},
      fromJson: (json) => RespostaIa.fromJson(json! as Map<String, dynamic>),
    );
  }
}
