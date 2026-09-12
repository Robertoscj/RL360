import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';

class CartaoVidro extends StatelessWidget {
  const CartaoVidro({
    super.key,
    required this.child,
    this.padding = const EdgeInsets.all(16),
    this.bordaEsquerda,
  });

  final Widget child;
  final EdgeInsets padding;
  final Color? bordaEsquerda;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    return Container(
      padding: padding,
      decoration: BoxDecoration(
        color: c.card,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: c.border),
      ),
      foregroundDecoration: bordaEsquerda != null
          ? BoxDecoration(
              borderRadius: BorderRadius.circular(16),
              border: Border(left: BorderSide(color: bordaEsquerda!, width: 3)),
            )
          : null,
      child: child,
    );
  }
}
