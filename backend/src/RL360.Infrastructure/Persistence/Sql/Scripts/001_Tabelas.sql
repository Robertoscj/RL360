-- RL360 — Script 001: Criação de tabelas
-- Executar em ordem: 001 → 002 → 003

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Empresas')
CREATE TABLE Empresas (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Name            NVARCHAR(200)    NOT NULL,
    Document        NVARCHAR(20)     NOT NULL,
    Segment         NVARCHAR(100)    NOT NULL DEFAULT '',
    PlanCode        NVARCHAR(50)     NOT NULL DEFAULT 'pro',
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Usuarios')
CREATE TABLE Usuarios (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Name            NVARCHAR(200)    NOT NULL,
    Email           NVARCHAR(256)    NOT NULL,
    PasswordHash    NVARCHAR(500)    NOT NULL,
    Role            INT              NOT NULL DEFAULT 3,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_Users_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SnapshotsFaturamento')
CREATE TABLE SnapshotsFaturamento (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    ReferenceDate   DATE             NOT NULL,
    RevenueDay      DECIMAL(18,2)    NOT NULL,
    RevenueMonth    DECIMAL(18,2)    NOT NULL,
    MonthlyTarget   DECIMAL(18,2)    NOT NULL,
    CostMonth       DECIMAL(18,2)    NOT NULL,
    FixedCostMonth  DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_RevenueSnapshots_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Vendas')
CREATE TABLE Vendas (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    ClientId        UNIQUEIDENTIFIER NULL,
    Channel         NVARCHAR(100)    NOT NULL,
    Amount          DECIMAL(18,2)    NOT NULL,
    Margin          DECIMAL(18,2)    NOT NULL,
    ClosedAtUtc     DATETIME2        NOT NULL,
    IsNewClient     BIT              NOT NULL DEFAULT 0,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_Sales_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EtapasFunil')
CREATE TABLE EtapasFunil (
    Id                      UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId                UNIQUEIDENTIFIER NOT NULL,
    Name                    NVARCHAR(100)    NOT NULL,
    [Order]                 INT              NOT NULL,
    Count                   INT              NOT NULL,
    PotentialValue          DECIMAL(18,2)    NOT NULL,
    ConversionRate          DECIMAL(8,4)     NOT NULL,
    BaselineConversionRate  DECIMAL(8,4)     NOT NULL,
    CreatedAtUtc            DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc            DATETIME2        NULL,
    CONSTRAINT FK_FunnelStages_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Clientes')
CREATE TABLE Clientes (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL,
    Name                NVARCHAR(200)    NOT NULL,
    MonthlyRevenue      DECIMAL(18,2)    NOT NULL DEFAULT 0,
    LifetimeValue       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Status              INT              NOT NULL DEFAULT 0,
    OverdueAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    ExpansionPotential  DECIMAL(18,2)    NOT NULL DEFAULT 0,
    HealthScore         INT              NOT NULL DEFAULT 100,
    CreatedAtUtc        DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc        DATETIME2        NULL,
    CONSTRAINT FK_Clients_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RegistrosInadimplencia')
CREATE TABLE RegistrosInadimplencia (
    Id                      UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId                UNIQUEIDENTIFIER NOT NULL,
    ClientId                UNIQUEIDENTIFIER NULL,
    ClientName              NVARCHAR(200)    NOT NULL,
    Amount                  DECIMAL(18,2)    NOT NULL,
    DaysOverdue             INT              NOT NULL,
    RecoveryProbability     DECIMAL(8,4)     NOT NULL,
    CreatedAtUtc            DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc            DATETIME2        NULL,
    CONSTRAINT FK_DelinquencyRecords_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Gargalos')
CREATE TABLE Gargalos (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Title           NVARCHAR(300)    NOT NULL,
    Description     NVARCHAR(1000)   NOT NULL,
    Area            INT              NOT NULL,
    FinancialImpact DECIMAL(18,2)    NOT NULL,
    IsCritical      BIT              NOT NULL DEFAULT 0,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_Bottlenecks_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MembrosEquipe')
CREATE TABLE MembrosEquipe (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL,
    Name                NVARCHAR(200)    NOT NULL,
    Role                NVARCHAR(100)    NOT NULL,
    RevenueGenerated    DECIMAL(18,2)    NOT NULL,
    Target              DECIMAL(18,2)    NOT NULL,
    ProductivityScore   INT              NOT NULL,
    CreatedAtUtc        DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc        DATETIME2        NULL,
    CONSTRAINT FK_TeamMembers_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Metas')
CREATE TABLE Metas (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Name            NVARCHAR(200)    NOT NULL,
    TargetValue     DECIMAL(18,2)    NOT NULL,
    CurrentValue    DECIMAL(18,2)    NOT NULL,
    PeriodStart     DATETIME2        NOT NULL,
    PeriodEnd       DATETIME2        NOT NULL,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_Goals_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Alertas')
CREATE TABLE Alertas (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Title           NVARCHAR(300)    NOT NULL,
    Message         NVARCHAR(1000)   NOT NULL,
    Severity        INT              NOT NULL,
    FinancialImpact DECIMAL(18,2)    NOT NULL,
    RelatedFactor   INT              NULL,
    IsResolved      BIT              NOT NULL DEFAULT 0,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_Alerts_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SnapshotsDashboard')
CREATE TABLE SnapshotsDashboard (
    Id                          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId                    UNIQUEIDENTIFIER NOT NULL,
    RevenueToday                DECIMAL(18,2)    NOT NULL,
    RevenueMonth                DECIMAL(18,2)    NOT NULL,
    CurrentProfit               DECIMAL(18,2)    NOT NULL,
    ProfitAtRisk                DECIMAL(18,2)    NOT NULL,
    OpportunityAmount           DECIMAL(18,2)    NOT NULL,
    CompanyHealthPercent        INT              NOT NULL,
    CriticalBottlenecks         INT              NOT NULL,
    DelinquencyAmount           DECIMAL(18,2)    NOT NULL,
    ConversionRate              DECIMAL(8,4)     NOT NULL,
    MonthGoal                   DECIMAL(18,2)    NOT NULL,
    MonthGoalPercent            DECIMAL(8,2)     NOT NULL,
    ForecastResult30Days        DECIMAL(18,2)    NOT NULL,
    ForecastGoal30Days          DECIMAL(18,2)    NOT NULL,
    ForecastBelowGoalPercent    DECIMAL(8,2)     NOT NULL,
    CashFlowReceivable60Days    DECIMAL(18,2)    NOT NULL,
    CashFlowAtRisk60Days        DECIMAL(18,2)    NOT NULL,
    CashFlowOverdue60Days       DECIMAL(18,2)    NOT NULL,
    CashFlowHealthyPercent      INT              NOT NULL,
    PayloadJson                 NVARCHAR(MAX)    NOT NULL DEFAULT '{}',
    CreatedAtUtc                DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc                DATETIME2        NULL,
    CONSTRAINT FK_DashboardSnapshots_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ContasReceber')
CREATE TABLE ContasReceber (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    ClientId        UNIQUEIDENTIFIER NULL,
    ClientName      NVARCHAR(200)    NOT NULL,
    Amount          DECIMAL(18,2)    NOT NULL,
    DueAtUtc        DATETIME2        NOT NULL,
    Status          NVARCHAR(50)     NOT NULL DEFAULT 'AReceber',
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_Receivables_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ItensPlanoAcao')
CREATE TABLE ItensPlanoAcao (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Title           NVARCHAR(300)    NOT NULL,
    Rationale       NVARCHAR(1000)   NOT NULL,
    ExpectedImpact  DECIMAL(18,2)    NOT NULL,
    Priority        INT              NOT NULL,
    IsDone          BIT              NOT NULL DEFAULT 0,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_ActionPlanItems_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InsightsInteligentes')
CREATE TABLE InsightsInteligentes (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Type            NVARCHAR(50)     NOT NULL,
    Title           NVARCHAR(300)    NOT NULL,
    Description     NVARCHAR(1000)   NOT NULL,
    ButtonText      NVARCHAR(100)    NOT NULL,
    ActionRoute     NVARCHAR(200)    NOT NULL,
    FinancialImpact DECIMAL(18,2)    NULL,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_SmartInsights_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Sales_Clients')
ALTER TABLE Vendas ADD CONSTRAINT FK_Sales_Clients FOREIGN KEY (ClientId) REFERENCES Clientes(Id);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DelinquencyRecords_Clients')
ALTER TABLE RegistrosInadimplencia ADD CONSTRAINT FK_DelinquencyRecords_Clients FOREIGN KEY (ClientId) REFERENCES Clientes(Id);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Receivables_Clients')
ALTER TABLE ContasReceber ADD CONSTRAINT FK_Receivables_Clients FOREIGN KEY (ClientId) REFERENCES Clientes(Id);

GO
