import type { TemaAplicacao } from '@/hooks/themeContext'

export function estilosTooltipGrafico(tema: TemaAplicacao) {
  if (tema === 'light') {
    return {
      background: '#ffffff',
      border: '1px solid #e2e8f0',
      borderRadius: 8,
      fontSize: 11,
      color: '#334155',
      boxShadow: '0 4px 12px rgba(15, 23, 42, 0.08)',
    }
  }
  return {
    background: '#111820',
    border: '1px solid #1e2836',
    borderRadius: 8,
    fontSize: 11,
    color: '#e2e8f0',
  }
}

export function corGridGrafico(tema: TemaAplicacao) {
  return tema === 'light' ? '#e2e8f0' : '#1a2332'
}

export function corTickGrafico(tema: TemaAplicacao) {
  return tema === 'light' ? '#64748b' : '#64748b'
}

export function corHubRadar(tema: TemaAplicacao) {
  return tema === 'light' ? '#f1f5f9' : '#0a1018'
}
