import axios from 'axios'
import { corrigirTextos } from '@/utils/textoEncoding'

const baseURL = import.meta.env.VITE_API_URL ?? ''

export const api = axios.create({
  baseURL,
  headers: { 'Content-Type': 'application/json' },
})

let tokenGetter: (() => string | null) | null = null
let refreshHandler: (() => Promise<string | null>) | null = null
let logoutHandler: (() => void) | null = null

export function configurarInterceptorsAuth(
  obterToken: () => string | null,
  renovarToken: () => Promise<string | null>,
  sair: () => void,
) {
  tokenGetter = obterToken
  refreshHandler = renovarToken
  logoutHandler = sair
}

api.interceptors.request.use((config) => {
  const token = tokenGetter?.()
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (res) => {
    if (res.data !== undefined) res.data = corrigirTextos(res.data)
    return res
  },
  async (error) => {
    const original = error.config
    if (error.response?.status === 401 && !original._retry && refreshHandler) {
      original._retry = true
      const novoToken = await refreshHandler()
      if (novoToken) {
        original.headers.Authorization = `Bearer ${novoToken}`
        return api(original)
      }
      logoutHandler?.()
    }
    return Promise.reject(error)
  },
)
