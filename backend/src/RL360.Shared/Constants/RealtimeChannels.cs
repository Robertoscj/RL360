namespace RL360.Shared.Constants;

public static class CanaisTempoReal
{
    public const string CaminhoHubDashboard = "/hubs/dashboard";

    /// <summary>Alias retrocompatível — aponta para o mesmo hub.</summary>
    public const string CaminhoHubRadar = CaminhoHubDashboard;

    public const string EventoDashboardAtualizado = "dashboard:atualizado";
    public const string EventoFaturamentoAtualizado = "faturamento:atualizado";
    public const string EventoRadarAtualizado = "radar:atualizado";
    public const string EventoAlertaCriado = "alerta:criado";
    public const string EventoGargaloDetectado = "gargalo:detectado";
    public const string EventoPlanoAcaoAtualizado = "plano-acao:atualizado";
}

public static class Filas
{
    public const string RecalculoRadar = "rl360.radar.recalcular";
    public const string ExchangeRadar = "rl360.radar";
}

