import { useValorAnimado } from '@/hooks/useValorAnimado'
import { formatarMoeda, formatarPercentual } from '@/utils/format'

interface PropsMoeda {
  valor: number
  className?: string
  reiniciarChave?: number
}

interface PropsNumero {
  valor: number
  className?: string
  casas?: number
  sufixo?: string
  reiniciarChave?: number
}

export function MoedaAnimada({ valor, className = '', reiniciarChave }: PropsMoeda) {
  const animado = useValorAnimado(valor, 650, reiniciarChave)
  return <span className={className}>{formatarMoeda(animado)}</span>
}

export function NumeroAnimado({
  valor,
  className = '',
  casas = 0,
  sufixo = '',
  reiniciarChave,
}: PropsNumero) {
  const animado = useValorAnimado(valor, 650, reiniciarChave)
  const texto = casas > 0
    ? animado.toLocaleString('pt-BR', { minimumFractionDigits: casas, maximumFractionDigits: casas })
    : Math.round(animado).toLocaleString('pt-BR')
  return <span className={className}>{texto}{sufixo}</span>
}

export function PercentualAnimado({
  valor,
  className = '',
  casas = 0,
  reiniciarChave,
}: Omit<PropsNumero, 'sufixo'>) {
  const animado = useValorAnimado(valor, 650, reiniciarChave)
  return <span className={className}>{formatarPercentual(animado, casas)}</span>
}
