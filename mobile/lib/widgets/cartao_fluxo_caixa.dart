import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class CartaoFluxoCaixa extends StatefulWidget {
  const CartaoFluxoCaixa({super.key, required this.fluxo});

  final FluxoCaixaFuturo fluxo;

  @override
  State<CartaoFluxoCaixa> createState() => _CartaoFluxoCaixaState();
}

class _CartaoFluxoCaixaState extends State<CartaoFluxoCaixa> {
  late PieChartData _pizza;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _montarGrafico();
  }

  @override
  void didUpdateWidget(CartaoFluxoCaixa oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.fluxo != widget.fluxo) {
      _montarGrafico();
    }
  }

  void _montarGrafico() {
    final c = TemaRlExtension.of(context).cores;
    final fluxo = widget.fluxo;

    _pizza = PieChartData(
      pieTouchData: PieTouchData(enabled: false),
      sectionsSpace: 2,
      centerSpaceRadius: 48,
      sections: fluxo.segmentos.isEmpty
          ? [
              PieChartSectionData(
                value: 1,
                color: c.border,
                radius: 66,
                showTitle: false,
              ),
            ]
          : fluxo.segmentos
              .map(
                (s) => PieChartSectionData(
                  value: s.valor <= 0 ? 0.001 : s.valor,
                  color: parseCorHex(s.cor),
                  radius: 66,
                  showTitle: false,
                ),
              )
              .toList(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    final fluxo = widget.fluxo;
    final itens = [
      (rotulo: 'A receber', valor: fluxo.aReceber, cor: c.emerald),
      (rotulo: 'Em risco', valor: fluxo.emRisco, cor: c.amber),
      (rotulo: 'Atrasado', valor: fluxo.atrasado, cor: c.red),
    ];

    return CartaoVidro(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'FLUXO DE CAIXA FUTURO',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: c.muted,
              letterSpacing: 0.5,
            ),
          ),
          Text(
            'Próximos ${fluxo.dias} dias',
            style: TextStyle(fontSize: 11, color: c.subtle),
          ),
          const SizedBox(height: 16),
          Center(
            child: SizedBox(
              width: 140,
              height: 140,
              child: Stack(
                alignment: Alignment.center,
                children: [
                  PieChart(
                    _pizza,
                    duration: Duration.zero,
                    curve: Curves.linear,
                  ),
                  Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        '${fluxo.percentualSaudavel}%',
                        style: TextStyle(
                          fontSize: 22,
                          fontWeight: FontWeight.w900,
                          color: c.emerald,
                        ),
                      ),
                      Text(
                        'Saudável',
                        style: TextStyle(fontSize: 10, color: c.muted),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          ...itens.map(
            (item) => Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Row(
                children: [
                  Container(
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      color: item.cor,
                      shape: BoxShape.circle,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      item.rotulo,
                      style: TextStyle(fontSize: 11, color: c.muted),
                    ),
                  ),
                  Flexible(
                    child: Text(
                      formatarMoeda(item.valor),
                      style: TextStyle(
                        fontSize: 11,
                        fontWeight: FontWeight.w700,
                        color: item.cor,
                      ),
                      overflow: TextOverflow.ellipsis,
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
