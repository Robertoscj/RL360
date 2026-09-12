import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/cores_rl.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/widgets/badge_ao_vivo.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class PaginaVendas extends StatelessWidget {
  const PaginaVendas({super.key});

  static const _funil = [
    ('Leads', 420, 1.0),
    ('Qualificados', 280, 0.67),
    ('Proposta', 156, 0.45),
    ('Negociação', 89, 0.28),
    ('Fechamento', 62, 0.18),
  ];

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const BadgeAoVivo(),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: CartaoVidro(
                  bordaEsquerda: c.emerald,
                  child: _KpiMini('Conversão', '22%', c),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: CartaoVidro(
                  bordaEsquerda: c.red,
                  child: _KpiMini('Queda', '-3,2pp', c),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Text(
            'FUNIL AO VIVO',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: c.muted,
            ),
          ),
          const SizedBox(height: 8),
          ..._funil.map((e) {
            return Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: CartaoVidro(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          e.$1,
                          style: TextStyle(
                            fontWeight: FontWeight.w700,
                            color: c.heading,
                          ),
                        ),
                        Text(
                          '${e.$2}',
                          style: TextStyle(color: c.muted, fontSize: 12),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: LinearProgressIndicator(
                        value: e.$3,
                        minHeight: 28,
                        backgroundColor: c.surface,
                        color: c.emerald.withValues(alpha: 0.7),
                      ),
                    ),
                  ],
                ),
              ),
            );
          }),
        ],
    );
  }
}

class _KpiMini extends StatelessWidget {
  const _KpiMini(this.rotulo, this.valor, this.c);
  final String rotulo;
  final String valor;
  final CoresRl c;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          rotulo.toUpperCase(),
          style: TextStyle(fontSize: 9, fontWeight: FontWeight.w700, color: c.muted),
        ),
        const SizedBox(height: 6),
        Text(
          valor,
          style: TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.w900,
            color: c.heading,
          ),
        ),
      ],
    );
  }
}
