import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/demo/dados_demo.dart';
import 'package:lucro360_mobile/core/theme/cores_rl.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/widgets/badge_ao_vivo.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class PaginaFaturamento extends StatelessWidget {
  const PaginaFaturamento({super.key});

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const BadgeAoVivo(),
          const SizedBox(height: 16),
          ...DadosDemo.faturamentoKpis.map((k) {
            return Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: CartaoVidro(
                bordaEsquerda: c.emerald,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      k.$1.toUpperCase(),
                      style: TextStyle(
                        fontSize: 10,
                        fontWeight: FontWeight.w700,
                        color: c.emerald,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      k.$2,
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.w900,
                        color: c.heading,
                      ),
                    ),
                    Text(k.$3, style: TextStyle(fontSize: 11, color: c.muted)),
                  ],
                ),
              ),
            );
          }),
          CartaoVidro(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'EVOLUÇÃO DIÁRIA',
                  style: TextStyle(
                    fontSize: 10,
                    fontWeight: FontWeight.w700,
                    color: c.muted,
                  ),
                ),
                const SizedBox(height: 12),
                SizedBox(
                  height: 160,
                  child: CustomPaint(
                    size: const Size(double.infinity, 160),
                    painter: _GraficoLinhaDemo(cor: c.emerald, grid: c.border),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          CartaoVidro(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'FATURAMENTO POR CANAL',
                  style: TextStyle(
                    fontSize: 10,
                    fontWeight: FontWeight.w700,
                    color: c.muted,
                  ),
                ),
                const SizedBox(height: 12),
                _BarraCanal('Direct Sales', 0.85, c.emerald, c),
                _BarraCanal('Parceiros', 0.62, c.blue, c),
                _BarraCanal('E-commerce', 0.45, c.amber, c),
              ],
            ),
          ),
        ],
    );
  }
}

class _BarraCanal extends StatelessWidget {
  const _BarraCanal(this.nome, this.pct, this.cor, this.cores);
  final String nome;
  final double pct;
  final Color cor;
  final CoresRl cores;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(nome, style: TextStyle(fontSize: 12, color: cores.body)),
              Text(
                '${(pct * 100).round()}%',
                style: TextStyle(fontSize: 11, fontWeight: FontWeight.w700, color: cores.muted),
              ),
            ],
          ),
          const SizedBox(height: 4),
          ClipRRect(
            borderRadius: BorderRadius.circular(4),
            child: LinearProgressIndicator(
              value: pct,
              minHeight: 8,
              backgroundColor: cores.surface,
              color: cor,
            ),
          ),
        ],
      ),
    );
  }
}

class _GraficoLinhaDemo extends CustomPainter {
  _GraficoLinhaDemo({required this.cor, required this.grid});
  final Color cor;
  final Color grid;

  @override
  void paint(Canvas canvas, Size size) {
    final paintGrid = Paint()
      ..color = grid
      ..strokeWidth = 1;
    for (var i = 1; i < 4; i++) {
      final y = size.height * i / 4;
      canvas.drawLine(Offset(0, y), Offset(size.width, y), paintGrid);
    }
    final path = Path();
    const pontos = [0.3, 0.45, 0.4, 0.55, 0.5, 0.65, 0.7, 0.62, 0.75, 0.8];
    for (var i = 0; i < pontos.length; i++) {
      final x = size.width * i / (pontos.length - 1);
      final y = size.height * (1 - pontos[i]);
      if (i == 0) {
        path.moveTo(x, y);
      } else {
        path.lineTo(x, y);
      }
    }
    canvas.drawPath(
      path,
      Paint()
        ..color = cor
        ..style = PaintingStyle.stroke
        ..strokeWidth = 2.5,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
