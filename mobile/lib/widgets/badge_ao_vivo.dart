import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';

class BadgeAoVivo extends StatelessWidget {
  const BadgeAoVivo({super.key, this.atualizando = false});

  final bool atualizando;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: c.emerald.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: c.emerald.withValues(alpha: 0.3)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            Icons.sensors,
            size: 12,
            color: c.emerald,
          ),
          const SizedBox(width: 6),
          Text(
            atualizando ? 'ATUALIZANDO RADAR...' : 'RADAR AO VIVO',
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w700,
              letterSpacing: 0.5,
              color: c.emerald,
            ),
          ),
        ],
      ),
    );
  }
}
