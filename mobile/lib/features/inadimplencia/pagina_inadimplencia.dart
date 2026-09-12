import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/widgets/badge_ao_vivo.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class PaginaInadimplencia extends StatelessWidget {
  const PaginaInadimplencia({super.key});

  static const _faixas = [
    ('0–30 dias', 'R\$ 420k', 0.35, Color(0xFF22C55E)),
    ('31–60 dias', 'R\$ 285k', 0.25, Color(0xFFF59E0B)),
    ('61–90 dias', 'R\$ 192k', 0.18, Color(0xFFF97316)),
    ('90+ dias', 'R\$ 145k', 0.22, Color(0xFFEF4444)),
  ];

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const BadgeAoVivo(),
          const SizedBox(height: 16),
          CartaoVidro(
            bordaEsquerda: c.red,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'TOTAL EM ATRASO',
                  style: TextStyle(
                    fontSize: 10,
                    fontWeight: FontWeight.w700,
                    color: c.red,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'R\$ 1.042.000',
                  style: TextStyle(
                    fontSize: 26,
                    fontWeight: FontWeight.w900,
                    color: c.red,
                  ),
                ),
                Text(
                  '6 contas críticas · Risco 30d: R\$ 487.320',
                  style: TextStyle(fontSize: 11, color: c.muted),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          Text(
            'AGING DA CARTEIRA',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: c.muted,
            ),
          ),
          const SizedBox(height: 8),
          ..._faixas.map((f) {
            return Padding(
              padding: const EdgeInsets.only(bottom: 10),
              child: CartaoVidro(
                padding: const EdgeInsets.all(14),
                child: Row(
                  children: [
                    Container(
                      width: 4,
                      height: 40,
                      decoration: BoxDecoration(
                        color: f.$4,
                        borderRadius: BorderRadius.circular(2),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            f.$1,
                            style: TextStyle(
                              fontWeight: FontWeight.w700,
                              color: c.heading,
                            ),
                          ),
                          Text(f.$2, style: TextStyle(color: c.muted, fontSize: 12)),
                        ],
                      ),
                    ),
                    Text(
                      '${(f.$3 * 100).round()}%',
                      style: TextStyle(
                        fontWeight: FontWeight.w800,
                        color: f.$4,
                      ),
                    ),
                  ],
                ),
              ),
            );
          }),
          const SizedBox(height: 8),
          Text(
            'CONTAS CRÍTICAS',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: c.muted,
            ),
          ),
          const SizedBox(height: 8),
          CartaoVidro(
            child: Column(
              children: [
                _Devedor('Comercial Andaza Ltda', 'R\$ 285.000', '12 dias', c),
                Divider(color: c.border, height: 20),
                _Devedor('Distribuidora Norte', 'R\$ 198.000', '45 dias', c),
                Divider(color: c.border, height: 20),
                _Devedor('Grupo Primavera', 'R\$ 156.000', '38 dias', c),
              ],
            ),
          ),
        ],
    );
  }
}

class _Devedor extends StatelessWidget {
  const _Devedor(this.nome, this.valor, this.atraso, this.c);
  final String nome;
  final String valor;
  final String atraso;
  final dynamic c;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                nome,
                style: TextStyle(
                  fontWeight: FontWeight.w600,
                  color: c.heading,
                  fontSize: 13,
                ),
              ),
              Text(atraso, style: TextStyle(fontSize: 11, color: c.muted)),
            ],
          ),
        ),
        Text(
          valor,
          style: TextStyle(
            fontWeight: FontWeight.w800,
            color: c.red,
            fontSize: 13,
          ),
        ),
      ],
    );
  }
}
