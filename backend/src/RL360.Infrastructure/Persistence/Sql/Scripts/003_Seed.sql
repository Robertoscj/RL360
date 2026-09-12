-- RL360 — Script 003: Seed de demonstração
-- IDs fixos compatíveis com ArmazenamentoDemo (modo Demo não usa SQL; útil para ambiente SqlServer)

DECLARE @TenantId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @UserId   UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

IF NOT EXISTS (SELECT 1 FROM Tenants WHERE Id = @TenantId)
INSERT INTO Tenants (Id, Name, Document, Segment, PlanCode, IsActive)
VALUES (@TenantId, N'Grupo Aurora Distribuição', N'12.345.678/0001-90', N'Distribuição', N'scale', 1);

-- Senha: rl360@2026 (BCrypt hash gerado pelo app; substitua após primeiro login em produção)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId)
INSERT INTO Users (Id, TenantId, Name, Email, PasswordHash, Role, IsActive)
VALUES (@UserId, @TenantId, N'Roberto Silva', N'ceo@rl360.com',
        N'$2a$11$placeholder-substituir-pelo-hash-real', 0, 1);

IF NOT EXISTS (SELECT 1 FROM DashboardSnapshots WHERE TenantId = @TenantId)
INSERT INTO DashboardSnapshots (
    Id, TenantId, RevenueToday, RevenueMonth, CurrentProfit, ProfitAtRisk,
    OpportunityAmount, CompanyHealthPercent, CriticalBottlenecks, DelinquencyAmount,
    ConversionRate, MonthGoal, MonthGoalPercent, ForecastResult30Days, ForecastGoal30Days,
    ForecastBelowGoalPercent, CashFlowReceivable60Days, CashFlowAtRisk60Days,
    CashFlowOverdue60Days, CashFlowHealthyPercent, PayloadJson)
VALUES (
    NEWID(), @TenantId,
    58420, 1250000, 1247850, 487320,
    214500, 78, 4, 312000,
    0.22, 1500000, 83.3,
    5712000, 6650000, 14,
    3842000, 642000, 285000, 78, N'{}');

GO
