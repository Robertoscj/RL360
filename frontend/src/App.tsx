import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider } from '@/hooks/AuthProvider'
import { PeriodoProvider } from '@/hooks/PeriodoProvider'
import { ThemeProvider } from '@/hooks/ThemeProvider'
import { RotaProtegida, RotaPublica } from '@/components/auth/RotaProtegida'
import { AppShell } from '@/components/layout/AppShell'
import { LoginPage } from '@/pages/Login/LoginPage'
import { DashboardPage } from '@/pages/Dashboard/DashboardPage'
import { FaturamentoPage } from '@/pages/Faturamento/FaturamentoPage'
import { VendasPage } from '@/pages/Vendas/VendasPage'
import { InadimplenciaPage } from '@/pages/Inadimplencia/InadimplenciaPage'
import { AlertasPage } from '@/pages/Alertas/AlertasPage'
import { PaginaPlaceholder } from '@/pages/PaginaPlaceholder'

export default function App() {
  return (
    <BrowserRouter>
      <ThemeProvider>
      <AuthProvider>
        <PeriodoProvider>
        <Routes>
          <Route element={<RotaPublica />}>
            <Route path="/login" element={<LoginPage />} />
          </Route>

          <Route element={<RotaProtegida />}>
            <Route element={<AppShell />}>
              <Route index element={<DashboardPage />} />
              <Route path="faturamento" element={<FaturamentoPage />} />
              <Route path="vendas" element={<VendasPage />} />
              <Route path="funil" element={<PaginaPlaceholder titulo="Funil de Vendas" descricao="Etapas, conversão e valor potencial." />} />
              <Route path="inadimplencia" element={<InadimplenciaPage />} />
              <Route path="gargalos" element={<PaginaPlaceholder titulo="Gargalos" descricao="Gargalos operacionais e impacto financeiro." />} />
              <Route path="clientes" element={<PaginaPlaceholder titulo="Clientes" descricao="Base de clientes, LTV e potencial de expansão." />} />
              <Route path="equipe" element={<PaginaPlaceholder titulo="Equipe" descricao="Produtividade e metas por colaborador." />} />
              <Route path="metas" element={<PaginaPlaceholder titulo="Metas" descricao="Objetivos e acompanhamento mensal." />} />
              <Route path="alertas" element={<AlertasPage />} />
              <Route path="relatorios" element={<PaginaPlaceholder titulo="Relatórios" descricao="Exportações e análises detalhadas." />} />
              <Route path="configuracoes" element={<PaginaPlaceholder titulo="Configurações" descricao="Preferências da empresa e da conta." />} />
            </Route>
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
        </PeriodoProvider>
      </AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  )
}
