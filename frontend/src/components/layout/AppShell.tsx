import { Outlet } from 'react-router-dom'
import { Sidebar } from './Sidebar'

export function AppShell() {
  return (
    <div className="min-h-screen bg-rl-bg">
      <Sidebar />
      <div className="pl-[260px]">
        <Outlet />
      </div>
    </div>
  )
}
