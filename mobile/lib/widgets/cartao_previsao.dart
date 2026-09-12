import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class CartaoPrevisao extends StatefulWidget {
  const CartaoPrevisao({super.key, required this.previsao});

  final PrevisaoResultado previsao;

  @override
  State<CartaoPrevisao> createState() => _CartaoPrevisaoState();
}

class _CartaoPrevisaoState extends State<CartaoPrevisao> {
  late LineChartData _linha;
  late BarChartData _barras;
  late List<PontoSerieTemporal> _dados;
  late bool _abaixoMeta;
  late Color _corValor;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _montarGraficos();
  }

  @override
  void didUpdateWidget(CartaoPrevisao oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.previsao != widget.previsao) {
      _montarGraficos();
    }
  }

  void _montarGraficos() {
    final previsao = widget.previsao;
    _abaixoMeta = previsao.percentualAbaixoMeta > 0;
    _dados = previsao.serie.length > 12
        ? previsao.serie.sublist(previsao.serie.length - 12)
        : previsao.serie;

    _corValor = _abaixoMeta
        ? TemaRlExtension.of(context).cores.red
        : TemaRlExtension.of(context).cores.emerald;

    final c = TemaRlExtension.of(context).cores;

    _linha = LineChartData(
      lineTouchData: LineTouchData(enabled: false),
      gridData: FlGridData(
        show: true,
        drawVerticalLine: false,
        horizontalInterval: 1,
        getDrawingHorizontalLine: (_) => FlLine(
          color: c.border,
          strokeWidth: 1,
          dashArray: const [3, 3],
        ),
      ),
      titlesData: FlTitlesData(
        leftTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        bottomTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: true,
            reservedSize: 22,
            interval: _dados.length > 6 ? 2 : 1,
            getTitlesWidget: (v, _) {
              final i = v.toInt();
              if (i < 0 || i >= _dados.length) {
                return const SizedBox.shrink();
              }
              return Padding(
                padding: const EdgeInsets.only(top: 4),
                child: Text(
                  _dados[i].rotulo,
                  style: TextStyle(fontSize: 7, color: c.subtle),
                ),
              );
            },
          ),
        ),
      ),
      borderData: FlBorderData(show: false),
      lineBarsData: [
        if (_dados.any((p) => p.meta != null))
          LineChartBarData(
            spots: [
              for (var i = 0; i < _dados.length; i++)
                FlSpot(i.toDouble(), _dados[i].meta ?? _dados[i].valor),
            ],
            isCurved: true,
            color: c.subtle,
            barWidth: 1.5,
            dashArray: const [4, 4],
            dotData: const FlDotData(show: false),
          ),
        LineChartBarData(
          spots: [
            for (var i = 0; i < _dados.length; i++)
              FlSpot(i.toDouble(), _dados[i].valor),
          ],
          isCurved: true,
          color: _corValor,
          barWidth: 2.5,
          dotData: const FlDotData(show: false),
        ),
      ],
    );

    final maxY = [
      previsao.meta,
      previsao.cenarioMaisProvavel,
    ].reduce((a, b) => a > b ? a : b) *
        1.1;

    _barras = BarChartData(
      barTouchData: BarTouchData(enabled: false),
      alignment: BarChartAlignment.spaceAround,
      maxY: maxY,
      titlesData: FlTitlesData(
        leftTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        bottomTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: true,
            getTitlesWidget: (v, _) {
              const labels = ['Meta', 'Estimado'];
              final i = v.toInt();
              if (i < 0 || i >= labels.length) {
                return const SizedBox.shrink();
              }
              return Text(
                labels[i],
                style: TextStyle(fontSize: 9, color: c.subtle),
              );
            },
          ),
        ),
      ),
      gridData: const FlGridData(show: false),
      borderData: FlBorderData(show: false),
      barGroups: [
        BarChartGroupData(
          x: 0,
          barRods: [
            BarChartRodData(
              toY: previsao.meta,
              color: c.subtle,
              width: 14,
              borderRadius: BorderRadius.circular(4),
            ),
          ],
        ),
        BarChartGroupData(
          x: 1,
          barRods: [
            BarChartRodData(
              toY: previsao.cenarioMaisProvavel,
              color: _corValor,
              width: 14,
              borderRadius: BorderRadius.circular(4),
            ),
          ],
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    final previsao = widget.previsao;

    return CartaoVidro(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'PREVISÃO DE RESULTADO',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: c.muted,
              letterSpacing: 0.5,
            ),
          ),
          Text(
            'Próximos ${previsao.dias} dias',
            style: TextStyle(fontSize: 11, color: c.subtle),
          ),
          const SizedBox(height: 12),
          FittedBox(
            fit: BoxFit.scaleDown,
            alignment: Alignment.centerLeft,
            child: Text(
              formatarMoeda(previsao.cenarioMaisProvavel),
              style: TextStyle(
                fontSize: 22,
                fontWeight: FontWeight.w900,
                color: _corValor,
              ),
            ),
          ),
          Text(
            _abaixoMeta
                ? '-${formatarPercentual(previsao.percentualAbaixoMeta)} abaixo da meta'
                : 'Na meta ou acima do esperado',
            style: TextStyle(
              fontSize: 11,
              fontWeight: FontWeight.w700,
              color: _corValor,
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            height: 140,
            child: _dados.isEmpty
                ? Center(
                    child: Text('Sem dados', style: TextStyle(color: c.muted)),
                  )
                : LineChart(
                    _linha,
                    duration: Duration.zero,
                    curve: Curves.linear,
                  ),
          ),
          const Divider(height: 24),
          SizedBox(
            height: 56,
            child: BarChart(
              _barras,
              duration: Duration.zero,
              curve: Curves.linear,
            ),
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Flexible(
                child: Text(
                  'Meta: ${formatarMoeda(previsao.meta)}',
                  style: TextStyle(fontSize: 9, color: c.subtle),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              Flexible(
                child: Text(
                  'Estimado: ${formatarMoeda(previsao.cenarioMaisProvavel)}',
                  style: TextStyle(fontSize: 9, color: _corValor),
                  textAlign: TextAlign.end,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
