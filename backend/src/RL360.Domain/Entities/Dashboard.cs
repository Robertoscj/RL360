using RL360.Domain.Common;
using RL360.Domain.Enums;

namespace RL360.Domain.Entities;

/// <summary>
/// Snapshot desnormalizado do dashboard — lido primeiro do Redis, depois do SQL.
/// Evita consultas pesadas a cada requisição.
/// </summary>
public sealed class SnapshotDashboard : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }

    public decimal FaturamentoDia { get; private set; }
    public decimal FaturamentoMes { get; private set; }
    public decimal LucroAtual { get; private set; }
    public decimal LucroEmRisco { get; private set; }
    public decimal ValorOportunidade { get; private set; }
    public int SaudeEmpresaPercentual { get; private set; }
    public int GargalosCriticos { get; private set; }
    public decimal ValorInadimplencia { get; private set; }
    public decimal TaxaConversao { get; private set; }
    public decimal MetaMes { get; private set; }
    public decimal PercentualMetaMes { get; private set; }

    public decimal PrevisaoResultado30Dias { get; private set; }
    public decimal MetaResultado30Dias { get; private set; }
    public decimal PercentualAbaixoMetaPrevisao { get; private set; }

    public decimal FluxoAReceber60Dias { get; private set; }
    public decimal FluxoEmRisco60Dias { get; private set; }
    public decimal FluxoAtrasado60Dias { get; private set; }
    public int PercentualFluxoSaudavel { get; private set; }

    /// <summary>JSON com radar, insights, alertas e séries de gráficos.</summary>
    public string PayloadJson { get; private set; } = "{}";

    private SnapshotDashboard() { }

    public SnapshotDashboard(
        Guid idEmpresa,
        decimal faturamentoDia, decimal faturamentoMes,
        decimal lucroAtual, decimal lucroEmRisco, decimal valorOportunidade,
        int saudeEmpresaPercentual, int gargalosCriticos,
        decimal valorInadimplencia, decimal taxaConversao,
        decimal metaMes, decimal percentualMetaMes,
        decimal previsaoResultado30Dias, decimal metaResultado30Dias, decimal percentualAbaixoMetaPrevisao,
        decimal fluxoAReceber60Dias, decimal fluxoEmRisco60Dias, decimal fluxoAtrasado60Dias,
        int percentualFluxoSaudavel,
        string payloadJson)
    {
        IdEmpresa = idEmpresa;
        FaturamentoDia = faturamentoDia;
        FaturamentoMes = faturamentoMes;
        LucroAtual = lucroAtual;
        LucroEmRisco = lucroEmRisco;
        ValorOportunidade = valorOportunidade;
        SaudeEmpresaPercentual = saudeEmpresaPercentual;
        GargalosCriticos = gargalosCriticos;
        ValorInadimplencia = valorInadimplencia;
        TaxaConversao = taxaConversao;
        MetaMes = metaMes;
        PercentualMetaMes = percentualMetaMes;
        PrevisaoResultado30Dias = previsaoResultado30Dias;
        MetaResultado30Dias = metaResultado30Dias;
        PercentualAbaixoMetaPrevisao = percentualAbaixoMetaPrevisao;
        FluxoAReceber60Dias = fluxoAReceber60Dias;
        FluxoEmRisco60Dias = fluxoEmRisco60Dias;
        FluxoAtrasado60Dias = fluxoAtrasado60Dias;
        PercentualFluxoSaudavel = percentualFluxoSaudavel;
        PayloadJson = payloadJson;
    }
}

/// <summary>Conta a receber (fluxo de caixa futuro).</summary>
public sealed class ContaReceber : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public Guid? IdCliente { get; private set; }
    public string NomeCliente { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateTime VencimentoEmUtc { get; private set; }
    public string Status { get; private set; } = "AReceber"; // AReceber, EmRisco, Atrasado, Recebido
    public bool Ativo { get; private set; } = true;

    private ContaReceber() { }

    public ContaReceber(Guid idEmpresa, Guid? idCliente, string nomeCliente, decimal valor,
        DateTime vencimentoEmUtc, string status)
    {
        IdEmpresa = idEmpresa;
        IdCliente = idCliente;
        NomeCliente = nomeCliente;
        Valor = valor;
        VencimentoEmUtc = vencimentoEmUtc;
        Status = status;
    }
}

/// <summary>Item persistido do plano de ação sugerido.</summary>
public sealed class ItemPlanoAcaoRegistro : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Justificativa { get; private set; } = string.Empty;
    public decimal ImpactoEsperado { get; private set; }
    public PrioridadeAcao Prioridade { get; private set; }
    public bool Executado { get; private set; }
    public bool Ativo { get; private set; } = true;

    private ItemPlanoAcaoRegistro() { }

    public ItemPlanoAcaoRegistro(Guid idEmpresa, string titulo, string justificativa,
        decimal impactoEsperado, PrioridadeAcao prioridade)
    {
        IdEmpresa = idEmpresa;
        Titulo = titulo;
        Justificativa = justificativa;
        ImpactoEsperado = impactoEsperado;
        Prioridade = prioridade;
    }

    public void MarcarExecutado()
    {
        Executado = true;
        Atualizar();
    }
}

/// <summary>Insight inteligente exibido na coluna lateral do dashboard.</summary>
public sealed class InsightInteligente : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Tipo { get; private set; } = string.Empty; // AcaoUrgente, Gargalo, Oportunidade
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public string TextoBotao { get; private set; } = string.Empty;
    public string RotaAcao { get; private set; } = string.Empty;
    public decimal? ImpactoFinanceiro { get; private set; }
    public bool Ativo { get; private set; } = true;

    private InsightInteligente() { }

    public InsightInteligente(Guid idEmpresa, string tipo, string titulo, string descricao,
        string textoBotao, string rotaAcao, decimal? impactoFinanceiro = null)
    {
        IdEmpresa = idEmpresa;
        Tipo = tipo;
        Titulo = titulo;
        Descricao = descricao;
        TextoBotao = textoBotao;
        RotaAcao = rotaAcao;
        ImpactoFinanceiro = impactoFinanceiro;
    }
}
