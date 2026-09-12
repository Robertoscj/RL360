import 'package:flutter/material.dart';
import 'package:lucro360_mobile/core/theme/cores_rl.dart';

ThemeData construirTema({required bool claro}) {
  final c = claro ? CoresRl.light : CoresRl.dark;
  final base = claro ? ThemeData.light() : ThemeData.dark();

  return base.copyWith(
    scaffoldBackgroundColor: c.bg,
    colorScheme: base.colorScheme.copyWith(
      primary: c.emerald,
      surface: c.surface,
      onSurface: c.body,
    ),
    dividerColor: c.border,
    appBarTheme: AppBarTheme(
      backgroundColor: c.bg,
      foregroundColor: c.heading,
      elevation: 0,
      scrolledUnderElevation: 0,
      titleTextStyle: TextStyle(
        fontSize: 14,
        fontWeight: FontWeight.w700,
        letterSpacing: 0.5,
        color: c.heading,
      ),
    ),
    drawerTheme: DrawerThemeData(
      backgroundColor: c.sidebar,
    ),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: c.surface,
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide(color: c.border),
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide(color: c.border),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide(color: c.emerald.withValues(alpha: 0.5)),
      ),
      labelStyle: TextStyle(color: c.muted, fontSize: 12),
      hintStyle: TextStyle(color: c.subtle),
    ),
    elevatedButtonTheme: ElevatedButtonThemeData(
      style: ElevatedButton.styleFrom(
        backgroundColor: c.emerald,
        foregroundColor: Colors.white,
        padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 20),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        textStyle: const TextStyle(fontWeight: FontWeight.w600, fontSize: 14),
      ),
    ),
    textTheme: base.textTheme.apply(
      bodyColor: c.body,
      displayColor: c.heading,
    ),
    extensions: [TemaRlExtension(cores: c)],
  );
}

class TemaRlExtension extends ThemeExtension<TemaRlExtension> {
  const TemaRlExtension({required this.cores});

  final CoresRl cores;

  static TemaRlExtension of(BuildContext context) {
    return Theme.of(context).extension<TemaRlExtension>()!;
  }

  @override
  TemaRlExtension copyWith({CoresRl? cores}) =>
      TemaRlExtension(cores: cores ?? this.cores);

  @override
  TemaRlExtension lerp(ThemeExtension<TemaRlExtension>? other, double t) {
    if (other is! TemaRlExtension) return this;
    return other;
  }
}
