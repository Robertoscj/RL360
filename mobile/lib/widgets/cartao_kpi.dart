import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';
class CartaoKpi extends StatelessWidget {
  const CartaoKpi({
    super.key,
    required this.titulo,
    required this.valor,
    required this.subtitulo,
    required this.corAccent,
    this.icone,
  });

  final String titulo;
  final String valor;
  final String subtitulo;
  final Color corAccent;
  final IconData? icone;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    return CartaoVidro(
      bordaEsquerda: corAccent,
      padding: const EdgeInsets.all(16),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  titulo,
                  style: TextStyle(
                    fontSize: 10,
                    fontWeight: FontWeight.w700,
                    letterSpacing: 0.5,
                    color: corAccent,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  valor,
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.w900,
                    color: corAccent,
                    height: 1,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  subtitulo,
                  style: TextStyle(fontSize: 11, color: c.muted, height: 1.3),
                ),
              ],
            ),
          ),
          if (icone != null)
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: corAccent.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Icon(icone, color: corAccent, size: 20),
            ),
        ],
      ),
    );
  }
}

class BannerRodape extends StatelessWidget {
  const BannerRodape({
    super.key,
    required this.mensagem,
    required this.quantidadeAcoes,
    required this.onAbrirPlano,
  });

  final String mensagem;
  final int quantidadeAcoes;
  final VoidCallback onAbrirPlano;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        gradient: const LinearGradient(
          colors: [Color(0xFF15803D), Color(0xFF16A34A), Color(0xFF22C55E)],
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              const Icon(Icons.emoji_events_outlined, color: Colors.white, size: 20),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  mensagem.isNotEmpty
                      ? mensagem.toUpperCase()
                      : 'EMPRESAS QUE USAM O RL360 TÊM EM MÉDIA 23% MAIS LUCRO.',
                  style: const TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.w700,
                    color: Colors.white,
                    letterSpacing: 0.3,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          OutlinedButton(
            onPressed: onAbrirPlano,
            style: OutlinedButton.styleFrom(
              foregroundColor: Colors.white,
              side: BorderSide(color: Colors.white.withValues(alpha: 0.3)),
              backgroundColor: Colors.white.withValues(alpha: 0.2),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  'Plano de ação sugerido ($quantidadeAcoes ações)',
                  style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 12),
                ),
                const SizedBox(width: 4),
                const Icon(Icons.chevron_right, size: 18),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
