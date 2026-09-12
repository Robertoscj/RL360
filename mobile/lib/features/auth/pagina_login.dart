import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucro360_mobile/core/demo/dados_demo.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/providers/app_providers.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';
import 'package:provider/provider.dart';

class PaginaLogin extends StatefulWidget {
  const PaginaLogin({super.key});

  @override
  State<PaginaLogin> createState() => _PaginaLoginState();
}

class _PaginaLoginState extends State<PaginaLogin> {
  final _usuarioCtrl = TextEditingController(text: DadosDemo.usuario);
  final _senhaCtrl = TextEditingController(text: DadosDemo.senha);
  bool _carregando = false;
  String? _erro;

  @override
  void dispose() {
    _usuarioCtrl.dispose();
    _senhaCtrl.dispose();
    super.dispose();
  }

  Future<void> _entrar() async {
    setState(() {
      _carregando = true;
      _erro = null;
    });
    final auth = context.read<AuthProvider>();
    final ok = await auth.entrar(
          _usuarioCtrl.text.trim(),
          _senhaCtrl.text,
        );
    if (!mounted) return;
    setState(() => _carregando = false);
    if (ok) {
      context.read<DashboardProvider>().carregarSeNecessario();
      context.go('/');
    } else {
      setState(() => _erro = auth.ultimoErro ?? 'Usuário ou senha inválidos');
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return Scaffold(
      body: SafeArea(
        child: Stack(
          children: [
            Align(
              alignment: Alignment.topRight,
              child: IconButton(
                icon: Icon(
                  Theme.of(context).brightness == Brightness.light
                      ? Icons.dark_mode
                      : Icons.light_mode,
                ),
                onPressed: () => context.read<TemaProvider>().alternar(),
              ),
            ),
            Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(24),
                child: ConstrainedBox(
                  constraints: const BoxConstraints(maxWidth: 400),
                  child: Column(
                    children: [
                      Container(
                        width: 64,
                        height: 64,
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(20),
                          gradient: const LinearGradient(
                            colors: [Color(0xFF22C55E), Color(0xFF0D9488)],
                          ),
                          boxShadow: [
                            BoxShadow(
                              color: c.emerald.withValues(alpha: 0.35),
                              blurRadius: 20,
                              offset: const Offset(0, 8),
                            ),
                          ],
                        ),
                        child: const Icon(Icons.radar, color: Colors.white, size: 32),
                      ),
                      const SizedBox(height: 20),
                      RichText(
                        text: TextSpan(
                          style: TextStyle(
                            fontSize: 28,
                            fontWeight: FontWeight.w900,
                            color: c.heading,
                          ),
                          children: [
                            const TextSpan(text: 'LUCRO'),
                            TextSpan(
                              text: '360',
                              style: TextStyle(color: c.emerald),
                            ),
                          ],
                        ),
                      ),
                      Text(
                        'Radar de Lucro em Tempo Real',
                        style: TextStyle(color: c.muted, fontSize: 14),
                      ),
                      const SizedBox(height: 32),
                      CartaoVidro(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          children: [
                            Text('Usuário', style: TextStyle(fontSize: 12, color: c.muted)),
                            const SizedBox(height: 6),
                            TextField(
                              controller: _usuarioCtrl,
                              keyboardType: TextInputType.emailAddress,
                              decoration: const InputDecoration(hintText: 'Usuário'),
                            ),
                            const SizedBox(height: 16),
                            Text('Senha', style: TextStyle(fontSize: 12, color: c.muted)),
                            const SizedBox(height: 6),
                            TextField(
                              controller: _senhaCtrl,
                              obscureText: true,
                              decoration: const InputDecoration(hintText: '••••••••'),
                            ),
                            if (_erro != null) ...[
                              const SizedBox(height: 12),
                              Text(_erro!, style: TextStyle(color: c.red, fontSize: 13)),
                            ],
                            const SizedBox(height: 20),
                            ElevatedButton(
                              onPressed: _carregando ? null : _entrar,
                              child: _carregando
                                  ? const SizedBox(
                                      height: 20,
                                      width: 20,
                                      child: CircularProgressIndicator(strokeWidth: 2),
                                    )
                                  : const Text('Entrar'),
                            ),
                            const SizedBox(height: 12),
                            Text(
                              'Demo: ${DadosDemo.usuario} / ${DadosDemo.senha}',
                              textAlign: TextAlign.center,
                              style: TextStyle(fontSize: 11, color: c.subtle),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
