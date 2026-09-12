namespace RL360.Shared.Dtos;

public sealed class RequisicaoFaturamento
{
    public decimal FaturamentoDia { get; set; }
    public decimal FaturamentoMes { get; set; }
    public decimal MetaMensal { get; set; }
    public decimal CustoMes { get; set; }
    public decimal CustoFixoMes { get; set; }
}

public sealed class RequisicaoVenda
{
    public Guid? IdCliente { get; set; }
    public string Canal { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Margem { get; set; }
    public bool ClienteNovo { get; set; }
}

public sealed class RequisicaoEtapaFunil
{
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorPotencial { get; set; }
    public decimal TaxaConversao { get; set; }
    public decimal TaxaConversaoBase { get; set; }
}

public sealed class RequisicaoInadimplencia
{
    public Guid? IdCliente { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DiasEmAtraso { get; set; }
    public decimal ProbabilidadeRecuperacao { get; set; }
}

public sealed class RequisicaoGargalo
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Area { get; set; } = "Comercial";
    public decimal ImpactoFinanceiro { get; set; }
    public bool Critico { get; set; }
}
