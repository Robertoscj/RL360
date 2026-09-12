namespace RL360.Shared.Dtos;

public sealed class FaturamentoDto
{
    public decimal FaturamentoDia { get; set; }
    public decimal FaturamentoMes { get; set; }
    public decimal MetaMensal { get; set; }
    public decimal PercentualMetaAtingida { get; set; }
    public decimal CustoMes { get; set; }
    public decimal Lucro { get; set; }
    public int QuantidadeOperacoes { get; set; }
    public decimal TicketMedio { get; set; }
    public List<PontoSerieTemporalDto> SerieDiaria { get; set; } = [];
}

public sealed class PontoSerieTemporalDto
{
    public string Rotulo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal? Meta { get; set; }
}

public sealed class VendaDto
{
    public Guid Id { get; set; }
    public string Canal { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Margem { get; set; }
    public DateTime FechadaEmUtc { get; set; }
    public bool ClienteNovo { get; set; }
    public string? NomeCliente { get; set; }
}

public sealed class EtapaFunilDto
{
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorPotencial { get; set; }
    public decimal TaxaConversao { get; set; }
    public decimal TaxaConversaoBase { get; set; }
}

public sealed class InadimplenciaDto
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DiasEmAtraso { get; set; }
    public decimal ProbabilidadeRecuperacao { get; set; }
    public decimal PerdaProjetada { get; set; }
}

public sealed class GargaloDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public decimal ImpactoFinanceiro { get; set; }
    public bool Critico { get; set; }
}

public sealed class ClienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ReceitaMensal { get; set; }
    public decimal ValorVidaUtil { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorEmAtraso { get; set; }
    public decimal PotencialExpansao { get; set; }
    public int PontuacaoSaude { get; set; }
}

public sealed class MembroEquipeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal ReceitaGerada { get; set; }
    public decimal Meta { get; set; }
    public int PontuacaoProdutividade { get; set; }
}

public sealed class MetaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorMeta { get; set; }
    public decimal ValorAtual { get; set; }
    public decimal PercentualAtingido { get; set; }
    public DateTime InicioPeriodo { get; set; }
    public DateTime FimPeriodo { get; set; }
}

public sealed class AlertaDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Severidade { get; set; } = string.Empty;
    public decimal ImpactoFinanceiro { get; set; }
    public string? FatorRelacionado { get; set; }
    public bool Resolvido { get; set; }
    public DateTime CriadoEmUtc { get; set; }
}
