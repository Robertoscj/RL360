using Dapper;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;

namespace RL360.Infrastructure.Persistence.Relational;

public sealed class EmpresaRepositorio(IFabricaConexaoBanco fabrica) : IEmpresaRepositorio
{
    public async Task<Empresa?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return await db.QuerySingleOrDefaultAsync<Empresa>(
            @"SELECT Id, Name AS Nome, Document AS Documento, Segment AS Segmento,
                     PlanCode AS CodigoPlano, IsActive AS Ativo, CreatedAtUtc AS CriadoEmUtc
              FROM Tenants WHERE Id = @id", new { id });
    }

    public async Task<IReadOnlyList<Empresa>> ObterTodosAsync(CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<Empresa>(
            @"SELECT Id, Name AS Nome, Document AS Documento, Segment AS Segmento,
                     PlanCode AS CodigoPlano, IsActive AS Ativo, CreatedAtUtc AS CriadoEmUtc
              FROM Tenants")).ToList();
    }

    public async Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Tenants WHERE Document = @documento", new { documento = documento.Trim() }) > 0;
    }

    public async Task<Empresa> CriarAsync(Empresa empresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO Tenants (Id, Name, Document, Segment, PlanCode, IsActive, CreatedAtUtc)
            VALUES (@Id, @Nome, @Documento, @Segmento, @CodigoPlano, @Ativo, @CriadoEmUtc)", empresa);
        return empresa;
    }
}

public sealed class UsuarioRepositorio(IFabricaConexaoBanco fabrica) : IUsuarioRepositorio
{
    private const string Colunas = @"Id, TenantId AS IdEmpresa, Name AS Nome, Email,
        PasswordHash AS HashSenha, Role AS Perfil, IsActive AS Ativo, CreatedAtUtc AS CriadoEmUtc";

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return await db.QuerySingleOrDefaultAsync<Usuario>(
            $"SELECT {Colunas} FROM Users WHERE LOWER(Email) = @email", new { email = email.Trim().ToLowerInvariant() });
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return await db.QuerySingleOrDefaultAsync<Usuario>($"SELECT {Colunas} FROM Users WHERE Id = @id", new { id });
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Users WHERE LOWER(Email) = @email", new { email = email.Trim().ToLowerInvariant() }) > 0;
    }

    public async Task<Usuario> CriarAsync(Usuario usuario, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO Users (Id, TenantId, Name, Email, PasswordHash, Role, IsActive, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @Nome, @Email, @HashSenha, @Perfil, @Ativo, @CriadoEmUtc)", usuario);
        return usuario;
    }
}

public sealed class FaturamentoRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IFaturamentoRepositorio
{
    private const string Colunas = @"Id, TenantId AS IdEmpresa, ReferenceDate AS DataReferencia,
        RevenueDay AS FaturamentoDia, RevenueMonth AS FaturamentoMes, MonthlyTarget AS MetaMensal,
        CostMonth AS CustoMes, FixedCostMonth AS CustoFixoMes, CreatedAtUtc AS CriadoEmUtc";

    public async Task<SnapshotFaturamento?> ObterMaisRecenteAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        var limite = dialecto.SelecionarLimiteFixo(1);
        var sufixo = dialecto.SufixoLimiteFixo(1);
        return await db.QuerySingleOrDefaultAsync<SnapshotFaturamento>(
            $"SELECT {limite} {Colunas} FROM RevenueSnapshots WHERE TenantId = @idEmpresa ORDER BY ReferenceDate DESC{sufixo}",
            new { idEmpresa });
    }

    public async Task<IReadOnlyList<SnapshotFaturamento>> ObterSerieDiariaAsync(Guid idEmpresa, int dias, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        var limite = dialecto.SelecionarLimiteParametrizado("@dias");
        var sufixo = dialecto.SufixoLimiteParametrizado("@dias");
        var linhas = await db.QueryAsync<SnapshotFaturamento>(
            $"SELECT {limite} {Colunas} FROM RevenueSnapshots WHERE TenantId = @idEmpresa ORDER BY ReferenceDate DESC{sufixo}",
            new { idEmpresa, dias });
        return linhas.OrderBy(r => r.DataReferencia).ToList();
    }

    public async Task<SnapshotFaturamento> InserirAsync(SnapshotFaturamento snapshot, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO RevenueSnapshots (Id, TenantId, ReferenceDate, RevenueDay, RevenueMonth,
                MonthlyTarget, CostMonth, FixedCostMonth, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @DataReferencia, @FaturamentoDia, @FaturamentoMes,
                @MetaMensal, @CustoMes, @CustoFixoMes, @CriadoEmUtc)", snapshot);
        return snapshot;
    }
}

public sealed class VendaRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IVendaRepositorio
{
    public async Task<IReadOnlyList<Venda>> ObterMesAtualAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<Venda>(
            $@"SELECT Id, TenantId AS IdEmpresa, ClientId AS IdCliente, Channel AS Canal,
                     Amount AS Valor, Margin AS Margem, ClosedAtUtc AS FechadaEmUtc,
                     IsNewClient AS ClienteNovo, CreatedAtUtc AS CriadoEmUtc
              FROM Sales WHERE TenantId = @idEmpresa AND ClosedAtUtc >= {dialecto.UtcMenosDias(31)}",
            new { idEmpresa })).ToList();
    }

    public async Task<Venda> InserirAsync(Venda venda, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO Sales (Id, TenantId, ClientId, Channel, Amount, Margin, ClosedAtUtc, IsNewClient, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @IdCliente, @Canal, @Valor, @Margem, @FechadaEmUtc, @ClienteNovo, @CriadoEmUtc)", venda);
        return venda;
    }
}

public sealed class FunilRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IFunilRepositorio
{
    public async Task<IReadOnlyList<EtapaFunil>> ObterEtapasAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        var ordem = dialecto.ColunaOrdemFunil;
        return (await db.QueryAsync<EtapaFunil>(
            $@"SELECT Id, TenantId AS IdEmpresa, Name AS Nome, {ordem} AS Ordem, Count AS Quantidade,
                     PotentialValue AS ValorPotencial, ConversionRate AS TaxaConversao,
                     BaselineConversionRate AS TaxaConversaoBase, CreatedAtUtc AS CriadoEmUtc
              FROM FunnelStages WHERE TenantId = @idEmpresa ORDER BY {ordem}", new { idEmpresa })).ToList();
    }

    public async Task<EtapaFunil> InserirOuAtualizarEtapaAsync(EtapaFunil etapa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        var ordem = dialecto.ColunaOrdemFunil;
        await db.ExecuteAsync(
            $"DELETE FROM FunnelStages WHERE TenantId = @IdEmpresa AND {ordem} = @Ordem", etapa);
        await db.ExecuteAsync($@"
            INSERT INTO FunnelStages (Id, TenantId, Name, {ordem}, Count, PotentialValue, ConversionRate, BaselineConversionRate, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @Nome, @Ordem, @Quantidade, @ValorPotencial, @TaxaConversao, @TaxaConversaoBase, @CriadoEmUtc)", etapa);
        return etapa;
    }
}

public sealed class InadimplenciaRepositorio(IFabricaConexaoBanco fabrica) : IInadimplenciaRepositorio
{
    public async Task<IReadOnlyList<RegistroInadimplencia>> ObterAbertasAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<RegistroInadimplencia>(
            @"SELECT Id, TenantId AS IdEmpresa, ClientId AS IdCliente, ClientName AS NomeCliente,
                     Amount AS Valor, DaysOverdue AS DiasEmAtraso, RecoveryProbability AS ProbabilidadeRecuperacao,
                     CreatedAtUtc AS CriadoEmUtc
              FROM DelinquencyRecords WHERE TenantId = @idEmpresa", new { idEmpresa })).ToList();
    }

    public async Task<RegistroInadimplencia> InserirAsync(RegistroInadimplencia registro, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO DelinquencyRecords (Id, TenantId, ClientId, ClientName, Amount, DaysOverdue, RecoveryProbability, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @IdCliente, @NomeCliente, @Valor, @DiasEmAtraso, @ProbabilidadeRecuperacao, @CriadoEmUtc)", registro);
        return registro;
    }
}

public sealed class GargaloRepositorio(IFabricaConexaoBanco fabrica) : IGargaloRepositorio
{
    public async Task<IReadOnlyList<Gargalo>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<Gargalo>(
            @"SELECT Id, TenantId AS IdEmpresa, Title AS Titulo, Description AS Descricao, Area,
                     FinancialImpact AS ImpactoFinanceiro, IsCritical AS Critico, CreatedAtUtc AS CriadoEmUtc
              FROM Bottlenecks WHERE TenantId = @idEmpresa", new { idEmpresa })).ToList();
    }

    public async Task<Gargalo> InserirAsync(Gargalo gargalo, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO Bottlenecks (Id, TenantId, Title, Description, Area, FinancialImpact, IsCritical, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @Titulo, @Descricao, @Area, @ImpactoFinanceiro, @Critico, @CriadoEmUtc)", gargalo);
        return gargalo;
    }
}

public sealed class ClienteRepositorio(IFabricaConexaoBanco fabrica) : IClienteRepositorio
{
    public async Task<IReadOnlyList<Cliente>> ObterTodosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<Cliente>(
            @"SELECT Id, TenantId AS IdEmpresa, Name AS Nome, MonthlyRevenue AS ReceitaMensal,
                     LifetimeValue AS ValorVidaUtil, Status, OverdueAmount AS ValorEmAtraso,
                     ExpansionPotential AS PotencialExpansao, HealthScore AS PontuacaoSaude,
                     CreatedAtUtc AS CriadoEmUtc
              FROM Clients WHERE TenantId = @idEmpresa", new { idEmpresa })).ToList();
    }
}

public sealed class EquipeRepositorio(IFabricaConexaoBanco fabrica) : IEquipeRepositorio
{
    public async Task<IReadOnlyList<MembroEquipe>> ObterTodosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<MembroEquipe>(
            @"SELECT Id, TenantId AS IdEmpresa, Name AS Nome, Role AS Cargo,
                     RevenueGenerated AS ReceitaGerada, Target AS Meta,
                     ProductivityScore AS PontuacaoProdutividade, CreatedAtUtc AS CriadoEmUtc
              FROM TeamMembers WHERE TenantId = @idEmpresa", new { idEmpresa })).ToList();
    }
}

public sealed class MetaRepositorio(IFabricaConexaoBanco fabrica) : IMetaRepositorio
{
    public async Task<IReadOnlyList<Meta>> ObterAtivasAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<Meta>(
            @"SELECT Id, TenantId AS IdEmpresa, Name AS Nome, TargetValue AS ValorMeta,
                     CurrentValue AS ValorAtual, PeriodStart AS InicioPeriodo, PeriodEnd AS FimPeriodo,
                     CreatedAtUtc AS CriadoEmUtc
              FROM Goals WHERE TenantId = @idEmpresa", new { idEmpresa })).ToList();
    }
}

public sealed class AlertaRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IAlertaRepositorio
{
    public async Task<IReadOnlyList<Alerta>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<Alerta>(
            $@"SELECT Id, TenantId AS IdEmpresa, Title AS Titulo, Message AS Mensagem, Severity AS Severidade,
                     FinancialImpact AS ImpactoFinanceiro, RelatedFactor AS FatorRelacionado,
                     IsResolved AS Resolvido, CreatedAtUtc AS CriadoEmUtc
              FROM Alerts WHERE TenantId = @idEmpresa AND IsResolved = {dialecto.LiteralFalso} ORDER BY Severity DESC",
            new { idEmpresa })).ToList();
    }

    public async Task ResolverAsync(Guid idEmpresa, Guid idAlerta, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(
            $"UPDATE Alerts SET IsResolved = {dialecto.LiteralVerdadeiro}, UpdatedAtUtc = {dialecto.UtcAgora} WHERE Id = @idAlerta AND TenantId = @idEmpresa",
            new { idAlerta, idEmpresa });
    }

    public async Task<Alerta> InserirAsync(Alerta alerta, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO Alerts (Id, TenantId, Title, Message, Severity, FinancialImpact, RelatedFactor, IsResolved, CreatedAtUtc)
            VALUES (@Id, @IdEmpresa, @Titulo, @Mensagem, @Severidade, @ImpactoFinanceiro, @FatorRelacionado, @Resolvido, @CriadoEmUtc)", alerta);
        return alerta;
    }
}
