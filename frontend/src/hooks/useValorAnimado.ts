import { useEffect, useRef, useState } from 'react'

export function useValorAnimado(valor: number, duracaoMs = 650, reiniciarChave?: number) {
  const [exibido, setExibido] = useState(valor)
  const referencia = useRef(valor)

  useEffect(() => {
    if (reiniciarChave === undefined) return
    referencia.current = 0
    setExibido(0)
  }, [reiniciarChave])

  useEffect(() => {
    if (referencia.current === valor) return

    const inicio = referencia.current
    const delta = valor - inicio
    const t0 = performance.now()
    let frame = 0

    const animar = (t: number) => {
      const progresso = Math.min(1, (t - t0) / duracaoMs)
      const ease = 1 - (1 - progresso) ** 3
      setExibido(inicio + delta * ease)
      if (progresso < 1) {
        frame = requestAnimationFrame(animar)
      } else {
        referencia.current = valor
      }
    }

    frame = requestAnimationFrame(animar)
    return () => cancelAnimationFrame(frame)
  }, [valor, duracaoMs, reiniciarChave])

  return exibido
}
