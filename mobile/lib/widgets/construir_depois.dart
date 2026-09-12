import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';

/// Constrói widgets pesados após o primeiro frame — UI responde ao toque antes.
class ConstruirDepois extends StatefulWidget {
  const ConstruirDepois({
    super.key,
    required this.builder,
    this.alturaReserva = 180,
    this.atraso = Duration.zero,
  });

  final Widget Function() builder;
  final double alturaReserva;
  final Duration atraso;

  @override
  State<ConstruirDepois> createState() => _ConstruirDepoisState();
}

class _ConstruirDepoisState extends State<ConstruirDepois> {
  bool _pronto = false;

  @override
  void initState() {
    super.initState();
    Future<void>.delayed(widget.atraso, () {
      if (mounted) setState(() => _pronto = true);
    });
  }

  @override
  Widget build(BuildContext context) {
    if (_pronto) return widget.builder();

    final c = TemaRlExtension.of(context).cores;
    return SizedBox(
      height: widget.alturaReserva,
      child: Center(
        child: SizedBox(
          width: 22,
          height: 22,
          child: CircularProgressIndicator(
            strokeWidth: 2,
            color: c.emerald.withValues(alpha: 0.7),
          ),
        ),
      ),
    );
  }
}
