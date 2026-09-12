import 'package:flutter_test/flutter_test.dart';
import 'package:lucro360_mobile/core/demo/dados_demo.dart';

void main() {
  test('dados demo possuem plano de acao', () {
    expect(DadosDemo.planoAcao.length, greaterThan(0));
    expect(DadosDemo.cardsTopo.length, 4);
  });
}
