using RL360.Domain.Common;
using RL360.Domain.Enums;

namespace RL360.Domain.Entities;

/// <summary>Cliente da empresa.</summary>
public sealed class Cliente : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal ReceitaMensal { get; private set; }
    public decimal ValorVidaUtil { get; private set; }
    public StatusCliente Status { get; private set; } = StatusCliente.Saudavel;
    public decimal ValorEmAtraso { get; private set; }
    public decimal PotencialExpansao { get; private set; }
    public int PontuacaoSaude { get; private set; } = 100;

    private Cliente() { }

    public Cliente(Guid idEmpresa, string nome, decimal receitaMensal, StatusCliente status,
        decimal valorEmAtraso, decimal potencialExpansao, int pontuacaoSaude, decimal valorVidaUtil)
    {
        IdEmpresa = idEmpresa;
        Nome = nome;
        ReceitaMensal = receitaMensal;
        Status = status;
        ValorEmAtraso = valorEmAtraso;
        PotencialExpansao = potencialExpansao;
        PontuacaoSaude = pontuacaoSaude;
        ValorVidaUtil = valorVidaUtil;
    }
}

/// <summary>Venda registrada (módulo Vendas).</summary>
public sealed class Venda : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public Guid? IdCliente { get; private set; }
    public string Canal { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public decimal Margem { get; private set; }
    public DateTime FechadaEmUtc { get; private set; }
    public bool ClienteNovo { get; private set; }

    private Venda() { }

    public Venda(Guid idEmpresa, Guid? idCliente, string canal, decimal valor, decimal margem, DateTime fechadaEmUtc, bool clienteNovo)
    {
        IdEmpresa = idEmpresa;
        IdCliente = idCliente;
        Canal = canal;
        Valor = valor;
        Margem = margem;
        FechadaEmUtc = fechadaEmUtc;
        ClienteNovo = clienteNovo;
    }
}

/// <summary>Etapa do funil de vendas.</summary>
public sealed class EtapaFunil : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public int Ordem { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorPotencial { get; private set; }
    public decimal TaxaConversao { get; private set; }
    public decimal TaxaConversaoBase { get; private set; }

    private EtapaFunil() { }

    public EtapaFunil(Guid idEmpresa, string nome, int ordem, int quantidade, decimal valorPotencial,
        decimal taxaConversao, decimal taxaConversaoBase)
    {
        IdEmpresa = idEmpresa;
        Nome = nome;
        Ordem = ordem;
        Quantidade = quantidade;
        ValorPotencial = valorPotencial;
        TaxaConversao = taxaConversao;
        TaxaConversaoBase = taxaConversaoBase;
    }
}

/// <summary>Gargalo identificado no negócio.</summary>
public sealed class Gargalo : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public AreaGargalo Area { get; private set; }
    public decimal ImpactoFinanceiro { get; private set; }
    public bool Critico { get; private set; }

    private Gargalo() { }

    public Gargalo(Guid idEmpresa, string titulo, string descricao, AreaGargalo area, decimal impactoFinanceiro, bool critico)
    {
        IdEmpresa = idEmpresa;
        Titulo = titulo;
        Descricao = descricao;
        Area = area;
        ImpactoFinanceiro = impactoFinanceiro;
        Critico = critico;
    }
}

/// <summary>Membro da equipe (módulo Equipe / Produtividade).</summary>
public sealed class MembroEquipe : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Cargo { get; private set; } = string.Empty;
    public decimal ReceitaGerada { get; private set; }
    public decimal Meta { get; private set; }
    public int PontuacaoProdutividade { get; private set; }

    private MembroEquipe() { }

    public MembroEquipe(Guid idEmpresa, string nome, string cargo, decimal receitaGerada, decimal meta, int pontuacaoProdutividade)
    {
        IdEmpresa = idEmpresa;
        Nome = nome;
        Cargo = cargo;
        ReceitaGerada = receitaGerada;
        Meta = meta;
        PontuacaoProdutividade = pontuacaoProdutividade;
    }
}
