IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MensagensConversaIa')
CREATE TABLE MensagensConversaIa (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    UserId          UNIQUEIDENTIFIER NOT NULL,
    ConversationId  UNIQUEIDENTIFIER NOT NULL,
    Role            NVARCHAR(20)     NOT NULL,
    Content         NVARCHAR(MAX)    NOT NULL,
    Mode            NVARCHAR(30)     NOT NULL DEFAULT 'Demo',
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AiChatMessages_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DocumentosConhecimento')
CREATE TABLE DocumentosConhecimento (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    TenantId        UNIQUEIDENTIFIER NOT NULL,
    Title           NVARCHAR(300)    NOT NULL,
    Category        NVARCHAR(100)    NOT NULL,
    Content         NVARCHAR(MAX)    NOT NULL,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAtUtc    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc    DATETIME2        NULL,
    CONSTRAINT FK_KnowledgeDocuments_Tenants FOREIGN KEY (TenantId) REFERENCES Empresas(Id)
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AiChatMessages_TenantUser')
CREATE NONCLUSTERED INDEX IX_AiChatMessages_TenantUser
    ON MensagensConversaIa(TenantId, UserId, CreatedAtUtc DESC);

GO
