-- RL360 — Script 003: Seed do Radar de Lucro
-- IDs alinhados com o modo Demo (ceo@rl360.com / rl360@2026)
-- Executar com UTF-8: sqlcmd -S localhost\SQLEXPRESS -E -C -I -f 65001 -d RL360 -i 003_Seed.sql

DECLARE @TenantId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @UserId   UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

DECLARE @ClienteHorizonte UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333331';
DECLARE @ClienteVale      UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333332';
DECLARE @ClienteBoaVista  UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @ClienteUniao     UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333334';
DECLARE @ClienteNorte     UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333335';
DECLARE @ClientePrimavera UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333336';
DECLARE @ClienteAurora    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333337';
DECLARE @ClienteAndaza    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333338';

IF NOT EXISTS (SELECT 1 FROM Empresas WHERE Id = @TenantId)
INSERT INTO Empresas (Id, Name, Document, Segment, PlanCode, IsActive)
VALUES (@TenantId, N'Grupo Aurora Distribuição', N'12.345.678/0001-90', N'Distribuição', N'scale', 1);

-- Senha: rl360@2026
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Id = @UserId)
INSERT INTO Usuarios (Id, TenantId, Name, Email, PasswordHash, Role, IsActive)
VALUES (@UserId, @TenantId, N'Roberto Silva', N'ceo@rl360.com',
        N'$2a$11$qtpaLcek8Zr29aCMQIzlMu7Fh4nwnE5tqm/oaZByqTDd1c88OK7vu', 0, 1);

IF EXISTS (SELECT 1 FROM Clientes WHERE TenantId = @TenantId)
    GOTO FimSeedRadar;

-- StatusCliente: Saudavel=0, EmRisco=1, OportunidadeExpansao=2, Inadimplente=3
INSERT INTO Clientes (Id, TenantId, Name, MonthlyRevenue, LifetimeValue, Status, OverdueAmount, ExpansionPotential, HealthScore)
VALUES
    (@ClienteHorizonte, @TenantId, N'Rede Horizonte',        120000, 1450000, 2, 0,      90000, 100),
    (@ClienteVale,      @TenantId, N'Supermercados Vale',     64000,  880000, 2, 0,      50000,  84),
    (@ClienteBoaVista,  @TenantId, N'Atacado Boa Vista',      56000,  410000, 1, 130000,     0,  35),
    (@ClienteUniao,     @TenantId, N'Mercado União',          41000,  220000, 1,  55000,     0,  35),
    (@ClienteNorte,     @TenantId, N'Distribuidora Norte',    98000, 1120000, 0,      0,     0,  92),
    (@ClientePrimavera, @TenantId, N'Grupo Primavera',        76000,  940000, 0,      0,     0,  88),
    (@ClienteAurora,    @TenantId, N'Comercial Aurora',       52000,  610000, 0,      0,     0,  81),
    (@ClienteAndaza,    @TenantId, N'Comercial Andaza Ltda',  38000,  280000, 3,  92000,     0,  22);

DECLARE @Hoje DATE = CAST(SYSUTCDATETIME() AS DATE);
DECLARE @Dia DATE = DATEADD(DAY, -29, @Hoje);
DECLARE @Acumulado DECIMAL(18,2) = 0;
DECLARE @ValorDia DECIMAL(18,2);
DECLARE @FatorFimSemana DECIMAL(8,2);

WHILE @Dia <= @Hoje
BEGIN
    SET @FatorFimSemana = CASE WHEN DATEPART(WEEKDAY, @Dia) IN (1, 7) THEN 0.58 ELSE 1 END;
    SET @ValorDia = CASE WHEN @Dia = @Hoje THEN 58420
        ELSE ROUND((32000 + (DATEPART(DAY, @Dia) * 850) + (DATEPART(WEEKDAY, @Dia) * 1400)) * @FatorFimSemana, 2) END;
    SET @Acumulado = @Acumulado + @ValorDia;

    INSERT INTO SnapshotsFaturamento (Id, TenantId, ReferenceDate, RevenueDay, RevenueMonth, MonthlyTarget, CostMonth, FixedCostMonth)
    VALUES (
        NEWID(), @TenantId, @Dia, @ValorDia,
        CASE WHEN @Dia = @Hoje THEN 1250000 ELSE @Acumulado END,
        1500000, 812680, 410000);

    SET @Dia = DATEADD(DAY, 1, @Dia);
END;

INSERT INTO Vendas (Id, TenantId, ClientId, Channel, Amount, Margin, ClosedAtUtc, IsNewClient)
VALUES
    (NEWID(), @TenantId, @ClienteHorizonte, N'Inbound',     232000, 78500, DATEADD(DAY, -2,  SYSUTCDATETIME()), 1),
    (NEWID(), @TenantId, @ClienteVale,      N'Outbound',    180000, 60000, DATEADD(DAY, -5,  SYSUTCDATETIME()), 1),
    (NEWID(), @TenantId, @ClienteAurora,    N'Indicação',   121000, 41000, DATEADD(DAY, -9,  SYSUTCDATETIME()), 1),
    (NEWID(), @TenantId, @ClienteNorte,     N'Marketplace', 104000, 35000, DATEADD(DAY, -14, SYSUTCDATETIME()), 1),
    (NEWID(), @TenantId, @ClientePrimavera, N'Recorrência',  96000, 33500, DATEADD(DAY, -1,  SYSUTCDATETIME()), 0),
    (NEWID(), @TenantId, @ClienteHorizonte, N'Recorrência',  88000, 30100, DATEADD(DAY, -3,  SYSUTCDATETIME()), 0),
    (NEWID(), @TenantId, @ClienteNorte,     N'Upsell',       64000, 22400, DATEADD(DAY, -7,  SYSUTCDATETIME()), 0);

INSERT INTO EtapasFunil (Id, TenantId, Name, [Order], Count, PotentialValue, ConversionRate, BaselineConversionRate)
VALUES
    (NEWID(), @TenantId, N'Lead',         1, 1200, 700000, 0.35, 0.45),
    (NEWID(), @TenantId, N'Qualificação', 2,  420, 600000, 0.28, 0.34),
    (NEWID(), @TenantId, N'Proposta',     3,  150, 400000, 0.16, 0.24),
    (NEWID(), @TenantId, N'Negociação',   4,   60, 200000, 0.09, 0.17);

INSERT INTO RegistrosInadimplencia (Id, TenantId, ClientId, ClientName, Amount, DaysOverdue, RecoveryProbability)
VALUES
    (NEWID(), @TenantId, @ClienteBoaVista, N'Atacado Boa Vista',     130000, 62, 0.40),
    (NEWID(), @TenantId, @ClienteAndaza,   N'Comercial Andaza Ltda',  92000, 45, 0.30),
    (NEWID(), @TenantId, @ClienteUniao,    N'Mercado União',          55000, 38, 0.40),
    (NEWID(), @TenantId, NULL,             N'Distribuidora Sul',      35000, 21, 0.7829);

-- AreaGargalo: Comercial=0, Operacional=2, Atendimento=3, Logistica=4
INSERT INTO Gargalos (Id, TenantId, Title, Description, Area, FinancialImpact, IsCritical)
VALUES
    (NEWID(), @TenantId, N'Tempo de resposta a leads acima de 12h', N'Leads esfriando antes do primeiro contato.', 0, -42000, 1),
    (NEWID(), @TenantId, N'Propostas paradas sem follow-up', N'62 propostas sem retorno há mais de 7 dias.', 0, -50000, 1),
    (NEWID(), @TenantId, N'Atraso recorrente na entrega', N'SLA de entrega estourado em 18% dos pedidos.', 4, -38000, 1),
    (NEWID(), @TenantId, N'Fila de atendimento sobrecarregada', N'Tempo médio de espera de 9 minutos.', 3, -27000, 1),
    (NEWID(), @TenantId, N'Retrabalho no faturamento', N'Notas reemitidas por erro de cadastro.', 2, -15000, 0);

INSERT INTO MembrosEquipe (Id, TenantId, Name, Role, RevenueGenerated, Target, ProductivityScore)
VALUES
    (NEWID(), @TenantId, N'Ana Martins',  N'Closer Sênior', 320000, 280000, 92),
    (NEWID(), @TenantId, N'Bruno Costa',  N'Closer Pleno',  245000, 230000, 80),
    (NEWID(), @TenantId, N'Carla Dias',   N'SDR Líder',     198000, 150000, 88),
    (NEWID(), @TenantId, N'Diego Souza',  N'SDR',            95500, 100000, 61);

DECLARE @InicioMes DATETIME2 = DATETIMEFROMPARTS(YEAR(SYSUTCDATETIME()), MONTH(SYSUTCDATETIME()), 1, 0, 0, 0, 0);
DECLARE @FimMes DATETIME2 = DATEADD(DAY, -1, DATEADD(MONTH, 1, @InicioMes));

INSERT INTO Metas (Id, TenantId, Name, TargetValue, CurrentValue, PeriodStart, PeriodEnd)
VALUES
    (NEWID(), @TenantId, N'Faturamento mensal', 1500000, 1250000, @InicioMes, @FimMes),
    (NEWID(), @TenantId, N'Novos clientes', 40, 27, @InicioMes, @FimMes),
    (NEWID(), @TenantId, N'Redução de inadimplência (R$)', 150000, 96000, @InicioMes, @FimMes),
    (NEWID(), @TenantId, N'Conversão do funil (%)', 30, 22, @InicioMes, @FimMes);

-- SeveridadeAlerta: Info=0, Atencao=1, Critico=2
-- FatorRadar: Inadimplencia=0, Conversao=1, VendasNovas=2, ExpansaoClientes=4, GargaloComercial=5
INSERT INTO Alertas (Id, TenantId, Title, Message, Severity, FinancialImpact, RelatedFactor, IsResolved)
VALUES
    (NEWID(), @TenantId, N'Inadimplência acima do normal', N'Impacto financeiro relevante na região Norte.', 2, -183000, 0, 0),
    (NEWID(), @TenantId, N'Aprovação muito lenta', N'Análise de crédito com atraso médio de 3h45m.', 2, -92000, 5, 0),
    (NEWID(), @TenantId, N'Queda na conversão', N'Conversão caiu de 30% para 22%.', 2, -152000, 1, 0),
    (NEWID(), @TenantId, N'Oportunidade na região Sul', N'Potencial de expansão identificado.', 0, 126000, 4, 0),
    (NEWID(), @TenantId, N'Produto em alta', N'Produto A com forte demanda e cross-sell.', 0, 186000, 2, 0),
    (NEWID(), @TenantId, N'Risco de perda em 30 dias', N'Você pode perder R$ 487.320 nos próximos 30 dias se nada for feito.', 2, -487320, 0, 0);

INSERT INTO InsightsInteligentes (Id, TenantId, Type, Title, Description, ButtonText, ActionRoute, FinancialImpact, IsActive)
VALUES
    (NEWID(), @TenantId, N'AcaoUrgente', N'Ação urgente recomendada',
        N'Negocie com os clientes da Região Norte. 65% do aumento da inadimplência vem desta região.',
        N'Ver clientes', N'/clientes', -183000, 1),
    (NEWID(), @TenantId, N'Gargalo', N'Gargalo identificado',
        N'A etapa de análise de crédito está causando atraso médio de 3h45m.',
        N'Ver gargalo', N'/gargalos', -92000, 1),
    (NEWID(), @TenantId, N'Oportunidade', N'Oportunidade real',
        N'Clientes que compram o Produto A também têm 70% mais chance de comprar o Produto B.',
        N'Ver oportunidade', N'/vendas', 186000, 1);

-- PrioridadeAcao: Media=1, Alta=2, Urgente=3
INSERT INTO ItensPlanoAcao (Id, TenantId, Title, Rationale, ExpectedImpact, Priority, IsDone, IsActive)
VALUES
    (NEWID(), @TenantId, N'Acionar régua de cobrança nos maiores inadimplentes', N'Recuperar parte dos R$ 183.000 em risco.', 73200, 3, 0, 1),
    (NEWID(), @TenantId, N'Revisar etapas do funil com maior queda', N'Recuperar conversão e destravar pipeline.', 53200, 2, 0, 1),
    (NEWID(), @TenantId, N'Ofertar expansão para clientes de alto potencial', N'Potencial de R$ 132.000 em expansão.', 79200, 1, 0, 1),
    (NEWID(), @TenantId, N'Eliminar gargalo comercial prioritário', N'Destravar propostas paradas.', 46000, 2, 0, 1),
    (NEWID(), @TenantId, N'Reduzir tempo de análise de crédito', N'Meta: abaixo de 1h por proposta.', 36800, 2, 0, 1),
    (NEWID(), @TenantId, N'Campanha cross-sell Produto A → B', N'Aproveitar correlação de 70%.', 130200, 1, 0, 1),
    (NEWID(), @TenantId, N'Reforçar equipe SDR na Região Norte', N'Atacar origem da inadimplência.', 42500, 3, 0, 1);

INSERT INTO ContasReceber (Id, TenantId, ClientId, ClientName, Amount, DueAtUtc, Status, IsActive)
VALUES
    (NEWID(), @TenantId, @ClienteHorizonte, N'Rede Horizonte',         980000, DATEADD(DAY,  15, CAST(@Hoje AS DATETIME2)), N'AReceber', 1),
    (NEWID(), @TenantId, @ClienteNorte,     N'Distribuidora Norte',    720000, DATEADD(DAY,  28, CAST(@Hoje AS DATETIME2)), N'AReceber', 1),
    (NEWID(), @TenantId, @ClientePrimavera, N'Grupo Primavera',        542000, DATEADD(DAY,  45, CAST(@Hoje AS DATETIME2)), N'AReceber', 1),
    (NEWID(), @TenantId, @ClienteBoaVista,  N'Atacado Boa Vista',      420000, DATEADD(DAY,  10, CAST(@Hoje AS DATETIME2)), N'EmRisco',  1),
    (NEWID(), @TenantId, @ClienteUniao,     N'Mercado União',          222000, DATEADD(DAY,   5, CAST(@Hoje AS DATETIME2)), N'EmRisco',  1),
    (NEWID(), @TenantId, @ClienteAndaza,    N'Comercial Andaza Ltda',  285000, DATEADD(DAY, -12, CAST(@Hoje AS DATETIME2)), N'Atrasado', 1);

IF NOT EXISTS (SELECT 1 FROM SnapshotsDashboard WHERE TenantId = @TenantId)
INSERT INTO SnapshotsDashboard (
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

FimSeedRadar:

IF NOT EXISTS (SELECT 1 FROM DocumentosConhecimento WHERE TenantId = @TenantId)
INSERT INTO DocumentosConhecimento (Id, TenantId, Title, Category, Content, IsActive)
VALUES
    (NEWID(), @TenantId, N'Política de análise de crédito', N'Crédito',
     N'Prazo máximo de aprovação: 1 hora útil. Propostas acima de R$ 50.000 exigem segunda alçada. Taxa de conversão cai 3,2pp quando o SLA estoura.', 1),
    (NEWID(), @TenantId, N'Playbook comercial — Região Norte', N'Vendas',
     N'A inadimplência na Região Norte está 18% acima da média. Priorizar clientes com score acima de 720 e oferecer desconto à vista de 2% para antecipação.', 1),
    (NEWID(), @TenantId, N'Metas trimestrais Q2', N'Metas',
     N'Meta de faturamento: R$ 1,5M/mês. Meta de conversão: 25%. Meta de inadimplência máxima: 8% do faturamento. Cross-sell Produto A→B tem 70% de conversão histórica.', 1),
    (NEWID(), @TenantId, N'Régua de cobrança padrão', N'Financeiro',
     N'D+1: lembrete automático. D+7: contato SDR. D+15: escalonamento gerente. D+30: bloqueio de novos pedidos. Recuperação média: 40% entre D+7 e D+15.', 1),
    (NEWID(), @TenantId, N'Gargalo de aprovação comercial', N'Operações',
     N'Tempo médio atual: 3h45m por proposta. Causa principal: fila manual de análise. Automatizar pré-aprovação para tickets abaixo de R$ 15.000 reduz 60% do volume.', 1);

GO
