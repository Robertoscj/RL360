import 'package:flutter/material.dart';

/// Tokens visuais espelhando o frontend web (index.css).
class CoresRl {
  const CoresRl({
    required this.bg,
    required this.surface,
    required this.card,
    required this.cardHover,
    required this.border,
    required this.sidebar,
    required this.heading,
    required this.body,
    required this.muted,
    required this.subtle,
    required this.emerald,
    required this.red,
    required this.amber,
    required this.blue,
  });

  final Color bg;
  final Color surface;
  final Color card;
  final Color cardHover;
  final Color border;
  final Color sidebar;
  final Color heading;
  final Color body;
  final Color muted;
  final Color subtle;
  final Color emerald;
  final Color red;
  final Color amber;
  final Color blue;

  Color get radarHub => bg;

  static const dark = CoresRl(
    bg: Color(0xFF0A0E17),
    surface: Color(0xFF0F1419),
    card: Color(0xFF151B26),
    cardHover: Color(0xFF1A2233),
    border: Color(0xFF1E2836),
    sidebar: Color(0xFF090B0D),
    heading: Color(0xFFF8FAFC),
    body: Color(0xFFE2E8F0),
    muted: Color(0xFF64748B),
    subtle: Color(0xFF475569),
    emerald: Color(0xFF22C55E),
    red: Color(0xFFEF4444),
    amber: Color(0xFFF59E0B),
    blue: Color(0xFF3B82F6),
  );

  static const light = CoresRl(
    bg: Color(0xFFF1F5F9),
    surface: Color(0xFFFFFFFF),
    card: Color(0xFFFFFFFF),
    cardHover: Color(0xFFF8FAFC),
    border: Color(0xFFE2E8F0),
    sidebar: Color(0xFFFFFFFF),
    heading: Color(0xFF0F172A),
    body: Color(0xFF334155),
    muted: Color(0xFF64748B),
    subtle: Color(0xFF94A3B8),
    emerald: Color(0xFF16A34A),
    red: Color(0xFFDC2626),
    amber: Color(0xFFD97706),
    blue: Color(0xFF2563EB),
  );
}
