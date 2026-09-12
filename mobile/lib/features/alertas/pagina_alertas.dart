import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/demo/dados_demo.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';

class PaginaAlertas extends StatelessWidget {
  const PaginaAlertas({super.key});

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: DadosDemo.alertasCriticos.length,
        separatorBuilder: (_, __) => const SizedBox(height: 10),
        itemBuilder: (_, i) {
          final a = DadosDemo.alertasCriticos[i];
          final critico = a.severidade == 'Critico';
          final cor = critico ? c.red : c.emerald;
          return CartaoVidro(
            bordaEsquerda: cor,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: cor.withValues(alpha: 0.15),
                        borderRadius: BorderRadius.circular(999),
                      ),
                      child: Text(
                        a.severidade.toUpperCase(),
                        style: TextStyle(
                          fontSize: 9,
                          fontWeight: FontWeight.w700,
                          color: cor,
                        ),
                      ),
                    ),
                    const Spacer(),
                    Text(
                      a.impacto,
                      style: TextStyle(
                        fontWeight: FontWeight.w900,
                        color: cor,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                Text(
                  a.titulo,
                  style: TextStyle(
                    fontWeight: FontWeight.w700,
                    color: c.heading,
                  ),
                ),
              ],
            ),
          );
        },
    );
  }
}
