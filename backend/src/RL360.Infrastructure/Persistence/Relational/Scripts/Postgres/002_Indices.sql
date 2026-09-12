-- RL360 — PostgreSQL: Script 002 (índices)

CREATE INDEX IF NOT EXISTS ix_users_tenantid ON users(tenantid);
CREATE INDEX IF NOT EXISTS ix_revenuesnapshots_tenantid ON revenuesnapshots(tenantid);
CREATE INDEX IF NOT EXISTS ix_sales_tenantid ON sales(tenantid);
CREATE INDEX IF NOT EXISTS ix_funnelstages_tenantid ON funnelstages(tenantid);
CREATE INDEX IF NOT EXISTS ix_clients_tenantid ON clients(tenantid);
CREATE INDEX IF NOT EXISTS ix_delinquencyrecords_tenantid ON delinquencyrecords(tenantid);
CREATE INDEX IF NOT EXISTS ix_bottlenecks_tenantid ON bottlenecks(tenantid);
CREATE INDEX IF NOT EXISTS ix_teammembers_tenantid ON teammembers(tenantid);
CREATE INDEX IF NOT EXISTS ix_goals_tenantid ON goals(tenantid);
CREATE INDEX IF NOT EXISTS ix_alerts_tenantid ON alerts(tenantid);
CREATE INDEX IF NOT EXISTS ix_dashboardsnapshots_tenantid ON dashboardsnapshots(tenantid);
CREATE INDEX IF NOT EXISTS ix_receivables_tenantid ON receivables(tenantid);
CREATE INDEX IF NOT EXISTS ix_actionplanitems_tenantid ON actionplanitems(tenantid);
CREATE INDEX IF NOT EXISTS ix_smartinsights_tenantid ON smartinsights(tenantid);

CREATE INDEX IF NOT EXISTS ix_revenuesnapshots_createdat ON revenuesnapshots(createdatutc DESC);
CREATE INDEX IF NOT EXISTS ix_dashboardsnapshots_createdat ON dashboardsnapshots(createdatutc DESC);
CREATE INDEX IF NOT EXISTS ix_alerts_createdat ON alerts(createdatutc DESC);

CREATE INDEX IF NOT EXISTS ix_dashboardsnapshots_tenant_created
    ON dashboardsnapshots(tenantid, createdatutc DESC);

CREATE INDEX IF NOT EXISTS ix_revenuesnapshots_tenant_created
    ON revenuesnapshots(tenantid, createdatutc DESC);

CREATE INDEX IF NOT EXISTS ix_alerts_tenant_resolved
    ON alerts(tenantid, isresolved) WHERE isresolved = FALSE;

CREATE INDEX IF NOT EXISTS ix_clients_tenant_status
    ON clients(tenantid, status);

CREATE INDEX IF NOT EXISTS ix_receivables_tenant_status
    ON receivables(tenantid, status) WHERE isactive = TRUE;

CREATE INDEX IF NOT EXISTS ix_receivables_tenant_client
    ON receivables(tenantid, clientid);

CREATE INDEX IF NOT EXISTS ix_delinquencyrecords_tenant_client
    ON delinquencyrecords(tenantid, clientid);

CREATE INDEX IF NOT EXISTS ix_sales_tenant_client
    ON sales(tenantid, clientid);

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_email ON users(email);
