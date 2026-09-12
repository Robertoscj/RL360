import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/api/api_client.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/services/ia_service.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';
import 'package:provider/provider.dart';

class _EstiloInsight {
  const _EstiloInsight({
    required this.borda,
    required this.fundo,
    required this.rotulo,
  });

  final Color borda;
  final Color fundo;
  final Color rotulo;
}

_EstiloInsight _estiloInsight(String tipo, dynamic c) {
  switch (tipo) {
    case 'AcaoUrgente':
      return _EstiloInsight(borda: c.red, fundo: c.red.withValues(alpha: 0.07), rotulo: c.red);
    case 'Gargalo':
      return _EstiloInsight(
        borda: c.amber,
        fundo: c.amber.withValues(alpha: 0.07),
        rotulo: c.amber,
      );
    default:
      return _EstiloInsight(
        borda: c.emerald,
        fundo: c.emerald.withValues(alpha: 0.07),
        rotulo: c.emerald,
      );
  }
}

String _tituloTipo(String tipo) {
  switch (tipo) {
    case 'AcaoUrgente':
      return 'Ação urgente recomendada';
    case 'Gargalo':
      return 'Gargalo identificado';
    case 'Oportunidade':
      return 'Oportunidade real';
    default:
      return tipo;
  }
}

const _sugestoes = [
  'Por que a conversão caiu?',
  'Como reduzir a inadimplência?',
  'Qual o maior risco agora?',
];

class PainelInsights extends StatefulWidget {
  const PainelInsights({super.key, required this.insights});

  final List<InsightInteligente> insights;

  @override
  State<PainelInsights> createState() => _PainelInsightsState();
}

class _PainelInsightsState extends State<PainelInsights> {
  final _ctrl = TextEditingController();
  final _mensagens = <({bool usuario, String texto})>[];
  bool _carregando = false;
  String? _erro;

  @override
  void dispose() {
    _ctrl.dispose();
    super.dispose();
  }

  Future<void> _enviar(String texto) async {
    final pergunta = texto.trim();
    if (pergunta.isEmpty || _carregando) return;

    setState(() {
      _carregando = true;
      _erro = null;
      _mensagens.add((usuario: true, texto: pergunta));
      _ctrl.clear();
    });

    try {
      final ia = IaService(context.read<ApiClient>());
      final resposta = await ia.perguntar(pergunta);
      if (!mounted) return;
      setState(() {
        _mensagens.add((usuario: false, texto: resposta.resposta));
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _erro = e.toString());
    } finally {
      if (mounted) setState(() => _carregando = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return CartaoVidro(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Icon(Icons.auto_awesome, size: 16, color: c.emerald),
              const SizedBox(width: 8),
              Text(
                'INSIGHTS INTELIGENTES',
                style: TextStyle(
                  fontSize: 10,
                  fontWeight: FontWeight.w700,
                  color: c.muted,
                  letterSpacing: 0.5,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          ...widget.insights.map((insight) {
            final est = _estiloInsight(insight.tipo, c);
            return Container(
              margin: const EdgeInsets.only(bottom: 10),
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: est.fundo,
                borderRadius: BorderRadius.circular(10),
                border: Border(
                  left: BorderSide(color: est.borda, width: 3),
                  top: BorderSide(color: c.border),
                  right: BorderSide(color: c.border),
                  bottom: BorderSide(color: c.border),
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    _tituloTipo(insight.tipo).toUpperCase(),
                    style: TextStyle(
                      fontSize: 9,
                      fontWeight: FontWeight.w700,
                      color: est.rotulo,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    insight.titulo,
                    style: TextStyle(
                      fontSize: 11,
                      fontWeight: FontWeight.w600,
                      color: c.heading,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    insight.descricao,
                    style: TextStyle(fontSize: 10, color: c.muted, height: 1.3),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    '${insight.textoBotao} →',
                    style: TextStyle(
                      fontSize: 10,
                      fontWeight: FontWeight.w700,
                      color: est.rotulo,
                    ),
                  ),
                ],
              ),
            );
          }),
          const Divider(height: 24),
          Text(
            'Pergunte para a IA',
            style: TextStyle(fontSize: 10, fontWeight: FontWeight.w600, color: c.muted),
          ),
          if (_mensagens.isNotEmpty) ...[
            const SizedBox(height: 8),
            ..._mensagens.map((msg) {
              return Align(
                alignment:
                    msg.usuario ? Alignment.centerRight : Alignment.centerLeft,
                child: Container(
                  margin: const EdgeInsets.only(bottom: 6),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 8,
                  ),
                  constraints: BoxConstraints(
                    maxWidth: MediaQuery.sizeOf(context).width * 0.7,
                  ),
                  decoration: BoxDecoration(
                    color: msg.usuario
                        ? c.emerald.withValues(alpha: 0.1)
                        : c.surface,
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Text(
                    msg.texto,
                    style: TextStyle(fontSize: 10, color: c.body, height: 1.3),
                  ),
                ),
              );
            }),
          ],
          if (_mensagens.isEmpty) ...[
            const SizedBox(height: 8),
            Wrap(
              spacing: 6,
              runSpacing: 6,
              children: _sugestoes
                  .map(
                    (s) => ActionChip(
                      label: Text(s, style: const TextStyle(fontSize: 9)),
                      onPressed: _carregando ? null : () => _enviar(s),
                      visualDensity: VisualDensity.compact,
                    ),
                  )
                  .toList(),
            ),
          ],
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _ctrl,
                  enabled: !_carregando,
                  maxLength: 500,
                  style: TextStyle(fontSize: 11, color: c.body),
                  decoration: InputDecoration(
                    hintText: 'Ex: Por que a conversão caiu?',
                    counterText: '',
                    isDense: true,
                    contentPadding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 10,
                    ),
                  ),
                  onSubmitted: _enviar,
                ),
              ),
              const SizedBox(width: 8),
              IconButton.filled(
                onPressed: _carregando ? null : () => _enviar(_ctrl.text),
                icon: _carregando
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.send, size: 16),
                style: IconButton.styleFrom(
                  backgroundColor: c.emerald,
                  foregroundColor: Colors.white,
                ),
              ),
            ],
          ),
          if (_erro != null)
            Padding(
              padding: const EdgeInsets.only(top: 6),
              child: Text(_erro!, style: TextStyle(fontSize: 10, color: c.red)),
            ),
        ],
      ),
    );
  }
}
