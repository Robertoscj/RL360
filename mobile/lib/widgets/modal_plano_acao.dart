import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';

void mostrarModalPlanoAcao(
  BuildContext context,
  List<ItemPlanoAcao> plano,
) {
  final c = TemaRlExtension.of(context).cores;

  showModalBottomSheet<void>(
    context: context,
    isScrollControlled: true,
    backgroundColor: c.card,
    shape: const RoundedRectangleBorder(
      borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
    ),
    builder: (ctx) {
      return DraggableScrollableSheet(
        expand: false,
        initialChildSize: 0.85,
        minChildSize: 0.5,
        maxChildSize: 0.95,
        builder: (_, controller) {
          return Column(
            children: [
              const SizedBox(height: 8),
              Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: c.border,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              Padding(
                padding: const EdgeInsets.all(20),
                child: Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        color: c.emerald.withValues(alpha: 0.15),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Icon(Icons.gps_fixed, color: c.emerald),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Plano de Ação Sugerido',
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.w900,
                              color: c.heading,
                            ),
                          ),
                          Text(
                            '${plano.length} ações priorizadas pelo radar',
                            style: TextStyle(fontSize: 12, color: c.muted),
                          ),
                        ],
                      ),
                    ),
                    IconButton(
                      onPressed: () => Navigator.pop(ctx),
                      icon: Icon(Icons.close, color: c.muted),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: plano.isEmpty
                    ? Center(
                        child: Text(
                          'Nenhuma ação sugerida no momento.',
                          style: TextStyle(color: c.muted),
                        ),
                      )
                    : ListView.separated(
                        controller: controller,
                        padding: const EdgeInsets.fromLTRB(16, 0, 16, 24),
                        itemCount: plano.length,
                        separatorBuilder: (_, __) => const SizedBox(height: 10),
                        itemBuilder: (_, i) {
                          final item = plano[i];
                          final cor = item.prioridade == 'Urgente'
                              ? c.red
                              : item.prioridade == 'Alta'
                                  ? c.amber
                                  : c.emerald;
                          return Container(
                            padding: const EdgeInsets.all(16),
                            decoration: BoxDecoration(
                              color: cor.withValues(alpha: 0.05),
                              borderRadius: BorderRadius.circular(14),
                              border: Border(
                                left: BorderSide(color: cor, width: 3),
                                top: BorderSide(color: c.border),
                                right: BorderSide(color: c.border),
                                bottom: BorderSide(color: c.border),
                              ),
                            ),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Row(
                                  mainAxisAlignment:
                                      MainAxisAlignment.spaceBetween,
                                  children: [
                                    Container(
                                      padding: const EdgeInsets.symmetric(
                                        horizontal: 8,
                                        vertical: 4,
                                      ),
                                      decoration: BoxDecoration(
                                        color: cor.withValues(alpha: 0.15),
                                        borderRadius: BorderRadius.circular(999),
                                      ),
                                      child: Text(
                                        item.prioridade.toUpperCase(),
                                        style: TextStyle(
                                          fontSize: 9,
                                          fontWeight: FontWeight.w700,
                                          color: cor,
                                        ),
                                      ),
                                    ),
                                    Text(
                                      '+${formatarMoeda(item.impactoEsperado)}',
                                      style: TextStyle(
                                        fontWeight: FontWeight.w900,
                                        color: c.emerald,
                                      ),
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  item.titulo,
                                  style: TextStyle(
                                    fontWeight: FontWeight.w700,
                                    color: c.heading,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  item.justificativa,
                                  style: TextStyle(fontSize: 12, color: c.muted),
                                ),
                              ],
                            ),
                          );
                        },
                      ),
              ),
            ],
          );
        },
      );
    },
  );
}
