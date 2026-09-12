namespace RL360.Infrastructure.Persistence.Relational;

/// <summary>
/// Diferenças de sintaxe SQL entre provedores relacionais (SQL Server vs PostgreSQL).
/// </summary>
public interface IDialectoSql
{
    string SelecionarLimiteFixo(int quantidade);
    string SelecionarLimiteParametrizado(string nomeParametro);
    string SufixoLimiteFixo(int quantidade);
    string SufixoLimiteParametrizado(string nomeParametro);
    string UtcAgora { get; }
    string UtcMenosDias(int dias);
    string UtcMaisDias(int dias);
    string ColunaOrdemFunil { get; }
    string LiteralVerdadeiro { get; }
    string LiteralFalso { get; }
}

public sealed class DialectoSqlServer : IDialectoSql
{
    public string SelecionarLimiteFixo(int quantidade) => $"TOP {quantidade}";
    public string SelecionarLimiteParametrizado(string nomeParametro) => $"TOP ({nomeParametro})";
    public string SufixoLimiteFixo(int quantidade) => string.Empty;
    public string SufixoLimiteParametrizado(string nomeParametro) => string.Empty;
    public string UtcAgora => "SYSUTCDATETIME()";
    public string UtcMenosDias(int dias) => $"DATEADD(DAY, -{dias}, SYSUTCDATETIME())";
    public string UtcMaisDias(int dias) => $"DATEADD(DAY, {dias}, SYSUTCDATETIME())";
    public string ColunaOrdemFunil => "[Order]";
    public string LiteralVerdadeiro => "1";
    public string LiteralFalso => "0";
}

public sealed class DialectoPostgres : IDialectoSql
{
    public string SelecionarLimiteFixo(int quantidade) => string.Empty;
    public string SelecionarLimiteParametrizado(string nomeParametro) => string.Empty;
    public string SufixoLimiteFixo(int quantidade) => $" LIMIT {quantidade}";
    public string SufixoLimiteParametrizado(string nomeParametro) => $" LIMIT {nomeParametro}";
    public string UtcAgora => "NOW() AT TIME ZONE 'utc'";
    public string UtcMenosDias(int dias) => $"(NOW() AT TIME ZONE 'utc') - INTERVAL '{dias} days'";
    public string UtcMaisDias(int dias) => $"(NOW() AT TIME ZONE 'utc') + INTERVAL '{dias} days'";
    public string ColunaOrdemFunil => "\"order\"";
    public string LiteralVerdadeiro => "TRUE";
    public string LiteralFalso => "FALSE";
}
