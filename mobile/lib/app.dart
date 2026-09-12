import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucro360_mobile/core/constants/menu_itens.dart';
import 'package:lucro360_mobile/core/navigation/pagina_sem_transicao.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/features/alertas/pagina_alertas.dart';
import 'package:lucro360_mobile/features/auth/pagina_login.dart';
import 'package:lucro360_mobile/features/dashboard/pagina_dashboard.dart';
import 'package:lucro360_mobile/features/faturamento/pagina_faturamento.dart';
import 'package:lucro360_mobile/features/inadimplencia/pagina_inadimplencia.dart';
import 'package:lucro360_mobile/features/placeholder/pagina_placeholder.dart';
import 'package:lucro360_mobile/features/vendas/pagina_vendas.dart';
import 'package:lucro360_mobile/providers/app_providers.dart';
import 'package:lucro360_mobile/widgets/shell_app.dart';
import 'package:provider/provider.dart';

GoRoute _rotaInstantanea({
  required String path,
  required Widget child,
}) {
  return GoRoute(
    path: path,
    pageBuilder: (context, state) => paginaSemTransicao(
      key: state.pageKey,
      child: child,
    ),
  );
}

GoRouter criarRouter(AuthProvider auth) {
  return GoRouter(
    initialLocation: '/',
    refreshListenable: auth,
    redirect: (context, state) {
      final logado = auth.autenticado;
      final login = state.matchedLocation == '/login';
      if (!logado && !login) return '/login';
      if (logado && login) return '/';
      return null;
    },
    routes: [
      GoRoute(
        path: '/login',
        pageBuilder: (context, state) => paginaSemTransicao(
          key: state.pageKey,
          child: const PaginaLogin(),
        ),
      ),
      ShellRoute(
        builder: (context, state, child) {
          final rota = state.uri.path;
          final item = itemMenuPorRota(rota);

          return _ShellNavegacao(
            titulo: item?.rotulo ?? 'LUCRO360',
            subtitulo: item?.subtitulo ?? '',
            rotaAtual: rota,
            child: child,
          );
        },
        routes: [
          _rotaInstantanea(path: '/', child: const PaginaDashboard()),
          _rotaInstantanea(
            path: '/faturamento',
            child: const PaginaFaturamento(),
          ),
          _rotaInstantanea(path: '/vendas', child: const PaginaVendas()),
          _rotaInstantanea(
            path: '/inadimplencia',
            child: const PaginaInadimplencia(),
          ),
          _rotaInstantanea(path: '/alertas', child: const PaginaAlertas()),
          _rotaInstantanea(
            path: '/funil',
            child: const PaginaPlaceholder(
              titulo: 'Funil de Vendas',
              subtitulo: 'Etapas, conversão e valor potencial.',
              rota: '/funil',
            ),
          ),
          _rotaInstantanea(
            path: '/gargalos',
            child: const PaginaPlaceholder(
              titulo: 'Gargalos',
              subtitulo: 'Gargalos operacionais e impacto financeiro.',
              rota: '/gargalos',
            ),
          ),
          _rotaInstantanea(
            path: '/clientes',
            child: const PaginaPlaceholder(
              titulo: 'Clientes',
              subtitulo: 'Base de clientes, LTV e potencial de expansão.',
              rota: '/clientes',
            ),
          ),
          _rotaInstantanea(
            path: '/equipe',
            child: const PaginaPlaceholder(
              titulo: 'Equipe',
              subtitulo: 'Produtividade e metas por colaborador.',
              rota: '/equipe',
            ),
          ),
          _rotaInstantanea(
            path: '/metas',
            child: const PaginaPlaceholder(
              titulo: 'Metas',
              subtitulo: 'Objetivos e acompanhamento mensal.',
              rota: '/metas',
            ),
          ),
          _rotaInstantanea(
            path: '/relatorios',
            child: const PaginaPlaceholder(
              titulo: 'Relatórios',
              subtitulo: 'Exportações e análises detalhadas.',
              rota: '/relatorios',
            ),
          ),
          _rotaInstantanea(
            path: '/configuracoes',
            child: const PaginaPlaceholder(
              titulo: 'Configurações',
              subtitulo: 'Preferências da empresa e da conta.',
              rota: '/configuracoes',
            ),
          ),
        ],
      ),
    ],
  );
}

/// Shell isolado — escuta DashboardProvider só na rota do dashboard.
class _ShellNavegacao extends StatelessWidget {
  const _ShellNavegacao({
    required this.titulo,
    required this.subtitulo,
    required this.rotaAtual,
    required this.child,
  });

  final String titulo;
  final String subtitulo;
  final String rotaAtual;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    final ehDashboard = rotaAtual == '/';

    if (!ehDashboard) {
      return ShellApp(
        titulo: titulo,
        subtitulo: subtitulo,
        rotaAtual: rotaAtual,
        child: child,
      );
    }

    return Selector<DashboardProvider, bool>(
      selector: (_, p) => p.atualizando,
      builder: (context, atualizando, _) {
        return ShellApp(
          titulo: titulo,
          subtitulo: subtitulo,
          rotaAtual: rotaAtual,
          onAtualizar: () => context.read<DashboardProvider>().atualizar(),
          atualizando: atualizando,
          child: child,
        );
      },
    );
  }
}

class AppLucro360 extends StatefulWidget {
  const AppLucro360({super.key, required this.auth});

  final AuthProvider auth;

  @override
  State<AppLucro360> createState() => _AppLucro360State();
}

class _AppLucro360State extends State<AppLucro360> {
  static final _temaClaro = construirTema(claro: true);
  static final _temaEscuro = construirTema(claro: false);

  late final GoRouter _router;

  @override
  void initState() {
    super.initState();
    _router = criarRouter(widget.auth);
  }

  @override
  Widget build(BuildContext context) {
    final modo = context.select<TemaProvider, ThemeMode>((p) => p.modo);

    return MaterialApp.router(
      title: 'LUCRO360',
      debugShowCheckedModeBanner: false,
      themeMode: modo,
      theme: _temaClaro,
      darkTheme: _temaEscuro,
      routerConfig: _router,
    );
  }
}
