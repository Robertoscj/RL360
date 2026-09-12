double? calcularVariacaoLucroHub(double faturamentoDia, double faturamentoMes) {
  if (faturamentoMes <= 0) return null;
  final mediaDiaria = faturamentoMes / 30;
  if (mediaDiaria <= 0) return null;
  return ((faturamentoDia - mediaDiaria) / mediaDiaria) * 100;
}
