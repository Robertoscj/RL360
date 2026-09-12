import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class _NoRadar {
  const _NoRadar({
    required this.fator,
    required this.angulo,
    required this.rotulo,
    required this.icone,
    required this.dados,
  });

  final String fator;
  final double angulo;
  final String rotulo;
  final IconData icone;
  final FatorRadar dados;
}

class _EstiloNo {
  const _EstiloNo({
    required this.cor,
    required this.corBg,
    required this.corBorda,
    required this.corLinha,
  });

  final Color cor;
  final Color corBg;
  final Color corBorda;
  final Color corLinha;
}

const _ordem = [
  ('Inadimplencia', -90.0, 'INADIMPLÊNCIA', Icons.warning_amber_outlined),
  ('GargaloComercial', -30.0, 'GARGALO COMERCIAL', Icons.schedule_outlined),
  ('ExpansaoClientes', 30.0, 'EXPANSÃO CLIENTES', Icons.bar_chart_outlined),
  ('Produtividade', 90.0, 'PRODUTIVIDADE', Icons.settings_outlined),
  ('VendasNovas', 150.0, 'VENDAS NOVAS', Icons.shopping_cart_outlined),
  ('Conversao', 210.0, 'CONVERSÃO', Icons.filter_alt_outlined),
];

_EstiloNo _estiloNo(FatorRadar fator, dynamic c) {
  if (fator.fator == 'Inadimplencia') {
    return _EstiloNo(
      cor: c.red,
      corBg: c.red.withValues(alpha: 0.14),
      corBorda: c.red.withValues(alpha: 0.55),
      corLinha: c.red,
    );
  }
  if (fator.direcao == 'positivo' || fator.impactoFinanceiro >= 0) {
    return _EstiloNo(
      cor: c.emerald,
      corBg: c.emerald.withValues(alpha: 0.14),
      corBorda: c.emerald.withValues(alpha: 0.55),
      corLinha: c.emerald,
    );
  }
  return _EstiloNo(
    cor: c.amber,
    corBg: c.amber.withValues(alpha: 0.14),
    corBorda: c.amber.withValues(alpha: 0.55),
    corLinha: c.amber,
  );
}

List<_NoRadar> _montarNos(List<FatorRadar> fatores) {
  final mapa = {for (final f in fatores) f.fator: f};
  return _ordem.map((o) {
    final dados = mapa[o.$1] ??
        FatorRadar(
          fator: o.$1,
          rotulo: o.$3,
          impactoFinanceiro: 0,
          direcao: 'negativo',
          descricao: 'Sem dados no período',
        );
    return _NoRadar(
      fator: o.$1,
      angulo: o.$2,
      rotulo: o.$3,
      icone: o.$4,
      dados: dados,
    );
  }).toList();
}

class RadarHexagonal extends StatefulWidget {
  const RadarHexagonal({
    super.key,
    required this.lucroAtual,
    required this.fatores,
    this.variacaoPercentual,
  });

  final double lucroAtual;
  final List<FatorRadar> fatores;
  final double? variacaoPercentual;

  @override
  State<RadarHexagonal> createState() => _RadarHexagonalState();
}

class _RadarHexagonalState extends State<RadarHexagonal> {
  late List<_NoRadar> _nos;

  @override
  void initState() {
    super.initState();
    _nos = _montarNos(widget.fatores);
  }

  @override
  void didUpdateWidget(RadarHexagonal oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.fatores != widget.fatores) {
      _nos = _montarNos(widget.fatores);
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    final variacaoPositiva = (widget.variacaoPercentual ?? 0) >= 0;
    final variacaoTexto = widget.variacaoPercentual != null
        ? '${variacaoPositiva ? '+' : ''}${widget.variacaoPercentual!.toStringAsFixed(1).replaceAll('.', ',')}% vs média do mês'
        : 'Atualizado em tempo real';

    return CartaoVidro(
      padding: const EdgeInsets.all(12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'RADAR DE LUCRO',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              color: c.muted,
              letterSpacing: 0.5,
            ),
          ),
          const SizedBox(height: 8),
          AspectRatio(
            aspectRatio: 560 / 420,
            child: _RadarCanvas(
              nos: _nos,
              lucroAtual: widget.lucroAtual,
              variacaoTexto: variacaoTexto,
              variacaoPositiva: variacaoPositiva,
              emerald: c.emerald,
              hub: c.radarHub,
              border: c.border,
              muted: c.muted,
              heading: c.heading,
              cores: c,
            ),
          ),
        ],
      ),
    );
  }
}

class _RadarCanvas extends StatelessWidget {
  const _RadarCanvas({
    required this.nos,
    required this.lucroAtual,
    required this.variacaoTexto,
    required this.variacaoPositiva,
    required this.emerald,
    required this.hub,
    required this.border,
    required this.muted,
    required this.heading,
    required this.cores,
  });

  final List<_NoRadar> nos;
  final double lucroAtual;
  final String variacaoTexto;
  final bool variacaoPositiva;
  final Color emerald;
  final Color hub;
  final Color border;
  final Color muted;
  final Color heading;
  final dynamic cores;

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final w = constraints.maxWidth;
        final h = constraints.maxHeight;
        const refW = 560.0;
        const refH = 420.0;
        final escala = math.min(w / refW, h / refH);
        final fontScale = escala.clamp(0.65, 1.0);

        return Stack(
          clipBehavior: Clip.none,
          children: [
            CustomPaint(
              size: Size(w, h),
              isComplex: true,
              willChange: false,
              painter: _RadarPainter(
                nos: nos,
                emerald: emerald,
                hub: hub,
                border: border,
              ),
            ),
            Positioned.fill(
              child: Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      'LUCRO ATUAL',
                      style: TextStyle(
                        fontSize: 9 * fontScale,
                        fontWeight: FontWeight.w700,
                        color: muted,
                        letterSpacing: 1.2,
                      ),
                    ),
                    const SizedBox(height: 4),
                    FittedBox(
                      child: Text(
                        formatarMoeda(lucroAtual),
                        style: TextStyle(
                          fontSize: 21 * fontScale,
                          fontWeight: FontWeight.w900,
                          color: heading,
                        ),
                      ),
                    ),
                    Text(
                      variacaoTexto,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 10 * fontScale,
                        fontWeight: FontWeight.w600,
                        color: variacaoPositiva ? emerald : cores.red,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            ...nos.map((no) {
              final est = _estiloNo(no.dados, cores);
              final rad = no.angulo * math.pi / 180;
              const dist = 168 / 560;
              final left = w * (0.5 + dist * math.cos(rad));
              final top = h * (0.5 + dist * math.sin(rad));
              final nodeW = (108 * escala).clamp(72.0, 108.0);

              return Positioned(
                left: left - nodeW / 2,
                top: top - 28 * escala,
                width: nodeW,
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Container(
                      width: 22 * fontScale,
                      height: 22 * fontScale,
                      decoration: BoxDecoration(
                        color: est.cor.withValues(alpha: 0.13),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Icon(no.icone, size: 12 * fontScale, color: est.cor),
                    ),
                    const SizedBox(width: 4),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            no.dados.rotulo.isNotEmpty
                                ? no.dados.rotulo
                                : no.rotulo,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              fontSize: 7 * fontScale,
                              fontWeight: FontWeight.w700,
                              color: muted,
                            ),
                          ),
                          Text(
                            formatarMoedaComSinal(no.dados.impactoFinanceiro),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              fontSize: 11 * fontScale,
                              fontWeight: FontWeight.w900,
                              color: est.cor,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              );
            }),
          ],
        );
      },
    );
  }
}

class _RadarPainter extends CustomPainter {
  _RadarPainter({
    required this.nos,
    required this.emerald,
    required this.hub,
    required this.border,
  });

  final List<_NoRadar> nos;
  final Color emerald;
  final Color hub;
  final Color border;

  List<Offset> _hexPoints(Offset center, double r) {
    return List.generate(6, (i) {
      final rad = (60 * i - 90) * math.pi / 180;
      return Offset(
        center.dx + r * math.cos(rad),
        center.dy + r * math.sin(rad),
      );
    });
  }

  Path _hexPath(List<Offset> pts) {
    final p = Path()..moveTo(pts[0].dx, pts[0].dy);
    for (var i = 1; i < pts.length; i++) {
      p.lineTo(pts[i].dx, pts[i].dy);
    }
    p.close();
    return p;
  }

  Offset _pos(Offset center, double angulo, double dist) {
    final rad = angulo * math.pi / 180;
    return Offset(
      center.dx + dist * math.cos(rad),
      center.dy + dist * math.sin(rad),
    );
  }

  @override
  void paint(Canvas canvas, Size size) {
    final center = Offset(size.width / 2, size.height / 2);
    final raioExterno = size.width * 0.30;
    final raioHub = size.width * 0.104;
    const escalas = [0.32, 0.52, 0.72, 0.92];

    for (final s in escalas) {
      canvas.drawPath(
        _hexPath(_hexPoints(center, raioExterno * s)),
        Paint()
          ..style = PaintingStyle.stroke
          ..color = emerald.withValues(alpha: 0.08 + s * 0.04)
          ..strokeWidth = 1,
      );
    }

    canvas.drawPath(
      _hexPath(_hexPoints(center, raioExterno)),
      Paint()
        ..style = PaintingStyle.stroke
        ..color = emerald.withValues(alpha: 0.2)
        ..strokeWidth = 1.2,
    );

    for (var i = 0; i < 6; i++) {
      canvas.drawLine(
        center,
        _pos(center, nos[i].angulo, raioExterno),
        Paint()
          ..color = emerald.withValues(alpha: 0.35)
          ..strokeWidth = 1.5,
      );
    }

    canvas.drawPath(_hexPath(_hexPoints(center, raioHub)), Paint()..color = hub);
    canvas.drawPath(
      _hexPath(_hexPoints(center, raioHub)),
      Paint()
        ..style = PaintingStyle.stroke
        ..color = emerald
        ..strokeWidth = 2,
    );

    final nodeR = size.width * 0.052;
    for (final no in nos) {
      canvas.drawPath(
        _hexPath(_hexPoints(_pos(center, no.angulo, raioExterno), nodeR)),
        Paint()..color = border.withValues(alpha: 0.35),
      );
    }
  }

  @override
  bool shouldRepaint(covariant _RadarPainter oldDelegate) =>
      !identical(oldDelegate.nos, nos) ||
      oldDelegate.emerald != emerald ||
      oldDelegate.hub != hub ||
      oldDelegate.border != border;
}
