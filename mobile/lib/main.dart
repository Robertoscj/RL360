import 'package:flutter/material.dart';
import 'package:lucro360_mobile/app.dart';
import 'package:lucro360_mobile/core/api/api_client.dart';
import 'package:lucro360_mobile/providers/app_providers.dart';
import 'package:lucro360_mobile/services/auth_service.dart';
import 'package:provider/provider.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();

  FlutterError.onError = (details) {
    FlutterError.presentError(details);
    debugPrint('FlutterError: ${details.exceptionAsString()}');
  };

  final apiClient = ApiClient();
  final authService = AuthService(apiClient);
  final authProvider = AuthProvider(authService, apiClient);
  final dashboardProvider = DashboardProvider(authProvider.dashboardService);

  runApp(
    MultiProvider(
      providers: [
        Provider<ApiClient>.value(value: apiClient),
        ChangeNotifierProvider<TemaProvider>(create: (_) => TemaProvider()),
        ChangeNotifierProvider<AuthProvider>.value(value: authProvider),
        ChangeNotifierProvider<DashboardProvider>.value(value: dashboardProvider),
      ],
      child: _AppBootstrap(auth: authProvider),
    ),
  );
}

class _AppBootstrap extends StatefulWidget {
  const _AppBootstrap({required this.auth});

  final AuthProvider auth;

  @override
  State<_AppBootstrap> createState() => _AppBootstrapState();
}

class _AppBootstrapState extends State<_AppBootstrap> {
  @override
  void initState() {
    super.initState();
    widget.auth.restaurarSessao();
  }

  @override
  Widget build(BuildContext context) {
    final inicializando = context.select<AuthProvider, bool>(
      (p) => p.inicializando,
    );

    if (inicializando) {
      return MaterialApp(
        debugShowCheckedModeBanner: false,
        theme: ThemeData(brightness: Brightness.dark),
        home: Scaffold(
          backgroundColor: const Color(0xFF0A0E17),
          body: Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const CircularProgressIndicator(),
                const SizedBox(height: 16),
                Text(
                  'LUCRO360',
                  style: TextStyle(
                    fontWeight: FontWeight.w900,
                    fontSize: 18,
                    color: Colors.grey.shade600,
                  ),
                ),
              ],
            ),
          ),
        ),
      );
    }

    return AppLucro360(auth: widget.auth);
  }
}
