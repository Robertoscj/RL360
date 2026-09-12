using RL360.Domain.Common;
using RL360.Domain.Enums;

namespace RL360.Domain.Entities;

/// <summary>Snapshot diário de faturamento e custos.</summary>
public sealed class SnapshotFaturamento : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public DateTime DataReferencia { get; private set; }
    public decimal FaturamentoDia { get; private set; }
    public decimal FaturamentoMes { get; private set; }
    public decimal MetaMensal { get; private set; }
    public decimal CustoMes { get; private set; }
    public decimal CustoFixoMes { get; private set; }

    private SnapshotFaturamento() { }

    public SnapshotFaturamento(Guid idEmpresa, DateTime dataReferencia, decimal faturamentoDia, decimal faturamentoMes,
        decimal metaMensal, decimal custoMes, decimal custoFixoMes)
    {
        IdEmpresa = idEmpresa;
        DataReferencia = dataReferencia;
        FaturamentoDia = faturamentoDia;
        FaturamentoMes = faturamentoMes;
        MetaMensal = metaMensal;
        CustoMes = custoMes;
        CustoFixoMes = custoFixoMes;
    }
}

/// <summary>Registro de inadimplência.</summary>
public sealed class RegistroInadimplencia : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public Guid? IdCliente { get; private set; }
    public string NomeCliente { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public int DiasEmAtraso { get; private set; }
    public decimal ProbabilidadeRecuperacao { get; private set; }

    private RegistroInadimplencia() { }

    public RegistroInadimplencia(Guid idEmpresa, Guid? idCliente, string nomeCliente, decimal valor, int diasEmAtraso, decimal probabilidadeRecuperacao)
    {
        IdEmpresa = idEmpresa;
        IdCliente = idCliente;
        NomeCliente = nomeCliente;
        Valor = valor;
        DiasEmAtraso = diasEmAtraso;
        ProbabilidadeRecuperacao = probabilidadeRecuperacao;
    }

    /// <summary>Valor projetado como perda definitiva caso nada seja feito.</summary>
    public decimal PerdaProjetada => Math.Round(Valor * (1 - ProbabilidadeRecuperacao), 2);
}

/// <summary>Meta comercial/financeira.</summary>
public sealed class Meta : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal ValorMeta { get; private set; }
    public decimal ValorAtual { get; private set; }
    public DateTime InicioPeriodo { get; private set; }
    public DateTime FimPeriodo { get; private set; }

    private Meta() { }

    public Meta(Guid idEmpresa, string nome, decimal valorMeta, decimal valorAtual, DateTime inicioPeriodo, DateTime fimPeriodo)
    {
        IdEmpresa = idEmpresa;
        Nome = nome;
        ValorMeta = valorMeta;
        ValorAtual = valorAtual;
        InicioPeriodo = inicioPeriodo;
        FimPeriodo = fimPeriodo;
    }

    public decimal PercentualAtingido => ValorMeta <= 0 ? 0 : Math.Round(ValorAtual / ValorMeta * 100m, 1);
}

/// <summary>Alerta inteligente.</summary>
public sealed class Alerta : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Mensagem { get; private set; } = string.Empty;
    public SeveridadeAlerta Severidade { get; private set; }
    public decimal ImpactoFinanceiro { get; private set; }
    public FatorRadar? FatorRelacionado { get; private set; }
    public bool Resolvido { get; private set; }

    private Alerta() { }

    public Alerta(Guid idEmpresa, string titulo, string mensagem, SeveridadeAlerta severidade, decimal impactoFinanceiro, FatorRadar? fatorRelacionado)
    {
        IdEmpresa = idEmpresa;
        Titulo = titulo;
        Mensagem = mensagem;
        Severidade = severidade;
        ImpactoFinanceiro = impactoFinanceiro;
        FatorRelacionado = fatorRelacionado;
    }

    public void Resolver()
    {
        Resolvido = true;
        Atualizar();
    }
}
