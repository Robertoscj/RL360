import 'package:flutter/material.dart';

/// Isola repaint e mantém seções já construídas ao rolar de volta.
class SecaoPesada extends StatefulWidget {
  const SecaoPesada({super.key, required this.child});

  final Widget child;

  @override
  State<SecaoPesada> createState() => _SecaoPesadaState();
}

class _SecaoPesadaState extends State<SecaoPesada>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  @override
  Widget build(BuildContext context) {
    super.build(context);
    return RepaintBoundary(child: widget.child);
  }
}
