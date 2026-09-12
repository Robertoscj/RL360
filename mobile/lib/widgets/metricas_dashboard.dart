import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class AlertasCriticosRow extends StatelessWidget {
  const AlertasCriticosRow({super.key, required this.alertas});

  final List<AlertaCritico> alertas;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    if (alertas.isEmpty) return const SizedBox.shrink();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'ALERTAS CRÍTICOS (${alertas.length})',
          style: TextStyle(
            fontSize: 10,
            fontWeight: FontWeight.w700,
            color: c.muted,
            letterSpacing: 0.5,
          ),
        ),
        const SizedBox(height: 8),
        SizedBox(
          height: 120,
          child: ListView.separated(
            scrollDirection: Axis.horizontal,
            itemCount: alertas.length,
            separatorBuilder: (_, __) => const SizedBox(width: 10),
            itemBuilder: (_, i) {
              final a = alertas[i];
              final negativo = a.tipo.toLowerCase() != 'positivo';
              final cor = negativo ? c.red : c.emerald;
              return SizedBox(
                width: 200,
                child: CartaoVidro(
                  bordaEsquerda: cor,
                  padding: const EdgeInsets.all(12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        a.titulo,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w700,
                          color: c.heading,
                        ),
                      ),
                      const Spacer(),
                      Text(
                        'Impacto: ${formatarMoedaComSinal(a.impactoFinanceiro)}',
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.w900,
                          color: cor,
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

class MetricasRadarGrid extends StatelessWidget {
  const MetricasRadarGrid({super.key, required this.radar});

  final SnapshotRadar radar;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    final metricas = [
      ('Faturamento dia', formatarMoeda(radar.faturamentoDia), '+'),
      ('Faturamento mês', formatarMoeda(radar.faturamentoMes), '+'),
      ('Meta mensal', formatarMoeda(radar.metaMensal), ''),
      (
        'Meta atingida',
        formatarPercentual(radar.percentualMetaAtingida),
        radar.percentualMetaAtingida >= 100 ? '+' : '-',
      ),
      ('Inadimplência', formatarMoeda(radar.inadimplenciaAtual), '-'),
      ('Conversão', formatarPercentual(radar.taxaConversaoAtual, casas: 1), '-'),
    ];

    return LayoutBuilder(
      builder: (context, constraints) {
        final cols = constraints.maxWidth > 500 ? 3 : 2;
        final rows = (metricas.length / cols).ceil();

        return Column(
          children: [
            for (var r = 0; r < rows; r++)
              Padding(
                padding: EdgeInsets.only(bottom: r < rows - 1 ? 10 : 0),
                child: IntrinsicHeight(
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      for (var col = 0; col < cols; col++) ...[
                        if (col > 0) const SizedBox(width: 10),
                        Expanded(
                          child: _celulaMetrica(
                            c: c,
                            indice: r * cols + col,
                            metricas: metricas,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ),
          ],
        );
      },
    );
  }
}

Widget _celulaMetrica({
  required dynamic c,
  required int indice,
  required List<(String, String, String)> metricas,
}) {
  if (indice >= metricas.length) {
    return const SizedBox.shrink();
  }

  final m = metricas[indice];
  final positivo = m.$3 != '-';

  return CartaoVidro(
    padding: const EdgeInsets.all(12),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          m.$1.toUpperCase(),
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
          style: TextStyle(
            fontSize: 9,
            fontWeight: FontWeight.w700,
            color: c.muted,
          ),
        ),
        const SizedBox(height: 12),
        FittedBox(
          fit: BoxFit.scaleDown,
          alignment: Alignment.centerLeft,
          child: Text(
            m.$2,
            style: TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w900,
              color: c.heading,
            ),
          ),
        ),
        if (m.$3.isNotEmpty) ...[
          const SizedBox(height: 4),
          Text(
            m.$3 == '+' ? '↑ positivo' : '↓ atenção',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: positivo ? c.emerald : c.red,
            ),
          ),
        ],
      ],
    ),
  );
}
