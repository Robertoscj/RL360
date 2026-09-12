namespace RL360.Shared.Dtos;

/// <summary>Resumo completo da tela principal (Radar de Resultados).</summary>
public sealed class ResumoDashboardDto
{
    public Guid IdEmpresa { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public DateTime GeradoEmUtc { get; set; }

    public CardsTopoDto CardsTopo { get; set; } = new();
    public SnapshotRadarDto Radar { get; set; } = new();
    public PrevisaoResultadoDto PrevisaoResultado { get; set; } = new();
    public FluxoCaixaFuturoDto FluxoCaixaFuturo { get; set; } = new();
    public List<InsightInteligenteDto> Insights { get; set; } = [];
    public List<AlertaCriticoDto> AlertasCriticos { get; set; } = [];
    public RodapeDashboardDto Rodape { get; set; } = new();
}

public sealed class CardsTopoDto
{
    public decimal RiscoProximos30Dias { get; set; }
    public decimal ValorOportunidade { get; set; }
    public int GargalosCriticos { get; set; }
    public int SaudeEmpresaPercentual { get; set; }
    public string StatusSaude { get; set; } = string.Empty;
}

public sealed class PrevisaoResultadoDto
{
    public int Dias { get; set; } = 30;
    public decimal CenarioMaisProvavel { get; set; }
    public decimal Meta { get; set; }
    public decimal PercentualAbaixoMeta { get; set; }
    public List<PontoSerieTemporalDto> Serie { get; set; } = [];
}

public sealed class FluxoCaixaFuturoDto
{
    public int Dias { get; set; } = 60;
    public decimal AReceber { get; set; }
    public decimal EmRisco { get; set; }
    public decimal Atrasado { get; set; }
    public int PercentualSaudavel { get; set; }
    public List<SegmentoDonutDto> Segmentos { get; set; } = [];
}

public sealed class SegmentoDonutDto
{
    public string Rotulo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Cor { get; set; } = string.Empty;
}

public sealed class InsightInteligenteDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string TextoBotao { get; set; } = string.Empty;
    public string RotaAcao { get; set; } = string.Empty;
    public decimal? ImpactoFinanceiro { get; set; }
}

public sealed class AlertaCriticoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Negativo, Positivo
    public decimal ImpactoFinanceiro { get; set; }
    public string Severidade { get; set; } = string.Empty;
}

public sealed class RodapeDashboardDto
{
    public string Mensagem { get; set; } = "Empresas que usam o RL360 têm em média 23% mais lucro.";
    public int QuantidadeAcoesPlano { get; set; }
    public List<ItemPlanoAcaoDto> PlanoAcao { get; set; } = [];
}

public sealed class RequisicaoRegistro
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string NomeEmpresa { get; set; } = string.Empty;
    public string DocumentoEmpresa { get; set; } = string.Empty;
}
