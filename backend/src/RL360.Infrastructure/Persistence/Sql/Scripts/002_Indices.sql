-- RL360 — Script 002: Índices de performance (multi-tenant SaaS)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_TenantId')
CREATE NONCLUSTERED INDEX IX_Users_TenantId ON Usuarios(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RevenueSnapshots_TenantId')
CREATE NONCLUSTERED INDEX IX_RevenueSnapshots_TenantId ON SnapshotsFaturamento(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sales_TenantId')
CREATE NONCLUSTERED INDEX IX_Sales_TenantId ON Vendas(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FunnelStages_TenantId')
CREATE NONCLUSTERED INDEX IX_FunnelStages_TenantId ON EtapasFunil(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Clients_TenantId')
CREATE NONCLUSTERED INDEX IX_Clients_TenantId ON Clientes(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DelinquencyRecords_TenantId')
CREATE NONCLUSTERED INDEX IX_DelinquencyRecords_TenantId ON RegistrosInadimplencia(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bottlenecks_TenantId')
CREATE NONCLUSTERED INDEX IX_Bottlenecks_TenantId ON Gargalos(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TeamMembers_TenantId')
CREATE NONCLUSTERED INDEX IX_TeamMembers_TenantId ON MembrosEquipe(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Goals_TenantId')
CREATE NONCLUSTERED INDEX IX_Goals_TenantId ON Metas(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Alerts_TenantId')
CREATE NONCLUSTERED INDEX IX_Alerts_TenantId ON Alertas(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DashboardSnapshots_TenantId')
CREATE NONCLUSTERED INDEX IX_DashboardSnapshots_TenantId ON SnapshotsDashboard(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Receivables_TenantId')
CREATE NONCLUSTERED INDEX IX_Receivables_TenantId ON ContasReceber(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ActionPlanItems_TenantId')
CREATE NONCLUSTERED INDEX IX_ActionPlanItems_TenantId ON ItensPlanoAcao(TenantId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SmartInsights_TenantId')
CREATE NONCLUSTERED INDEX IX_SmartInsights_TenantId ON InsightsInteligentes(TenantId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RevenueSnapshots_CreatedAt')
CREATE NONCLUSTERED INDEX IX_RevenueSnapshots_CreatedAt ON SnapshotsFaturamento(CreatedAtUtc DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DashboardSnapshots_CreatedAt')
CREATE NONCLUSTERED INDEX IX_DashboardSnapshots_CreatedAt ON SnapshotsDashboard(CreatedAtUtc DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Alerts_CreatedAt')
CREATE NONCLUSTERED INDEX IX_Alerts_CreatedAt ON Alertas(CreatedAtUtc DESC);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DashboardSnapshots_Tenant_Created')
CREATE NONCLUSTERED INDEX IX_DashboardSnapshots_Tenant_Created
    ON SnapshotsDashboard(TenantId, CreatedAtUtc DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RevenueSnapshots_Tenant_Created')
CREATE NONCLUSTERED INDEX IX_RevenueSnapshots_Tenant_Created
    ON SnapshotsFaturamento(TenantId, CreatedAtUtc DESC);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Alerts_Tenant_Resolved')
CREATE NONCLUSTERED INDEX IX_Alerts_Tenant_Resolved
    ON Alertas(TenantId, IsResolved) WHERE IsResolved = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Clients_Tenant_Status')
CREATE NONCLUSTERED INDEX IX_Clients_Tenant_Status
    ON Clientes(TenantId, Status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Receivables_Tenant_Status')
CREATE NONCLUSTERED INDEX IX_Receivables_Tenant_Status
    ON ContasReceber(TenantId, Status) WHERE IsActive = 1;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Receivables_Tenant_Client')
CREATE NONCLUSTERED INDEX IX_Receivables_Tenant_Client
    ON ContasReceber(TenantId, ClientId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DelinquencyRecords_Tenant_Client')
CREATE NONCLUSTERED INDEX IX_DelinquencyRecords_Tenant_Client
    ON RegistrosInadimplencia(TenantId, ClientId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sales_Tenant_Client')
CREATE NONCLUSTERED INDEX IX_Sales_Tenant_Client
    ON Vendas(TenantId, ClientId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Users_Email')
CREATE UNIQUE NONCLUSTERED INDEX UX_Users_Email ON Usuarios(Email);

GO
