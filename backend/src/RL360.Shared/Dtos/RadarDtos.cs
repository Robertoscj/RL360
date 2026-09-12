namespace RL360.Shared.Dtos;

public sealed class FatorRadarDto
{
    public string Fator { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public decimal ImpactoFinanceiro { get; set; }
    public string Direcao { get; set; } = "positivo";
    public string Descricao { get; set; } = string.Empty;
}

public sealed class ItemPlanoAcaoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Justificativa { get; set; } = string.Empty;
    public decimal ImpactoEsperado { get; set; }
    public string Prioridade { get; set; } = string.Empty;
}

/// <summary>Snapshot completo do Radar de Lucro para o dashboard executivo.</summary>
public sealed class SnapshotRadarDto
{
    public Guid IdEmpresa { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public DateTime GeradoEmUtc { get; set; }

    public decimal LucroAtual { get; set; }
    public decimal LucroEmRisco { get; set; }

    public decimal RiscoProximos30Dias { get; set; }
    public decimal ValorOportunidade { get; set; }
    public int GargalosCriticos { get; set; }
    public int SaudeEmpresaPercentual { get; set; }
    public string StatusSaude { get; set; } = string.Empty;

    public decimal FaturamentoDia { get; set; }
    public decimal FaturamentoMes { get; set; }
    public decimal MetaMensal { get; set; }
    public decimal PercentualMetaAtingida { get; set; }

    public decimal InadimplenciaAtual { get; set; }
    public decimal InadimplenciaProjetada { get; set; }

    public decimal TaxaConversaoAtual { get; set; }
    public decimal QuedaConversao { get; set; }

    public decimal FluxoCaixaFuturo { get; set; }

    public List<FatorRadarDto> Fatores { get; set; } = [];
    public List<ItemPlanoAcaoDto> PlanoAcao { get; set; } = [];
}
