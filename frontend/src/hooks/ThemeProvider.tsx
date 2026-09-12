import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import {
  CHAVE_TEMA_STORAGE,
  ThemeContext,
  type TemaAplicacao,
  type ThemeContexto,
} from '@/hooks/themeContext'

function carregarTemaSalvo(): TemaAplicacao {
  try {
    const salvo = localStorage.getItem(CHAVE_TEMA_STORAGE)
    if (salvo === 'light' || salvo === 'dark') return salvo
  } catch {
    /* ignora */
  }
  return 'dark'
}

function aplicarTemaDom(tema: TemaAplicacao) {
  document.documentElement.setAttribute('data-theme', tema)
  document.documentElement.style.colorScheme = tema
}

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [tema, setTema] = useState<TemaAplicacao>(() => carregarTemaSalvo())

  useEffect(() => {
    aplicarTemaDom(tema)
    localStorage.setItem(CHAVE_TEMA_STORAGE, tema)
  }, [tema])

  const definirTema = useCallback((proximo: TemaAplicacao) => {
    setTema(proximo)
  }, [])

  const alternarTema = useCallback(() => {
    setTema((atual) => (atual === 'dark' ? 'light' : 'dark'))
  }, [])

  const valor = useMemo<ThemeContexto>(
    () => ({
      tema,
      alternarTema,
      definirTema,
      ehClaro: tema === 'light',
    }),
    [tema, alternarTema, definirTema],
  )

  return <ThemeContext.Provider value={valor}>{children}</ThemeContext.Provider>
}
