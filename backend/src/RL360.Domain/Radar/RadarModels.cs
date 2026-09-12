using RL360.Domain.Enums;

namespace RL360.Domain.Radar;

/// <summary>
/// Entradas agregadas necessárias para calcular o Radar de Lucro de uma empresa.
/// </summary>
public sealed class EntradasRadar
{
    public Guid IdEmpresa { get; init; }
    public string NomeEmpresa { get; init; } = string.Empty;

    public decimal FaturamentoDia { get; init; }
    public decimal FaturamentoMes { get; init; }
    public decimal MetaMensal { get; init; }
    public decimal CustoMes { get; init; }

    public decimal InadimplenciaAtual { get; init; }
    public decimal PerdaProjetadaInadimplencia { get; init; }

    public decimal TaxaConversaoAtual { get; init; }
    public decimal TaxaConversaoBase { get; init; }
    public decimal ValorPipeline { get; init; }
    public decimal PipelineProntoFechar { get; init; }

    public decimal MargemProjetadaVendasNovas { get; init; }
    public decimal ImpactoProdutividade { get; init; }
    public decimal PotencialExpansaoPonderado { get; init; }
    public decimal ImpactoGargaloComercial { get; init; }
    public decimal PerdaProjetadaClientesEmRisco { get; init; }

    public int QuantidadeGargalosCriticos { get; init; }
    public int QuantidadeAlertasCriticos { get; init; }
}

public sealed record ResultadoFatorRadar(
    FatorRadar Fator,
    string Rotulo,
    decimal ImpactoFinanceiro,
    string Direcao,
    string Descricao);

public sealed record ItemPlanoAcao(
    string Titulo,
    string Justificativa,
    decimal ImpactoEsperado,
    PrioridadeAcao Prioridade);

/// <summary>
/// Snapshot completo do Radar de Lucro — consumido pelo dashboard, SignalR e worker.
/// </summary>
public sealed class SnapshotRadar
{
    public Guid IdEmpresa { get; init; }
    public string NomeEmpresa { get; init; } = string.Empty;
    public DateTime GeradoEmUtc { get; init; } = DateTime.UtcNow;

    public decimal LucroAtual { get; init; }
    public decimal LucroEmRisco { get; init; }

    public decimal RiscoProximos30Dias { get; init; }
    public decimal ValorOportunidade { get; init; }
    public int GargalosCriticos { get; init; }
    public int SaudeEmpresaPercentual { get; init; }
    public StatusSaude StatusSaude { get; init; }

    public decimal FaturamentoDia { get; init; }
    public decimal FaturamentoMes { get; init; }
    public decimal MetaMensal { get; init; }
    public decimal PercentualMetaAtingida { get; init; }

    public decimal InadimplenciaAtual { get; init; }
    public decimal InadimplenciaProjetada { get; init; }

    public decimal TaxaConversaoAtual { get; init; }
    public decimal QuedaConversao { get; init; }

    public decimal FluxoCaixaFuturo { get; init; }

    public IReadOnlyList<ResultadoFatorRadar> Fatores { get; init; } = [];
    public IReadOnlyList<ItemPlanoAcao> PlanoAcao { get; init; } = [];
}
