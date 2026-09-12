import 'package:intl/intl.dart';

final _moeda = NumberFormat.currency(
  locale: 'pt_BR',
  symbol: 'R\$',
  decimalDigits: 0,
);

final _moedaCompacta = NumberFormat.compactCurrency(
  locale: 'pt_BR',
  symbol: 'R\$',
  decimalDigits: 1,
);

String formatarMoeda(num valor) => _moeda.format(valor);

String formatarMoedaCompacta(num valor) {
  if (valor.abs() >= 1000000) return _moedaCompacta.format(valor);
  return formatarMoeda(valor);
}

String formatarMoedaComSinal(num valor) {
  final sinal = valor >= 0 ? '+' : '-';
  return '$sinal${formatarMoeda(valor.abs())}';
}

String formatarPercentual(num valor, {int casas = 0}) {
  return '${valor.toStringAsFixed(casas).replaceAll('.', ',')}%';
}

String formatarHorario(DateTime dt) {
  return DateFormat('HH:mm:ss').format(dt.toLocal());
}
