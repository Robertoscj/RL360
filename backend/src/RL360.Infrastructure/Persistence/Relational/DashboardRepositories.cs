using Dapper;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;

namespace RL360.Infrastructure.Persistence.Relational;

public sealed class SnapshotDashboardRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : ISnapshotDashboardRepositorio
{
    private const string Colunas = @"
        Id, TenantId AS IdEmpresa,
        RevenueToday AS FaturamentoDia, RevenueMonth AS FaturamentoMes,
        CurrentProfit AS LucroAtual, ProfitAtRisk AS LucroEmRisco,
        OpportunityAmount AS ValorOportunidade, CompanyHealthPercent AS SaudeEmpresaPercentual,
        CriticalBottlenecks AS GargalosCriticos, DelinquencyAmount AS ValorInadimplencia,
        ConversionRate AS TaxaConversao, MonthGoal AS MetaMes, MonthGoalPercent AS PercentualMetaMes,
        ForecastResult30Days AS PrevisaoResultado30Dias, ForecastGoal30Days AS MetaResultado30Dias,
        ForecastBelowGoalPercent AS PercentualAbaixoMetaPrevisao,
        CashFlowReceivable60Days AS FluxoAReceber60Dias, CashFlowAtRisk60Days AS FluxoEmRisco60Dias,
        CashFlowOverdue60Days AS FluxoAtrasado60Dias, CashFlowHealthyPercent AS PercentualFluxoSaudavel,
        PayloadJson, CreatedAtUtc AS CriadoEmUtc";

    public async Task<SnapshotDashboard?> ObterMaisRecenteAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        var limite = dialecto.SelecionarLimiteFixo(1);
        var sufixo = dialecto.SufixoLimiteFixo(1);
        return await db.QuerySingleOrDefaultAsync<SnapshotDashboard>(
            $"SELECT {limite} {Colunas} FROM DashboardSnapshots WHERE TenantId = @idEmpresa ORDER BY CreatedAtUtc DESC{sufixo}",
            new { idEmpresa });
    }

    public async Task SalvarAsync(SnapshotDashboard snapshot, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO DashboardSnapshots (
                Id, TenantId, RevenueToday, RevenueMonth, CurrentProfit, ProfitAtRisk,
                OpportunityAmount, CompanyHealthPercent, CriticalBottlenecks, DelinquencyAmount,
                ConversionRate, MonthGoal, MonthGoalPercent, ForecastResult30Days, ForecastGoal30Days,
                ForecastBelowGoalPercent, CashFlowReceivable60Days, CashFlowAtRisk60Days,
                CashFlowOverdue60Days, CashFlowHealthyPercent, PayloadJson, CreatedAtUtc)
            VALUES (
                @Id, @IdEmpresa, @FaturamentoDia, @FaturamentoMes, @LucroAtual, @LucroEmRisco,
                @ValorOportunidade, @SaudeEmpresaPercentual, @GargalosCriticos, @ValorInadimplencia,
                @TaxaConversao, @MetaMes, @PercentualMetaMes, @PrevisaoResultado30Dias, @MetaResultado30Dias,
                @PercentualAbaixoMetaPrevisao, @FluxoAReceber60Dias, @FluxoEmRisco60Dias,
                @FluxoAtrasado60Dias, @PercentualFluxoSaudavel, @PayloadJson, @CriadoEmUtc)", snapshot);
    }
}

public sealed class PlanoAcaoRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IPlanoAcaoRepositorio
{
    public async Task<IReadOnlyList<ItemPlanoAcaoRegistro>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<ItemPlanoAcaoRegistro>($@"
            SELECT Id, TenantId AS IdEmpresa, Title AS Titulo, Rationale AS Justificativa,
                   ExpectedImpact AS ImpactoEsperado, Priority AS Prioridade,
                   IsDone AS Executado, IsActive AS Ativo, CreatedAtUtc AS CriadoEmUtc
            FROM ActionPlanItems WHERE TenantId = @idEmpresa AND IsActive = {dialecto.LiteralVerdadeiro}",
            new { idEmpresa })).ToList();
    }
}

public sealed class InsightRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IInsightRepositorio
{
    public async Task<IReadOnlyList<InsightInteligente>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<InsightInteligente>($@"
            SELECT Id, TenantId AS IdEmpresa, Type AS Tipo, Title AS Titulo,
                   Description AS Descricao, ButtonText AS TextoBotao, ActionRoute AS RotaAcao,
                   FinancialImpact AS ImpactoFinanceiro, IsActive AS Ativo, CreatedAtUtc AS CriadoEmUtc
            FROM SmartInsights WHERE TenantId = @idEmpresa AND IsActive = {dialecto.LiteralVerdadeiro}",
            new { idEmpresa })).ToList();
    }
}

public sealed class ContaReceberRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IContaReceberRepositorio
{
    public async Task<IReadOnlyList<ContaReceber>> ObterProximos60DiasAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<ContaReceber>($@"
            SELECT Id, TenantId AS IdEmpresa, ClientId AS IdCliente, ClientName AS NomeCliente,
                   Amount AS Valor, DueAtUtc AS VencimentoEmUtc, Status, IsActive AS Ativo,
                   CreatedAtUtc AS CriadoEmUtc
            FROM Receivables
            WHERE TenantId = @idEmpresa AND IsActive = {dialecto.LiteralVerdadeiro}
              AND DueAtUtc <= {dialecto.UtcMaisDias(60)}",
            new { idEmpresa })).ToList();
    }
}
