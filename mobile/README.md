# LUCRO360 Mobile

App Flutter do **LUCRO360 — Radar de Lucro em Tempo Real**, com layout espelhando o sistema web.

## Estrutura

```
mobile/lib/
├── main.dart                 # Entry point
├── app.dart                  # MaterialApp + rotas (go_router)
├── core/
│   ├── theme/                # Cores e tema claro/escuro (paridade web)
│   ├── demo/                 # Dados estáticos demo (layout)
│   └── constants/            # Itens do menu lateral
├── providers/                # Auth + tema
├── widgets/                  # Cartão vidro, KPI, shell, drawer
└── features/
    ├── auth/                 # Login
    ├── dashboard/            # Radar de lucro
    ├── faturamento/
    ├── vendas/
    ├── inadimplencia/
    ├── alertas/
    └── placeholder/          # Telas em construção
```

## Telas implementadas (layout)

| Tela | Rota | Status |
|------|------|--------|
| Login | `/login` | Layout + auth local demo |
| Dashboard | `/` | KPIs, radar, alertas, métricas, plano de ação |
| Faturamento | `/faturamento` | KPIs + gráficos demo |
| Vendas | `/vendas` | Funil ao vivo |
| Inadimplência | `/inadimplencia` | Aging + contas críticas |
| Alertas | `/alertas` | Lista de alertas |
| Demais menus | várias | Placeholder |

## Como rodar

```bash
cd mobile
flutter pub get
flutter run
```

**Login demo:** `ceo@rl360.com` / `rl360@2026`

### API (próximo passo)

Para conectar à API .NET (porta 5080):

- Android emulador: `http://10.0.2.2:5080`
- iOS simulador: `http://localhost:5080`
- Dispositivo físico: IP da máquina na rede local

Pacotes sugeridos: `dio`, `flutter_secure_storage`, `signalr_netcore`.

## Tema

- **Escuro** (padrão): `#0A0E17` — igual ao web
- **Claro**: `#F1F5F9` — toggle no ícone sol/lua na topbar
- Accent emerald `#22C55E`

## Navegação mobile

- **Drawer** (menu hamburger) = Sidebar do web
- **AppBar** = Topbar (título, refresh, alertas, tema, sair)
