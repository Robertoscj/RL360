-- RL360 — Script 002: Índices de performance (multi-tenant SaaS)

-- TenantId isolado
CREATE NONCLUSTERED INDEX IX_Users_TenantId ON Users(TenantId);
CREATE NONCLUSTERED INDEX IX_RevenueSnapshots_TenantId ON RevenueSnapshots(TenantId);
CREATE NONCLUSTERED INDEX IX_Sales_TenantId ON Sales(TenantId);
CREATE NONCLUSTERED INDEX IX_FunnelStages_TenantId ON FunnelStages(TenantId);
CREATE NONCLUSTERED INDEX IX_Clients_TenantId ON Clients(TenantId);
CREATE NONCLUSTERED INDEX IX_DelinquencyRecords_TenantId ON DelinquencyRecords(TenantId);
CREATE NONCLUSTERED INDEX IX_Bottlenecks_TenantId ON Bottlenecks(TenantId);
CREATE NONCLUSTERED INDEX IX_TeamMembers_TenantId ON TeamMembers(TenantId);
CREATE NONCLUSTERED INDEX IX_Goals_TenantId ON Goals(TenantId);
CREATE NONCLUSTERED INDEX IX_Alerts_TenantId ON Alerts(TenantId);
CREATE NONCLUSTERED INDEX IX_DashboardSnapshots_TenantId ON DashboardSnapshots(TenantId);
CREATE NONCLUSTERED INDEX IX_Receivables_TenantId ON Receivables(TenantId);
CREATE NONCLUSTERED INDEX IX_ActionPlanItems_TenantId ON ActionPlanItems(TenantId);
CREATE NONCLUSTERED INDEX IX_SmartInsights_TenantId ON SmartInsights(TenantId);

-- CreatedAt para séries temporais
CREATE NONCLUSTERED INDEX IX_RevenueSnapshots_CreatedAt ON RevenueSnapshots(CreatedAtUtc DESC);
CREATE NONCLUSTERED INDEX IX_DashboardSnapshots_CreatedAt ON DashboardSnapshots(CreatedAtUtc DESC);
CREATE NONCLUSTERED INDEX IX_Alerts_CreatedAt ON Alerts(CreatedAtUtc DESC);

-- Compostos TenantId + CreatedAt (consultas do dashboard)
CREATE NONCLUSTERED INDEX IX_DashboardSnapshots_Tenant_Created
    ON DashboardSnapshots(TenantId, CreatedAtUtc DESC);

CREATE NONCLUSTERED INDEX IX_RevenueSnapshots_Tenant_Created
    ON RevenueSnapshots(TenantId, CreatedAtUtc DESC);

-- Compostos TenantId + Status
CREATE NONCLUSTERED INDEX IX_Alerts_Tenant_Resolved
    ON Alerts(TenantId, IsResolved) WHERE IsResolved = 0;

CREATE NONCLUSTERED INDEX IX_Clients_Tenant_Status
    ON Clients(TenantId, Status);

CREATE NONCLUSTERED INDEX IX_Receivables_Tenant_Status
    ON Receivables(TenantId, Status) WHERE IsActive = 1;

-- TenantId + ClientId
CREATE NONCLUSTERED INDEX IX_Receivables_Tenant_Client
    ON Receivables(TenantId, ClientId);

CREATE NONCLUSTERED INDEX IX_DelinquencyRecords_Tenant_Client
    ON DelinquencyRecords(TenantId, ClientId);

-- Email único por usuário
CREATE UNIQUE NONCLUSTERED INDEX UX_Users_Email ON Users(Email);

GO
