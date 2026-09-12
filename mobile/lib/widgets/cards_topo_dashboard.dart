import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/cores_rl.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/widgets/cartao_kpi.dart';

class CardsTopoDashboard extends StatelessWidget {
  const CardsTopoDashboard({super.key, required this.cards});

  final CardsTopo cards;

  Color _corSaude(CoresRl c) {
    if (cards.saudeEmpresaPercentual >= 70) return c.emerald;
    if (cards.saudeEmpresaPercentual >= 50) return c.amber;
    return c.red;
  }

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    final corSaude = _corSaude(c);

    final itens = [
      (
        titulo: 'RISCO PRÓX. 30 DIAS',
        valor: formatarMoeda(cards.riscoProximos30Dias),
        subtitulo: 'Impacto financeiro estimado',
        cor: c.red,
        icone: Icons.warning_amber_outlined,
      ),
      (
        titulo: 'OPORTUNIDADE',
        valor: formatarMoeda(cards.valorOportunidade),
        subtitulo: 'Potencial de ganho identificado',
        cor: c.emerald,
        icone: Icons.gps_fixed,
      ),
      (
        titulo: 'GARGALOS CRÍTICOS',
        valor: '${cards.gargalosCriticos}',
        subtitulo: 'Pontos que exigem ação imediata',
        cor: c.amber,
        icone: Icons.bolt_outlined,
      ),
      (
        titulo: 'SAÚDE DA EMPRESA',
        valor: '${cards.saudeEmpresaPercentual}%',
        subtitulo: cards.statusSaude,
        cor: corSaude,
        icone: Icons.favorite_border,
      ),
    ];

    return LayoutBuilder(
      builder: (context, constraints) {
        final largo = constraints.maxWidth > 600;
        if (largo) {
          return Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              for (var i = 0; i < itens.length; i++) ...[
                if (i > 0) const SizedBox(width: 10),
                Expanded(
                  child: CartaoKpi(
                    titulo: itens[i].titulo,
                    valor: itens[i].valor,
                    subtitulo: itens[i].subtitulo,
                    corAccent: itens[i].cor,
                    icone: itens[i].icone,
                  ),
                ),
              ],
            ],
          );
        }

        return Column(
          children: [
            for (var i = 0; i < itens.length; i++)
              Padding(
                padding: EdgeInsets.only(bottom: i < itens.length - 1 ? 10 : 0),
                child: CartaoKpi(
                  titulo: itens[i].titulo,
                  valor: itens[i].valor,
                  subtitulo: itens[i].subtitulo,
                  corAccent: itens[i].cor,
                  icone: itens[i].icone,
                ),
              ),
          ],
        );
      },
    );
  }
}
