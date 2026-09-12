namespace RL360.Domain.Enums;

/// <summary>
/// Os seis fatores estruturais ao redor do lucro no Radar de Lucro.
/// </summary>
public enum FatorRadar
{
    Inadimplencia = 0,
    Conversao = 1,
    VendasNovas = 2,
    Produtividade = 3,
    ExpansaoClientes = 4,
    GargaloComercial = 5
}

public enum StatusSaude
{
    Critico = 0,
    Atencao = 1,
    Saudavel = 2
}

public enum SeveridadeAlerta
{
    Info = 0,
    Atencao = 1,
    Critico = 2
}

public enum NivelRisco
{
    Baixo = 0,
    Medio = 1,
    Alto = 2,
    Critico = 3
}

public enum AreaGargalo
{
    Comercial = 0,
    Financeiro = 1,
    Operacional = 2,
    Atendimento = 3,
    Logistica = 4
}

public enum StatusCliente
{
    Saudavel = 0,
    EmRisco = 1,
    OportunidadeExpansao = 2,
    Inadimplente = 3
}

public enum PerfilUsuario
{
    Proprietario = 0,
    Admin = 1,
    Gerente = 2,
    Analista = 3
}

public enum PrioridadeAcao
{
    Baixa = 0,
    Media = 1,
    Alta = 2,
    Urgente = 3
}
