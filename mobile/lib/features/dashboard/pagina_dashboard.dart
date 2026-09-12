import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/utils/dashboard_metricas.dart';
import 'package:lucro360_mobile/core/utils/format.dart';
import 'package:lucro360_mobile/core/theme/tema_app.dart';
import 'package:lucro360_mobile/models/dashboard.dart';
import 'package:lucro360_mobile/providers/app_providers.dart';
import 'package:lucro360_mobile/widgets/badge_ao_vivo.dart';
import 'package:lucro360_mobile/widgets/cards_topo_dashboard.dart';
import 'package:lucro360_mobile/widgets/cartao_fluxo_caixa.dart';
import 'package:lucro360_mobile/widgets/cartao_kpi.dart';
import 'package:lucro360_mobile/widgets/cartao_previsao.dart';
import 'package:lucro360_mobile/widgets/cartao_vidro.dart';
import 'package:lucro360_mobile/widgets/metricas_dashboard.dart';
import 'package:lucro360_mobile/widgets/modal_plano_acao.dart';
import 'package:lucro360_mobile/widgets/painel_insights.dart';
import 'package:lucro360_mobile/widgets/radar_hexagonal.dart';
import 'package:lucro360_mobile/widgets/secao_pesada.dart';
import 'package:provider/provider.dart';

class PaginaDashboard extends StatefulWidget {
  const PaginaDashboard({super.key});

  @override
  State<PaginaDashboard> createState() => _PaginaDashboardState();
}

class _PaginaDashboardState extends State<PaginaDashboard> {
  @override
  void initState() {
    super.initState();
    final dash = context.read<DashboardProvider>();
    dash.marcarVisivel(true);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      dash.carregarSeNecessario();
      dash.iniciarAtualizacaoPeriodica();
    });
  }

  @override
  void dispose() {
    context.read<DashboardProvider>().marcarVisivel(false);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Selector<DashboardProvider, _VisaoDashboard>(
      selector: (_, p) => _VisaoDashboard(
        resumo: p.resumo,
        carregando: p.carregando,
        erro: p.erro,
        qtdPlano: p.planoAcao.length,
      ),
      builder: (context, visao, _) {
        final c = TemaRlExtension.of(context).cores;

        if (visao.resumo == null && visao.erro == null) {
          return _EsqueletoDashboard(carregando: visao.carregando);
        }

        if (visao.erro != null && visao.resumo == null) {
          return _ErroDashboard(erro: visao.erro!, cores: c);
        }

        return RefreshIndicator(
          onRefresh: () => context.read<DashboardProvider>().atualizar(),
          child: _DashboardCorpo(
            key: ValueKey(visao.resumo!.geradoEmUtc),
            resumo: visao.resumo!,
            qtdPlano: visao.qtdPlano,
          ),
        );
      },
    );
  }
}

class _VisaoDashboard {
  const _VisaoDashboard({
    required this.resumo,
    required this.carregando,
    required this.erro,
    required this.qtdPlano,
  });

  final ResumoDashboard? resumo;
  final bool carregando;
  final String? erro;
  final int qtdPlano;

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is _VisaoDashboard &&
        other.resumo == resumo &&
        other.carregando == carregando &&
        other.erro == erro &&
        other.qtdPlano == qtdPlano;
  }

  @override
  int get hashCode => Object.hash(resumo, carregando, erro, qtdPlano);
}

class _DashboardCorpo extends StatelessWidget {
  const _DashboardCorpo({
    super.key,
    required this.resumo,
    required this.qtdPlano,
  });

  final ResumoDashboard resumo;
  final int qtdPlano;

  static const _totalSecoes = 9;

  @override
  Widget build(BuildContext context) {
    final variacao = calcularVariacaoLucroHub(
      resumo.radar.faturamentoDia,
      resumo.radar.faturamentoMes,
    );

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      physics: const AlwaysScrollableScrollPhysics(
        parent: BouncingScrollPhysics(),
      ),
      cacheExtent: 600,
      addAutomaticKeepAlives: true,
      itemCount: _totalSecoes,
      itemBuilder: (context, index) {
        return SecaoPesada(
          key: ValueKey('dash-$index-${resumo.geradoEmUtc}'),
          child: Padding(
            padding: EdgeInsets.only(bottom: index == _totalSecoes - 1 ? 8 : 16),
            child: _secao(context, index, variacao),
          ),
        );
      },
    );
  }

  Widget _secao(BuildContext context, int index, double? variacao) {
    final c = TemaRlExtension.of(context).cores;

    switch (index) {
      case 0:
        return Row(
          children: [
            const _BadgeAtualizacao(),
            const SizedBox(width: 8),
            Expanded(
              child: Text(
                'Última atualização: ${formatarHorario(resumo.geradoEmUtc)} · automática a cada 60s',
                style: TextStyle(fontSize: 10, color: c.muted),
              ),
            ),
          ],
        );
      case 1:
        return CardsTopoDashboard(cards: resumo.cardsTopo);
      case 2:
        return CartaoPrevisao(previsao: resumo.previsaoResultado);
      case 3:
        return RadarHexagonal(
          lucroAtual: resumo.radar.lucroAtual,
          fatores: resumo.radar.fatores,
          variacaoPercentual: variacao,
        );
      case 4:
        return CartaoFluxoCaixa(fluxo: resumo.fluxoCaixaFuturo);
      case 5:
        return PainelInsights(insights: resumo.insights);
      case 6:
        return AlertasCriticosRow(alertas: resumo.alertasCriticos);
      case 7:
        return MetricasRadarGrid(radar: resumo.radar);
      case 8:
        return BannerRodape(
          mensagem: resumo.rodape.mensagem,
          quantidadeAcoes: qtdPlano,
          onAbrirPlano: () => mostrarModalPlanoAcao(
            context,
            context.read<DashboardProvider>().planoAcao,
          ),
        );
      default:
        return const SizedBox.shrink();
    }
  }
}

class _BadgeAtualizacao extends StatelessWidget {
  const _BadgeAtualizacao();

  @override
  Widget build(BuildContext context) {
    final atualizando = context.select<DashboardProvider, bool>(
      (p) => p.atualizando,
    );
    return BadgeAoVivo(atualizando: atualizando);
  }
}

class _ErroDashboard extends StatelessWidget {
  const _ErroDashboard({required this.erro, required this.cores});

  final String erro;
  final dynamic cores;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: cores.red.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: cores.red.withValues(alpha: 0.3)),
            ),
            child: Text(erro, style: TextStyle(color: cores.red)),
          ),
          const SizedBox(height: 12),
          ElevatedButton(
            onPressed: () => context.read<DashboardProvider>().carregar(),
            child: const Text('Tentar novamente'),
          ),
        ],
      ),
    );
  }
}

class _EsqueletoDashboard extends StatelessWidget {
  const _EsqueletoDashboard({required this.carregando});

  final bool carregando;

  @override
  Widget build(BuildContext context) {
    final c = TemaRlExtension.of(context).cores;

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: 5,
      itemBuilder: (context, i) {
        if (i == 0 && carregando) {
          return Padding(
            padding: const EdgeInsets.only(bottom: 16),
            child: Align(
              alignment: Alignment.centerLeft,
              child: SizedBox(
                width: 18,
                height: 18,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  color: c.emerald,
                ),
              ),
            ),
          );
        }

        return Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: CartaoVidro(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: 120,
                  height: 10,
                  decoration: BoxDecoration(
                    color: c.border,
                    borderRadius: BorderRadius.circular(4),
                  ),
                ),
                const SizedBox(height: 12),
                Container(
                  width: 160,
                  height: 24,
                  decoration: BoxDecoration(
                    color: c.border.withValues(alpha: 0.7),
                    borderRadius: BorderRadius.circular(6),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
