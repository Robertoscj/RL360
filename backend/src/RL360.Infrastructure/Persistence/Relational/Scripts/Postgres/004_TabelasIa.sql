-- RL360 — PostgreSQL: Script 004 (tabelas IA)

CREATE TABLE IF NOT EXISTS aichatmessages (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    userid          UUID            NOT NULL,
    conversationid  UUID            NOT NULL,
    role            VARCHAR(20)     NOT NULL,
    content         TEXT            NOT NULL,
    mode            VARCHAR(30)     NOT NULL DEFAULT 'Demo',
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc')
);

CREATE TABLE IF NOT EXISTS knowledgedocuments (
    id              UUID            NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    tenantid        UUID            NOT NULL REFERENCES tenants(id),
    title           VARCHAR(300)    NOT NULL,
    category        VARCHAR(100)    NOT NULL,
    content         TEXT            NOT NULL,
    isactive        BOOLEAN         NOT NULL DEFAULT TRUE,
    createdatutc    TIMESTAMPTZ     NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
    updatedatutc    TIMESTAMPTZ     NULL
);

CREATE INDEX IF NOT EXISTS ix_aichatmessages_tenantuser
    ON aichatmessages(tenantid, userid, createdatutc DESC);
