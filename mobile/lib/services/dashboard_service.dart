import 'package:lucro360_mobile/core/api/api_client.dart';
import 'package:lucro360_mobile/models/dashboard.dart';

class DashboardService {
  DashboardService(this._api);

  final ApiClient _api;

  Future<ResumoDashboard> obterResumo({bool atualizar = false}) async {
    return _api.getDados(
      '/api/dashboard/summary',
      queryParameters: {'atualizar': atualizar},
      fromJson: (json) =>
          ResumoDashboard.fromJson(json! as Map<String, dynamic>),
    );
  }

  Future<void> recalcularRadar() async {
    await _api.postVoid('/api/radar/recalcular');
  }
}
