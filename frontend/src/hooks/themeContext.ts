import { createContext } from 'react'

export type TemaAplicacao = 'dark' | 'light'

export interface ThemeContexto {
  tema: TemaAplicacao
  alternarTema: () => void
  definirTema: (tema: TemaAplicacao) => void
  ehClaro: boolean
}

export const CHAVE_TEMA_STORAGE = 'rl360:tema'

export const ThemeContext = createContext<ThemeContexto | null>(null)
