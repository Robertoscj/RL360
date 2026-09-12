import 'dart:async';

import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/api/api_client.dart';
import 'package:lucro360_mobile/models/auth.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/services/auth_service.dart';
import 'package:lucro360_mobile/services/dashboard_service.dart';

class AuthProvider extends ChangeNotifier {
  AuthProvider(this._authService, this._apiClient) {
    _apiClient.configurarAuth(
      obterToken: () => _token,
      renovarToken: _renovarTokenInterno,
      sair: sair,
    );
  }

  final AuthService _authService;
  final ApiClient _apiClient;

  bool _autenticado = false;
  bool _inicializando = true;
  String? _nome;
  String? _token;
  String? _refreshToken;
  Usuario? _usuario;

  String? _ultimoErro;

  String? get ultimoErro => _ultimoErro;
  bool get autenticado => _autenticado;
  bool get inicializando => _inicializando;
  String? get nome => _nome;
  String? get token => _token;
  Usuario? get usuario => _usuario;

  DashboardService get dashboardService => DashboardService(_apiClient);

  Future<void> restaurarSessao() async {
    _inicializando = true;
    notifyListeners();

    try {
      _token = await _authService.lerToken();
      _refreshToken = await _authService.lerRefresh();
      if (_token == null) return;

      _usuario = await _authService.obterUsuarioAtual();
      _nome = _usuario!.nome;
      _autenticado = true;
    } catch (_) {
      await _authService.limparTokens();
      _token = null;
      _refreshToken = null;
      _autenticado = false;
    } finally {
      _inicializando = false;
      notifyListeners();
    }
  }

  Future<bool> entrar(String usuario, String senha) async {
    _ultimoErro = null;
    try {
      final auth = await _authService.entrar(
        RequisicaoLogin(email: usuario.trim(), senha: senha),
      );
      await _aplicarAuth(auth);
      return true;
    } catch (e) {
      _ultimoErro = e.toString().replaceFirst('ApiException: ', '');
      return false;
    }
  }

  Future<String?> _renovarTokenInterno() async {
    if (_refreshToken == null) return null;
    try {
      final auth = await _authService.renovar(_refreshToken!);
      await _aplicarAuth(auth);
      return _token;
    } catch (_) {
      await sair();
      return null;
    }
  }

  Future<void> _aplicarAuth(RespostaAutenticacao auth) async {
    _token = auth.token;
    _refreshToken = auth.refreshToken;
    _usuario = auth.usuario;
    _nome = auth.usuario.nome;
    _autenticado = true;
    await _authService.salvarTokens(auth);
    notifyListeners();
  }

  Future<void> sair() async {
    await _authService.limparTokens();
    _autenticado = false;
    _nome = null;
    _token = null;
    _refreshToken = null;
    _usuario = null;
    notifyListeners();
  }
}

class DashboardProvider extends ChangeNotifier {
  DashboardProvider(this._dashboardService);

  final DashboardService _dashboardService;

  ResumoDashboard? _resumo;
  bool _carregando = false;
  bool _atualizando = false;
  String? _erro;
  Timer? _timerPeriodico;
  bool _timerIniciado = false;
  bool _visivel = false;

  ResumoDashboard? get resumo => _resumo;
  bool get carregando => _carregando;
  bool get atualizando => _atualizando;
  String? get erro => _erro;

  List<ItemPlanoAcao> get planoAcao {
    if (_resumo == null) return const [];
    if (_resumo!.rodape.planoAcao.isNotEmpty) return _resumo!.rodape.planoAcao;
    return _resumo!.radar.planoAcao;
  }

  /// Carrega só na primeira vez; evita spinner ao voltar para o dashboard.
  Future<void> carregarSeNecessario() async {
    if (_resumo != null) return;
    await carregar();
  }

  void marcarVisivel(bool visivel) {
    _visivel = visivel;
    if (!visivel) {
      _timerPeriodico?.cancel();
      _timerIniciado = false;
    }
  }

  void iniciarAtualizacaoPeriodica() {
    if (_timerIniciado || !_visivel) return;
    _timerIniciado = true;
    _timerPeriodico?.cancel();
    _timerPeriodico = Timer.periodic(
      const Duration(seconds: 60),
      (_) => atualizarSilencioso(),
    );
  }

  @override
  void dispose() {
    _timerPeriodico?.cancel();
    super.dispose();
  }

  Future<void> carregar() async {
    if (_carregando) return;
    _carregando = _resumo == null;
    _erro = null;
    if (_carregando) notifyListeners();

    try {
      _resumo = await _dashboardService.obterResumo();
      _erro = null;
    } catch (e) {
      _erro = e.toString();
    } finally {
      _carregando = false;
      notifyListeners();
    }
  }

  /// Atualização leve — só busca summary, sem recalcular radar.
  Future<void> atualizarSilencioso() async {
    if (!_visivel || _atualizando || _carregando) return;
    try {
      final dados = await _dashboardService.obterResumo();
      if (_resumo?.geradoEmUtc == dados.geradoEmUtc) return;
      _resumo = dados;
      _erro = null;
      notifyListeners();
    } catch (_) {}
  }

  /// Atualização completa — botão ↻ e pull-to-refresh.
  Future<void> atualizar() async {
    if (_atualizando) return;
    _atualizando = true;
    _erro = null;
    notifyListeners();

    try {
      try {
        await _dashboardService.recalcularRadar();
      } catch (_) {
        /* fallback via summary */
      }
      _resumo = await _dashboardService.obterResumo(atualizar: true);
    } catch (e) {
      _erro = e.toString();
    } finally {
      _atualizando = false;
      notifyListeners();
    }
  }
}

class TemaProvider extends ChangeNotifier {
  ThemeMode _modo = ThemeMode.dark;

  ThemeMode get modo => _modo;
  bool get ehClaro => _modo == ThemeMode.light;

  void alternar() {
    _modo = _modo == ThemeMode.dark ? ThemeMode.light : ThemeMode.dark;
    notifyListeners();
  }
}
