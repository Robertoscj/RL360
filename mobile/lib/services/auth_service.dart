import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:lucro360_mobile/core/api/api_client.dart';
import 'package:lucro360_mobile/models/auth.dart';

class AuthService {
  AuthService(this._api);

  final ApiClient _api;
  static const _storage = FlutterSecureStorage();
  static const _kToken = 'auth_token';
  static const _kRefresh = 'auth_refresh';

  Future<RespostaAutenticacao> entrar(RequisicaoLogin req) async {
    return _api.postDados(
      '/api/auth/login',
      data: req.toJson(),
      fromJson: (json) =>
          RespostaAutenticacao.fromJson(json! as Map<String, dynamic>),
    );
  }

  Future<RespostaAutenticacao> renovar(String refreshToken) async {
    return _api.postDados(
      '/api/auth/refresh',
      data: {'refreshToken': refreshToken},
      fromJson: (json) =>
          RespostaAutenticacao.fromJson(json! as Map<String, dynamic>),
    );
  }

  Future<Usuario> obterUsuarioAtual() async {
    return _api.getDados(
      '/api/auth/me',
      fromJson: (json) => Usuario.fromJson(json! as Map<String, dynamic>),
    );
  }

  Future<void> salvarTokens(RespostaAutenticacao auth) async {
    await _storage.write(key: _kToken, value: auth.token);
    await _storage.write(key: _kRefresh, value: auth.refreshToken);
  }

  Future<String?> lerToken() => _storage.read(key: _kToken);

  Future<String?> lerRefresh() => _storage.read(key: _kRefresh);

  Future<void> limparTokens() async {
    await _storage.delete(key: _kToken);
    await _storage.delete(key: _kRefresh);
  }
}
