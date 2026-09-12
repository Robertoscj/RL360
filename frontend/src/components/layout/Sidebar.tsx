import { NavLink } from 'react-router-dom'
import {
  Bell,
  ChevronRight,
  Clock,
  FileBarChart,
  Filter,
  MousePointerClick,
  Settings,
  Shield,
  ShieldAlert,
  Target,
  Users,
  UsersRound,
  Workflow,
} from 'lucide-react'
import type { LucideIcon } from 'lucide-react'

const itens: { to: string; rotulo: string; subtitulo: string; icone: LucideIcon }[] = [
  { to: '/', rotulo: 'Radar de Lucro', subtitulo: 'Visão geral', icone: MousePointerClick },
  { to: '/faturamento', rotulo: 'Faturamento', subtitulo: 'Em tempo real', icone: Shield },
  { to: '/vendas', rotulo: 'Vendas', subtitulo: 'Funil ao vivo', icone: Filter },
  { to: '/inadimplencia', rotulo: 'Inadimplência', subtitulo: 'Risco e previsão', icone: ShieldAlert },
  { to: '/gargalos', rotulo: 'Gargalos', subtitulo: 'Detectar e resolver', icone: Workflow },
  { to: '/clientes', rotulo: 'Clientes', subtitulo: 'Base e segmentação', icone: Users },
  { to: '/equipe', rotulo: 'Equipe', subtitulo: 'Performance', icone: UsersRound },
  { to: '/metas', rotulo: 'Metas', subtitulo: 'Acompanhar metas', icone: Clock },
  { to: '/alertas', rotulo: 'Alertas', subtitulo: 'Central de avisos', icone: Bell },
  { to: '/relatorios', rotulo: 'Relatórios', subtitulo: 'Análises e exportações', icone: FileBarChart },
  { to: '/configuracoes', rotulo: 'Configurações', subtitulo: 'Sistema e usuários', icone: Settings },
]

export function Sidebar() {
  return (
    <aside className="fixed inset-y-0 left-0 z-30 flex w-[260px] flex-col border-r border-rl-border bg-rl-sidebar">
      {/* Logo */}
      <div className="px-5 pb-2 pt-6">
        <div className="flex items-start gap-3">
          <div
            className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full"
            style={{
              background: 'radial-gradient(circle, rgba(34,197,94,0.25) 0%, rgba(34,197,94,0.08) 70%)',
              boxShadow: '0 0 12px rgba(34,197,94,0.15)',
            }}
          >
            <Target className="h-[18px] w-[18px] text-emerald-500" strokeWidth={2.2} />
          </div>
          <div className="min-w-0 pt-0.5">
            <p className="text-[15px] font-black uppercase tracking-wide text-rl-heading">LUCRO360</p>
            <p className="mt-0.5 text-[11px] font-medium leading-snug text-rl-body">
              Seu radar de lucro em tempo real
            </p>
          </div>
        </div>
      </div>

      {/* Nav */}
      <nav className="flex-1 overflow-y-auto px-3 py-3">
        {itens.map(({ to, rotulo, subtitulo, icone: Icone }) => (
          <NavLink
            key={to}
            to={to}
            end={to === '/'}
            className={({ isActive }) =>
              `nav-item group ${isActive ? 'nav-item-active' : ''}`
            }
          >
            {({ isActive }) => (
              <>
                <div
                  className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-lg transition-colors ${
                    isActive ? 'bg-emerald-500/15' : 'bg-transparent'
                  }`}
                >
                  <Icone
                    className={`h-[17px] w-[17px] ${isActive ? 'text-emerald-500' : 'text-rl-muted'}`}
                    strokeWidth={isActive ? 2 : 1.75}
                  />
                </div>
                <div className="min-w-0 flex-1">
                  <p
                    className={`truncate text-[13px] font-semibold leading-tight ${
                      isActive ? 'text-rl-heading' : 'text-rl-body'
                    }`}
                  >
                    {rotulo}
                  </p>
                  <p
                    className={`truncate text-[11px] font-medium leading-tight ${
                      isActive ? 'text-emerald-600' : 'text-rl-body'
                    }`}
                  >
                    {subtitulo}
                  </p>
                </div>
                {isActive && (
                  <ChevronRight className="h-3.5 w-3.5 shrink-0 text-emerald-500" strokeWidth={2.5} />
                )}
              </>
            )}
          </NavLink>
        ))}
      </nav>

      {/* Suporte */}
      <div className="p-4 pt-2">
        <div className="rounded-xl border border-rl-border bg-rl-sidebar-panel p-4">
          <p className="text-[12px] font-semibold text-rl-heading">Precisa de ajuda?</p>
          <p className="mt-0.5 text-[11px] font-medium text-rl-body">Fale com nosso suporte</p>
          <button
            type="button"
            className="mt-3 w-full rounded-lg border border-emerald-500/35 bg-emerald-500/[0.06] py-2.5 text-[11px] font-bold text-emerald-600 transition hover:border-emerald-500/50 hover:bg-emerald-500/10"
          >
            Abrir chamado
          </button>
        </div>
      </div>
    </aside>
  )
}
