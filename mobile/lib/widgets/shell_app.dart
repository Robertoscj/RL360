import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucro360_mobile/core/constants/menu_itens.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:provider/provider.dart';
import 'package:lucro360_mobile/providers/app_providers.dart';

class MenuLateral extends StatelessWidget {
  const MenuLateral({super.key, required this.rotaAtual});

  final String rotaAtual;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return Drawer(
      child: Column(
        children: [
          DrawerHeader(
            padding: const EdgeInsets.fromLTRB(20, 24, 20, 12),
            decoration: BoxDecoration(
              border: Border(bottom: BorderSide(color: c.border)),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      width: 36,
                      height: 36,
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        gradient: RadialGradient(
                          colors: [
                            c.emerald.withValues(alpha: 0.25),
                            c.emerald.withValues(alpha: 0.08),
                          ],
                        ),
                      ),
                      child: Icon(Icons.gps_fixed, color: c.emerald, size: 18),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'LUCRO360',
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.w900,
                              color: c.heading,
                              letterSpacing: 0.5,
                            ),
                          ),
                          Text(
                            'Seu radar de lucro em tempo real',
                            style: TextStyle(fontSize: 10, color: c.muted),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          Expanded(
            child: ListView.builder(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              itemCount: itensMenuRl.length,
              itemBuilder: (context, i) {
                final item = itensMenuRl[i];
                final ativo = rotaAtual == item.rota;
                return Padding(
                  padding: const EdgeInsets.only(bottom: 4),
                  child: ListTile(
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(14),
                      side: ativo
                          ? BorderSide(color: c.emerald.withValues(alpha: 0.2))
                          : BorderSide.none,
                    ),
                    tileColor: ativo
                        ? c.emerald.withValues(alpha: 0.1)
                        : null,
                    leading: Container(
                      width: 32,
                      height: 32,
                      decoration: BoxDecoration(
                        color: ativo
                            ? c.emerald.withValues(alpha: 0.15)
                            : Colors.transparent,
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: Icon(
                        item.icone,
                        size: 18,
                        color: ativo ? c.emerald : c.muted,
                      ),
                    ),
                    title: Text(
                      item.rotulo,
                      style: TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w600,
                        color: ativo ? c.heading : c.body,
                      ),
                    ),
                    subtitle: Text(
                      item.subtitulo,
                      style: TextStyle(fontSize: 10, color: c.muted),
                    ),
                    trailing: ativo
                        ? Icon(Icons.chevron_right, size: 16, color: c.emerald)
                        : null,
                    onTap: () {
                      Navigator.pop(context);
                      context.go(item.rota);
                    },
                  ),
                );
              },
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(16),
            child: Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: c.surface,
                borderRadius: BorderRadius.circular(14),
                border: Border.all(color: c.border),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    'Precisa de ajuda?',
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: c.heading,
                    ),
                  ),
                  Text(
                    'Fale com nosso suporte',
                    style: TextStyle(fontSize: 10, color: c.muted),
                  ),
                  const SizedBox(height: 12),
                  OutlinedButton(
                    onPressed: () {},
                    style: OutlinedButton.styleFrom(
                      foregroundColor: c.emerald,
                      side: BorderSide(color: c.emerald.withValues(alpha: 0.35)),
                      backgroundColor: c.emerald.withValues(alpha: 0.06),
                    ),
                    child: const Text(
                      'Abrir chamado',
                      style: TextStyle(fontSize: 11, fontWeight: FontWeight.w700),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class ShellApp extends StatelessWidget {
  const ShellApp({
    super.key,
    required this.titulo,
    required this.subtitulo,
    required this.rotaAtual,
    required this.child,
    this.onAtualizar,
    this.atualizando = false,
  });

  final String titulo;
  final String subtitulo;
  final String rotaAtual;
  final Widget child;
  final VoidCallback? onAtualizar;
  final bool atualizando;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    final ehClaro = context.select<TemaProvider, bool>((p) => p.ehClaro);

    return Scaffold(
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(titulo.toUpperCase()),
            Text(
              subtitulo,
              style: TextStyle(
                fontSize: 11,
                fontWeight: FontWeight.normal,
                color: c.muted,
              ),
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: Icon(ehClaro ? Icons.dark_mode : Icons.light_mode),
            onPressed: () => context.read<TemaProvider>().alternar(),
            tooltip: 'Alternar tema',
          ),
          if (onAtualizar != null)
            IconButton(
              icon: atualizando
                  ? SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: c.emerald,
                      ),
                    )
                  : const Icon(Icons.refresh),
              onPressed: atualizando ? null : onAtualizar,
            ),
          IconButton(
            icon: Badge(
              smallSize: 8,
              backgroundColor: c.red,
              child: const Icon(Icons.notifications_outlined),
            ),
            onPressed: () => context.go('/alertas'),
          ),
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () {
              context.read<AuthProvider>().sair();
              context.go('/login');
            },
          ),
        ],
      ),
      drawer: MenuLateral(rotaAtual: rotaAtual),
      body: child,
    );
  }
}
