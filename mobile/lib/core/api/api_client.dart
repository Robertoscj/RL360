import 'package:dio/dio.dart';
import 'package:lucro360_mobile/core/config/api_config.dart';
import 'package:lucro360_mobile/models/resposta_api.dart';

typedef TokenGetter = String? Function();
typedef TokenRefresher = Future<String?> Function();
typedef LogoutHandler = void Function();

class ApiClient {
  ApiClient() {
    _dio = Dio(
      BaseOptions(
        baseUrl: ApiConfig.baseUrl,
        connectTimeout: const Duration(seconds: 15),
        receiveTimeout: const Duration(seconds: 30),
        headers: {'Content-Type': 'application/json'},
      ),
    );

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          final token = _tokenGetter?.call();
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (error, handler) async {
          final response = error.response;
          if (response?.statusCode == 401 &&
              error.requestOptions.extra['_retry'] != true &&
              _tokenRefresher != null) {
            error.requestOptions.extra['_retry'] = true;
            final novo = await _tokenRefresher!.call();
            if (novo != null && novo.isNotEmpty) {
              error.requestOptions.headers['Authorization'] = 'Bearer $novo';
              try {
                final retry = await _dio.fetch<dynamic>(error.requestOptions);
                handler.resolve(retry);
                return;
              } catch (_) {}
            }
            _logoutHandler?.call();
          }
          handler.next(error);
        },
      ),
    );
  }

  late final Dio _dio;
  TokenGetter? _tokenGetter;
  TokenRefresher? _tokenRefresher;
  LogoutHandler? _logoutHandler;

  Dio get dio => _dio;

  void configurarAuth({
    required TokenGetter obterToken,
    required TokenRefresher renovarToken,
    required LogoutHandler sair,
  }) {
    _tokenGetter = obterToken;
    _tokenRefresher = renovarToken;
    _logoutHandler = sair;
  }

  Future<T> getDados<T>(
    String path, {
    Map<String, dynamic>? queryParameters,
    required T Function(Object? json) fromJson,
  }) async {
    try {
      final response = await _dio.get<Map<String, dynamic>>(
        path,
        queryParameters: queryParameters,
      );
      final body = response.data;
      if (body == null) throw ApiException('Resposta vazia');
      final resposta = RespostaApi.fromJson(body, fromJson);
      if (!resposta.sucesso || resposta.dados == null) {
        throw ApiException(resposta.mensagemErro());
      }
      return resposta.dados as T;
    } on DioException catch (e) {
      throw ApiException(_mensagemDio(e));
    }
  }

  Future<T> postDados<T>(
    String path, {
    Object? data,
    Map<String, dynamic>? queryParameters,
    required T Function(Object? json) fromJson,
  }) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        path,
        data: data,
        queryParameters: queryParameters,
      );
      final body = response.data;
      if (body == null) throw ApiException('Resposta vazia');
      final resposta = RespostaApi.fromJson(body, fromJson);
      if (!resposta.sucesso || resposta.dados == null) {
        throw ApiException(resposta.mensagemErro());
      }
      return resposta.dados as T;
    } on DioException catch (e) {
      throw ApiException(_mensagemDio(e));
    }
  }

  Future<void> postVoid(String path, {Object? data}) async {
    try {
      await _dio.post(path, data: data);
    } on DioException catch (e) {
      throw ApiException(_mensagemDio(e));
    }
  }

  String _mensagemDio(DioException e) {
    final data = e.response?.data;
    if (data is Map<String, dynamic>) {
      final resposta = RespostaApi.fromJson(data, (j) => j);
      return resposta.mensagemErro();
    }
    if (e.type == DioExceptionType.connectionError) {
      return 'Sem conexão com a API (${ApiConfig.baseUrl})';
    }
    return e.message ?? 'Erro de rede';
  }
}
