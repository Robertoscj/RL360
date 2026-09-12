-- RL360 — PostgreSQL: Script 001 (tabelas)
-- Executar em ordem: 001 → 002 → 003 → 004

CREATE TABLE IF NOT EXISTS tenants (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    name            VARCHAR(200)    NOT NULL,
    document        VARCHAR(20)     NOT NULL,
    segment         VARCHAR(100)    NOT NULL DEFAULT '',
    plancode        VARCHAR(50)     NOT NULL DEFAULT 'pro',
    isactive        BOOLEAN         NOT NULL DEFAULT TRUE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS users (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    name            VARCHAR(200)    NOT NULL,
    email           VARCHAR(256)    NOT NULL,
    passwordhash    VARCHAR(500)    NOT NULL,
    role            INT             NOT NULL DEFAULT 3,
    isactive        BOOLEAN         NOT NULL DEFAULT TRUE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS revenuesnapshots (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    referencedate   DATE            NOT NULL,
    revenueday      DECIMAL(18,2)   NOT NULL,
    revenuemonth    DECIMAL(18,2)   NOT NULL,
    monthlytarget   DECIMAL(18,2)   NOT NULL,
    costmonth       DECIMAL(18,2)   NOT NULL,
    fixedcostmonth  DECIMAL(18,2)   NOT NULL DEFAULT 0,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS sales (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    clientid        UUID            NULL,
    channel         VARCHAR(100)    NOT NULL,
    amount          DECIMAL(18,2)   NOT NULL,
    margin          DECIMAL(18,2)   NOT NULL,
    closedatutc     TIMESTAMPTZ     NOT NULL,
    isnewclient     BOOLEAN         NOT NULL DEFAULT FALSE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS funnelstages (
    id                      UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid                UUID            NOT NULL REFERENCES tenants(id),
    name                    VARCHAR(100)    NOT NULL,
    "order"                 INT             NOT NULL,
    count                   INT             NOT NULL,
    potentialvalue          DECIMAL(18,2)   NOT NULL,
    conversionrate          DECIMAL(8,4)    NOT NULL,
    baselineconversionrate  DECIMAL(8,4)    NOT NULL,
    createdatutc            TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc            TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS clients (
    id                  UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid            UUID            NOT NULL REFERENCES tenants(id),
    name                VARCHAR(200)    NOT NULL,
    monthlyrevenue      DECIMAL(18,2)   NOT NULL DEFAULT 0,
    lifetimevalue       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    status              INT             NOT NULL DEFAULT 0,
    overdueamount       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    expansionpotential  DECIMAL(18,2)   NOT NULL DEFAULT 0,
    healthscore         INT             NOT NULL DEFAULT 100,
    createdatutc        TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc        TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS delinquencyrecords (
    id                      UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid                UUID            NOT NULL REFERENCES tenants(id),
    clientid                UUID            NULL,
    clientname              VARCHAR(200)    NOT NULL,
    amount                  DECIMAL(18,2)   NOT NULL,
    daysoverdue             INT             NOT NULL,
    recoveryprobability     DECIMAL(8,4)    NOT NULL,
    createdatutc            TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc            TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS bottlenecks (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    title           VARCHAR(300)    NOT NULL,
    description     VARCHAR(1000)   NOT NULL,
    area            INT             NOT NULL,
    financialimpact DECIMAL(18,2)   NOT NULL,
    iscritical      BOOLEAN         NOT NULL DEFAULT FALSE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS teammembers (
    id                  UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid            UUID            NOT NULL REFERENCES tenants(id),
    name                VARCHAR(200)    NOT NULL,
    role                VARCHAR(100)    NOT NULL,
    revenuegenerated    DECIMAL(18,2)   NOT NULL,
    target              DECIMAL(18,2)   NOT NULL,
    productivityscore   INT             NOT NULL,
    createdatutc        TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc        TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS goals (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    name            VARCHAR(200)    NOT NULL,
    targetvalue     DECIMAL(18,2)   NOT NULL,
    currentvalue    DECIMAL(18,2)   NOT NULL,
    periodstart     TIMESTAMPTZ     NOT NULL,
    periodend       TIMESTAMPTZ     NOT NULL,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS alerts (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    title           VARCHAR(300)    NOT NULL,
    message         VARCHAR(1000)   NOT NULL,
    severity        INT             NOT NULL,
    financialimpact DECIMAL(18,2)   NOT NULL,
    relatedfactor   INT             NULL,
    isresolved      BOOLEAN         NOT NULL DEFAULT FALSE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS dashboardsnapshots (
    id                          UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid                    UUID            NOT NULL REFERENCES tenants(id),
    revenuetoday                DECIMAL(18,2)   NOT NULL,
    revenuemonth                DECIMAL(18,2)   NOT NULL,
    currentprofit               DECIMAL(18,2)   NOT NULL,
    profitatrisk                DECIMAL(18,2)   NOT NULL,
    opportunityamount           DECIMAL(18,2)   NOT NULL,
    companyhealthpercent        INT             NOT NULL,
    criticalbottlenecks         INT             NOT NULL,
    delinquencyamount           DECIMAL(18,2)   NOT NULL,
    conversionrate              DECIMAL(8,4)    NOT NULL,
    monthgoal                   DECIMAL(18,2)   NOT NULL,
    monthgoalpercent            DECIMAL(8,2)    NOT NULL,
    forecastresult30days        DECIMAL(18,2)   NOT NULL,
    forecastgoal30days          DECIMAL(18,2)   NOT NULL,
    forecastbelowgoalpercent    DECIMAL(8,2)    NOT NULL,
    cashflowreceivable60days    DECIMAL(18,2)   NOT NULL,
    cashflowatrisk60days        DECIMAL(18,2)   NOT NULL,
    cashflowoverdue60days       DECIMAL(18,2)   NOT NULL,
    cashflowhealthypercent      INT             NOT NULL,
    payloadjson                 TEXT            NOT NULL DEFAULT '{}',
    createdatutc                TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc                TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS receivables (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    clientid        UUID            NULL,
    clientname      VARCHAR(200)    NOT NULL,
    amount          DECIMAL(18,2)   NOT NULL,
    dueatutc        TIMESTAMPTZ     NOT NULL,
    status          VARCHAR(50)     NOT NULL DEFAULT 'AReceber',
    isactive        BOOLEAN         NOT NULL DEFAULT TRUE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS actionplanitems (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    title           VARCHAR(300)    NOT NULL,
    rationale       VARCHAR(1000)   NOT NULL,
    expectedimpact  DECIMAL(18,2)   NOT NULL,
    priority        INT             NOT NULL,
    isdone          BOOLEAN         NOT NULL DEFAULT FALSE,
    isactive        BOOLEAN         NOT NULL DEFAULT TRUE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE TABLE IF NOT EXISTS smartinsights (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    type            VARCHAR(50)     NOT NULL,
    title           VARCHAR(300)    NOT NULL,
    description     VARCHAR(1000)   NOT NULL,
    buttontext      VARCHAR(100)    NOT NULL,
    actionroute     VARCHAR(200)    NOT NULL,
    financialimpact DECIMAL(18,2)   NULL,
    isactive        BOOLEAN         NOT NULL DEFAULT TRUE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

-- Relacionamentos da tela Radar de Lucro (vendas, inadimplência e fluxo de caixa → cliente)
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_sales_clients') THEN
        ALTER TABLE sales
            ADD CONSTRAINT fk_sales_clients FOREIGN KEY (clientid) REFERENCES clients(id);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_delinquencyrecords_clients') THEN
        ALTER TABLE delinquencyrecords
            ADD CONSTRAINT fk_delinquencyrecords_clients FOREIGN KEY (clientid) REFERENCES clients(id);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_receivables_clients') THEN
        ALTER TABLE receivables
            ADD CONSTRAINT fk_receivables_clients FOREIGN KEY (clientid) REFERENCES clients(id);
    END IF;
END $$;
