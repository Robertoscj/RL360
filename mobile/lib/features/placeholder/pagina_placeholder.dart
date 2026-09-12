import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class PaginaPlaceholder extends StatelessWidget {
  const PaginaPlaceholder({
    super.key,
    required this.titulo,
    required this.subtitulo,
    required this.rota,
  });

  final String titulo;
  final String subtitulo;
  final String rota;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return Center(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: CartaoVidro(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(Icons.construction_outlined, size: 48, color: c.muted),
                const SizedBox(height: 16),
                Text(
                  titulo,
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w700,
                    color: c.heading,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  subtitulo,
                  textAlign: TextAlign.center,
                  style: TextStyle(color: c.muted, fontSize: 13),
                ),
                const SizedBox(height: 12),
                Text(
                  'Em construção — integração com API em breve',
                  style: TextStyle(fontSize: 11, color: c.emerald),
                ),
              ],
            ),
          ),
        ),
    );
  }
}
