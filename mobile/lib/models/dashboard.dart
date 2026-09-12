import 'package:flutter/material.dart';

class CardsTopo {
  const CardsTopo({
    required this.riscoProximos30Dias,
    required this.valorOportunidade,
    required this.gargalosCriticos,
    required this.saudeEmpresaPercentual,
    required this.statusSaude,
  });

  final double riscoProximos30Dias;
  final double valorOportunidade;
  final int gargalosCriticos;
  final int saudeEmpresaPercentual;
  final String statusSaude;

  factory CardsTopo.fromJson(Map<String, dynamic> json) {
    return CardsTopo(
      riscoProximos30Dias: (json['riscoProximos30Dias'] as num).toDouble(),
      valorOportunidade: (json['valorOportunidade'] as num).toDouble(),
      gargalosCriticos: json['gargalosCriticos'] as int,
      saudeEmpresaPercentual: json['saudeEmpresaPercentual'] as int,
      statusSaude: json['statusSaude'] as String? ?? '',
    );
  }
}

class FatorRadar {
  const FatorRadar({
    required this.fator,
    required this.rotulo,
    required this.impactoFinanceiro,
    required this.direcao,
    required this.descricao,
  });

  final String fator;
  final String rotulo;
  final double impactoFinanceiro;
  final String direcao;
  final String descricao;

  factory FatorRadar.fromJson(Map<String, dynamic> json) {
    return FatorRadar(
      fator: json['fator'] as String? ?? '',
      rotulo: json['rotulo'] as String? ?? '',
      impactoFinanceiro: (json['impactoFinanceiro'] as num?)?.toDouble() ?? 0,
      direcao: json['direcao'] as String? ?? 'negativo',
      descricao: json['descricao'] as String? ?? '',
    );
  }
}

class ItemPlanoAcao {
  const ItemPlanoAcao({
    required this.titulo,
    required this.justificativa,
    required this.impactoEsperado,
    required this.prioridade,
  });

  final String titulo;
  final String justificativa;
  final double impactoEsperado;
  final String prioridade;

  factory ItemPlanoAcao.fromJson(Map<String, dynamic> json) {
    return ItemPlanoAcao(
      titulo: json['titulo'] as String? ?? '',
      justificativa: json['justificativa'] as String? ?? '',
      impactoEsperado: (json['impactoEsperado'] as num?)?.toDouble() ?? 0,
      prioridade: json['prioridade'] as String? ?? '',
    );
  }
}

class SnapshotRadar {
  const SnapshotRadar({
    required this.lucroAtual,
    required this.faturamentoDia,
    required this.faturamentoMes,
    required this.metaMensal,
    required this.percentualMetaAtingida,
    required this.inadimplenciaAtual,
    required this.inadimplenciaProjetada,
    required this.taxaConversaoAtual,
    required this.quedaConversao,
    required this.fluxoCaixaFuturo,
    required this.fatores,
    required this.planoAcao,
  });

  final double lucroAtual;
  final double faturamentoDia;
  final double faturamentoMes;
  final double metaMensal;
  final double percentualMetaAtingida;
  final double inadimplenciaAtual;
  final double inadimplenciaProjetada;
  final double taxaConversaoAtual;
  final double quedaConversao;
  final double fluxoCaixaFuturo;
  final List<FatorRadar> fatores;
  final List<ItemPlanoAcao> planoAcao;

  factory SnapshotRadar.fromJson(Map<String, dynamic> json) {
    return SnapshotRadar(
      lucroAtual: (json['lucroAtual'] as num?)?.toDouble() ?? 0,
      faturamentoDia: (json['faturamentoDia'] as num?)?.toDouble() ?? 0,
      faturamentoMes: (json['faturamentoMes'] as num?)?.toDouble() ?? 0,
      metaMensal: (json['metaMensal'] as num?)?.toDouble() ?? 0,
      percentualMetaAtingida:
          (json['percentualMetaAtingida'] as num?)?.toDouble() ?? 0,
      inadimplenciaAtual: (json['inadimplenciaAtual'] as num?)?.toDouble() ?? 0,
      inadimplenciaProjetada:
          (json['inadimplenciaProjetada'] as num?)?.toDouble() ?? 0,
      taxaConversaoAtual: (json['taxaConversaoAtual'] as num?)?.toDouble() ?? 0,
      quedaConversao: (json['quedaConversao'] as num?)?.toDouble() ?? 0,
      fluxoCaixaFuturo: (json['fluxoCaixaFuturo'] as num?)?.toDouble() ?? 0,
      fatores: (json['fatores'] as List<dynamic>? ?? [])
          .map((e) => FatorRadar.fromJson(e as Map<String, dynamic>))
          .toList(),
      planoAcao: (json['planoAcao'] as List<dynamic>? ?? [])
          .map((e) => ItemPlanoAcao.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}

class PontoSerieTemporal {
  const PontoSerieTemporal({
    required this.rotulo,
    required this.valor,
    this.meta,
  });

  final String rotulo;
  final double valor;
  final double? meta;

  factory PontoSerieTemporal.fromJson(Map<String, dynamic> json) {
    return PontoSerieTemporal(
      rotulo: json['rotulo'] as String? ?? '',
      valor: (json['valor'] as num?)?.toDouble() ?? 0,
      meta: (json['meta'] as num?)?.toDouble(),
    );
  }
}

class PrevisaoResultado {
  const PrevisaoResultado({
    required this.dias,
    required this.cenarioMaisProvavel,
    required this.meta,
    required this.percentualAbaixoMeta,
    required this.serie,
  });

  final int dias;
  final double cenarioMaisProvavel;
  final double meta;
  final double percentualAbaixoMeta;
  final List<PontoSerieTemporal> serie;

  factory PrevisaoResultado.fromJson(Map<String, dynamic> json) {
    return PrevisaoResultado(
      dias: json['dias'] as int? ?? 30,
      cenarioMaisProvavel:
          (json['cenarioMaisProvavel'] as num?)?.toDouble() ?? 0,
      meta: (json['meta'] as num?)?.toDouble() ?? 0,
      percentualAbaixoMeta:
          (json['percentualAbaixoMeta'] as num?)?.toDouble() ?? 0,
      serie: (json['serie'] as List<dynamic>? ?? [])
          .map((e) => PontoSerieTemporal.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}

class SegmentoDonut {
  const SegmentoDonut({
    required this.rotulo,
    required this.valor,
    required this.cor,
  });

  final String rotulo;
  final double valor;
  final String cor;

  factory SegmentoDonut.fromJson(Map<String, dynamic> json) {
    return SegmentoDonut(
      rotulo: json['rotulo'] as String? ?? '',
      valor: (json['valor'] as num?)?.toDouble() ?? 0,
      cor: json['cor'] as String? ?? '#22c55e',
    );
  }
}

class FluxoCaixaFuturo {
  const FluxoCaixaFuturo({
    required this.dias,
    required this.aReceber,
    required this.emRisco,
    required this.atrasado,
    required this.percentualSaudavel,
    required this.segmentos,
  });

  final int dias;
  final double aReceber;
  final double emRisco;
  final double atrasado;
  final int percentualSaudavel;
  final List<SegmentoDonut> segmentos;

  factory FluxoCaixaFuturo.fromJson(Map<String, dynamic> json) {
    return FluxoCaixaFuturo(
      dias: json['dias'] as int? ?? 60,
      aReceber: (json['aReceber'] as num?)?.toDouble() ?? 0,
      emRisco: (json['emRisco'] as num?)?.toDouble() ?? 0,
      atrasado: (json['atrasado'] as num?)?.toDouble() ?? 0,
      percentualSaudavel: json['percentualSaudavel'] as int? ?? 0,
      segmentos: (json['segmentos'] as List<dynamic>? ?? [])
          .map((e) => SegmentoDonut.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}

class InsightInteligente {
  const InsightInteligente({
    required this.id,
    required this.tipo,
    required this.titulo,
    required this.descricao,
    required this.textoBotao,
    required this.rotaAcao,
    this.impactoFinanceiro,
  });

  final String id;
  final String tipo;
  final String titulo;
  final String descricao;
  final String textoBotao;
  final String rotaAcao;
  final double? impactoFinanceiro;

  factory InsightInteligente.fromJson(Map<String, dynamic> json) {
    return InsightInteligente(
      id: json['id'] as String,
      tipo: json['tipo'] as String? ?? '',
      titulo: json['titulo'] as String? ?? '',
      descricao: json['descricao'] as String? ?? '',
      textoBotao: json['textoBotao'] as String? ?? '',
      rotaAcao: json['rotaAcao'] as String? ?? '',
      impactoFinanceiro: (json['impactoFinanceiro'] as num?)?.toDouble(),
    );
  }
}

class AlertaCritico {
  const AlertaCritico({
    required this.id,
    required this.titulo,
    required this.tipo,
    required this.impactoFinanceiro,
    required this.severidade,
  });

  final String id;
  final String titulo;
  final String tipo;
  final double impactoFinanceiro;
  final String severidade;

  factory AlertaCritico.fromJson(Map<String, dynamic> json) {
    return AlertaCritico(
      id: json['id'] as String,
      titulo: json['titulo'] as String? ?? '',
      tipo: json['tipo'] as String? ?? '',
      impactoFinanceiro: (json['impactoFinanceiro'] as num?)?.toDouble() ?? 0,
      severidade: json['severidade'] as String? ?? '',
    );
  }
}

class RodapeDashboard {
  const RodapeDashboard({
    required this.mensagem,
    required this.quantidadeAcoesPlano,
    required this.planoAcao,
  });

  final String mensagem;
  final int quantidadeAcoesPlano;
  final List<ItemPlanoAcao> planoAcao;

  factory RodapeDashboard.fromJson(Map<String, dynamic> json) {
    return RodapeDashboard(
      mensagem: json['mensagem'] as String? ?? '',
      quantidadeAcoesPlano: json['quantidadeAcoesPlano'] as int? ?? 0,
      planoAcao: (json['planoAcao'] as List<dynamic>? ?? [])
          .map((e) => ItemPlanoAcao.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}

class ResumoDashboard {
  const ResumoDashboard({
    required this.geradoEmUtc,
    required this.cardsTopo,
    required this.radar,
    required this.previsaoResultado,
    required this.fluxoCaixaFuturo,
    required this.insights,
    required this.alertasCriticos,
    required this.rodape,
  });

  final DateTime geradoEmUtc;
  final CardsTopo cardsTopo;
  final SnapshotRadar radar;
  final PrevisaoResultado previsaoResultado;
  final FluxoCaixaFuturo fluxoCaixaFuturo;
  final List<InsightInteligente> insights;
  final List<AlertaCritico> alertasCriticos;
  final RodapeDashboard rodape;

  factory ResumoDashboard.fromJson(Map<String, dynamic> json) {
    return ResumoDashboard(
      geradoEmUtc: DateTime.parse(json['geradoEmUtc'] as String),
      cardsTopo: CardsTopo.fromJson(json['cardsTopo'] as Map<String, dynamic>),
      radar: SnapshotRadar.fromJson(json['radar'] as Map<String, dynamic>),
      previsaoResultado: PrevisaoResultado.fromJson(
        json['previsaoResultado'] as Map<String, dynamic>,
      ),
      fluxoCaixaFuturo: FluxoCaixaFuturo.fromJson(
        json['fluxoCaixaFuturo'] as Map<String, dynamic>,
      ),
      insights: (json['insights'] as List<dynamic>? ?? [])
          .map((e) => InsightInteligente.fromJson(e as Map<String, dynamic>))
          .toList(),
      alertasCriticos: (json['alertasCriticos'] as List<dynamic>? ?? [])
          .map((e) => AlertaCritico.fromJson(e as Map<String, dynamic>))
          .toList(),
      rodape: RodapeDashboard.fromJson(json['rodape'] as Map<String, dynamic>),
    );
  }
}

Color parseCorHex(String hex) {
  var h = hex.replaceFirst('#', '');
  if (h.length == 6) h = 'FF$h';
  return Color(int.parse(h, radix: 16));
}
