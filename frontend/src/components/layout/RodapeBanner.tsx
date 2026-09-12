import { useState } from 'react'
import { ChevronRight, Trophy } from 'lucide-react'
import { ModalPlanoAcao } from '@/components/plano-acao/ModalPlanoAcao'
import type { ItemPlanoAcao } from '@/types/dashboard'

interface RodapeBannerProps {
  mensagem: string
  quantidadeAcoes: number
  planoAcao?: ItemPlanoAcao[]
}

export function RodapeBanner({ mensagem, quantidadeAcoes, planoAcao = [] }: RodapeBannerProps) {
  const [modalAberto, setModalAberto] = useState(false)
  const total = planoAcao.length > 0 ? planoAcao.length : quantidadeAcoes

  return (
    <>
      <div
        className="flex flex-col items-center justify-between gap-3 rounded-xl px-5 py-4 sm:flex-row"
        style={{
          background: 'linear-gradient(90deg, #15803d 0%, #16a34a 50%, #22c55e 100%)',
        }}
      >
        <div className="flex items-center gap-3">
          <Trophy className="h-5 w-5 shrink-0 text-white/90" />
          <p className="text-center text-[12px] font-bold uppercase tracking-wide text-white/95 sm:text-left">
            {mensagem.toUpperCase()}
          </p>
        </div>
        <button
          type="button"
          onClick={() => setModalAberto(true)}
          className="inline-flex shrink-0 items-center gap-2 rounded-lg bg-white/20 px-4 py-2.5 text-[12px] font-bold text-white backdrop-blur transition hover:bg-white/30"
        >
          Plano de ação sugerido ({total} ações)
          <ChevronRight className="h-4 w-4" />
        </button>
      </div>

      <ModalPlanoAcao
        aberto={modalAberto}
        onFechar={() => setModalAberto(false)}
        itens={planoAcao}
      />
    </>
  )
}
