-- RL360 — PostgreSQL: Script 003 (seed demo)

DO $$
DECLARE
    tenant_id UUID := '11111111-1111-1111-1111-111111111111';
    user_id   UUID := '22222222-2222-2222-2222-222222222222';
BEGIN
    IF NOT EXISTS (SELECT 1 FROM tenants WHERE id = tenant_id) THEN
        INSERT INTO tenants (id, name, document, segment, plancode, isactive)
        VALUES (tenant_id, 'Grupo Aurora Distribuição', '12.345.678/0001-90', 'Distribuição', 'scale', TRUE);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE id = user_id) THEN
        INSERT INTO users (id, tenantid, name, email, passwordhash, role, isactive)
        VALUES (user_id, tenant_id, 'Roberto Silva', 'ceo@rl360.com',
                '$2a$11$placeholder-substituir-pelo-hash-real', 0, TRUE);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM dashboardsnapshots WHERE tenantid = tenant_id) THEN
        INSERT INTO dashboardsnapshots (
            id, tenantid, revenuetoday, revenuemonth, currentprofit, profitatrisk,
            opportunityamount, companyhealthpercent, criticalbottlenecks, delinquencyamount,
            conversionrate, monthgoal, monthgoalpercent, forecastresult30days, forecastgoal30days,
            forecastbelowgoalpercent, cashflowreceivable60days, cashflowatrisk60days,
            cashflowoverdue60days, cashflowhealthypercent, payloadjson)
        VALUES (
            gen_random_uuid(), tenant_id,
            58420, 1250000, 1247850, 487320,
            214500, 78, 4, 312000,
            0.22, 1500000, 83.3,
            5712000, 6650000, 14,
            3842000, 642000, 285000, 78, '{}');
    END IF;
END $$;
