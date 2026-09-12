import 'package:flutter/material.dart';

class ItemMenuRl {
  const ItemMenuRl({
    required this.rota,
    required this.rotulo,
    required this.subtitulo,
    required this.icone,
  });

  final String rota;
  final String rotulo;
  final String subtitulo;
  final IconData icone;
}

const itensMenuRl = [
  ItemMenuRl(
    rota: '/',
    rotulo: 'Radar de Lucro',
    subtitulo: 'Visão em tempo real do que impacta seu lucro',
    icone: Icons.ads_click_outlined,
  ),
  ItemMenuRl(
    rota: '/faturamento',
    rotulo: 'Faturamento',
    subtitulo: 'Em tempo real',
    icone: Icons.shield_outlined,
  ),
  ItemMenuRl(
    rota: '/vendas',
    rotulo: 'Vendas',
    subtitulo: 'Funil ao vivo',
    icone: Icons.filter_alt_outlined,
  ),
  ItemMenuRl(
    rota: '/inadimplencia',
    rotulo: 'Inadimplência',
    subtitulo: 'Risco e previsão',
    icone: Icons.warning_amber_outlined,
  ),
  ItemMenuRl(
    rota: '/gargalos',
    rotulo: 'Gargalos',
    subtitulo: 'Detectar e resolver',
    icone: Icons.account_tree_outlined,
  ),
  ItemMenuRl(
    rota: '/clientes',
    rotulo: 'Clientes',
    subtitulo: 'Base e segmentação',
    icone: Icons.people_outline,
  ),
  ItemMenuRl(
    rota: '/equipe',
    rotulo: 'Equipe',
    subtitulo: 'Performance',
    icone: Icons.groups_outlined,
  ),
  ItemMenuRl(
    rota: '/metas',
    rotulo: 'Metas',
    subtitulo: 'Acompanhar metas',
    icone: Icons.schedule_outlined,
  ),
  ItemMenuRl(
    rota: '/alertas',
    rotulo: 'Alertas',
    subtitulo: 'Central de avisos',
    icone: Icons.notifications_outlined,
  ),
  ItemMenuRl(
    rota: '/relatorios',
    rotulo: 'Relatórios',
    subtitulo: 'Análises e exportações',
    icone: Icons.bar_chart_outlined,
  ),
  ItemMenuRl(
    rota: '/configuracoes',
    rotulo: 'Configurações',
    subtitulo: 'Sistema e usuários',
    icone: Icons.settings_outlined,
  ),
];

ItemMenuRl? itemMenuPorRota(String rota) {
  for (final item in itensMenuRl) {
    if (item.rota == rota) return item;
  }
  return null;
}
