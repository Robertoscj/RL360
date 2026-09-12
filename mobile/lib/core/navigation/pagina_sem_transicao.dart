import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

/// Navegação instantânea — sem animação de slide que atrasa a percepção.
CustomTransitionPage<void> paginaSemTransicao({
  required LocalKey key,
  required Widget child,
}) {
  return CustomTransitionPage<void>(
    key: key,
    child: child,
    transitionDuration: Duration.zero,
    reverseTransitionDuration: Duration.zero,
    transitionsBuilder: (_, __, ___, child) => child,
  );
}
