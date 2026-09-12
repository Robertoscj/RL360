/// Dados estáticos para layout mobile (paridade com modo demo web).
class DadosDemo {
  static const usuario = 'ceo@rl360.com';
  static const senha = 'rl360@2026';
  static const nomeUsuario = 'Carlos Mendes';
  static const perfilUsuario = 'Diretor Executivo';

  static const cardsTopo = [
    KpiDemo(
      titulo: 'ATENÇÃO: RISCO DE PERDA',
      valor: 'R\$ 487.320',
      subtitulo: 'Principais ameaças nos próximos 30 dias',
      tipoCor: 'red',
    ),
    KpiDemo(
      titulo: 'OPORTUNIDADE IDENTIFICADA',
      valor: 'R\$ 318.000',
      subtitulo: 'Potencial com ações recomendadas',
      tipoCor: 'green',
    ),
    KpiDemo(
      titulo: 'GARGALOS CRÍTICOS',
      valor: '3',
      subtitulo: 'Pontos que reduzem sua margem',
      tipoCor: 'amber',
    ),
    KpiDemo(
      titulo: 'SAÚDE DA EMPRESA',
      valor: '68%',
      subtitulo: 'Status: Atenção',
      tipoCor: 'blue',
    ),
  ];

  static const metricasSparkline = [
    ('Faturamento Hoje', 'R\$ 48.200', '+12%'),
    ('Faturamento Mês', 'R\$ 1,25 mi', '+8%'),
    ('Ticket Médio', 'R\$ 3.840', '+5%'),
    ('Novos Clientes', '27', '+15%'),
    ('Conversão Geral', '22%', '-3,2pp'),
    ('Meta do Mês', '83%', '-17pp'),
  ];

  static const alertasCriticos = [
    AlertaDemo('Inadimplência acima do normal', 'R\$ -183.000', 'Critico'),
    AlertaDemo('Aprovação muito lenta', 'R\$ -92.000', 'Critico'),
    AlertaDemo('Queda na conversão', 'R\$ -152.000', 'Critico'),
    AlertaDemo('Oportunidade na região Sul', 'R\$ +126.000', 'Info'),
  ];

  static const planoAcao = [
    PlanoDemo(
      titulo: 'Acionar régua de cobrança nos maiores inadimplentes',
      justificativa: 'Recuperar parte dos R\$ 183.000 em risco.',
      impacto: 'R\$ 73.200',
      prioridade: 'Urgente',
    ),
    PlanoDemo(
      titulo: 'Revisar etapas do funil com maior queda',
      justificativa: 'Recuperar conversão e destravar pipeline.',
      impacto: 'R\$ 53.200',
      prioridade: 'Alta',
    ),
    PlanoDemo(
      titulo: 'Campanha cross-sell Produto A → B',
      justificativa: 'Aproveitar correlação de 70%.',
      impacto: 'R\$ 130.200',
      prioridade: 'Média',
    ),
  ];

  static const faturamentoKpis = [
    ('Faturamento Hoje', 'R\$ 48.200', '+12%'),
    ('Faturamento Mês', 'R\$ 1.250.000', '83% meta'),
    ('Meta Mensal', 'R\$ 1.500.000', 'Faltam R\$ 250k'),
    ('Lucro Estimado', 'R\$ 312.000', '25% margem'),
  ];
}

class KpiDemo {
  const KpiDemo({
    required this.titulo,
    required this.valor,
    required this.subtitulo,
    required this.tipoCor,
  });
  final String titulo;
  final String valor;
  final String subtitulo;
  final String tipoCor;
}

class AlertaDemo {
  const AlertaDemo(this.titulo, this.impacto, this.severidade);
  final String titulo;
  final String impacto;
  final String severidade;
}

class PlanoDemo {
  const PlanoDemo({
    required this.titulo,
    required this.justificativa,
    required this.impacto,
    required this.prioridade,
  });
  final String titulo;
  final String justificativa;
  final String impacto;
  final String prioridade;
}
