import 'package:flutter/foundation.dart';

/// URL base da API RL360 (dev).
/// Android emulador: 10.0.2.2 · Web/iOS/desktop: localhost
class ApiConfig {
  static String get baseUrl {
    const override = String.fromEnvironment('API_URL');
    if (override.isNotEmpty) return override;

    if (!kIsWeb && defaultTargetPlatform == TargetPlatform.android) {
      return 'http://10.0.2.2:5080';
    }
    return 'http://localhost:5080';
  }
}
