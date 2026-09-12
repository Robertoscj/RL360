using Microsoft.Extensions.Logging;
using RL360.Application.Abstractions;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface IProcessadorDashboardServico
{
    Task ProcessarEmpresaAsync(Guid idEmpresa, CancellationToken ct = default);
}

public sealed class ProcessadorDashboardServico(
    IRadarServico radar,
    IDashboardServico dashboard,
    ISnapshotDashboardServico snapshots,
    IDashboardNotificador notificador,
    IModulosServico modulos,
    GeradorAlertasServico geradorAlertas,
    ILogger<ProcessadorDashboardServico> logger) : IProcessadorDashboardServico
{
    public async Task ProcessarEmpresaAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        await snapshots.InvalidarAsync(idEmpresa, ct);

        var radarDto = await radar.RecalcularAsync(idEmpresa, ct);
        var novosAlertas = await geradorAlertas.GerarSeNecessarioAsync(idEmpresa, radarDto, ct);

        var resumo = await dashboard.ObterResumoAsync(idEmpresa, forcarAtualizacao: true, ct: ct);
        await snapshots.PersistirAsync(idEmpresa, resumo, ct);

        var faturamento = await modulos.ObterFaturamentoAsync(idEmpresa, ct: ct);
        var plano = resumo.Rodape.PlanoAcao;

        await notificador.NotificarRadarAsync(radarDto, ct);
        await notificador.NotificarDashboardAsync(resumo, ct);
        await notificador.NotificarFaturamentoAsync(idEmpresa, faturamento, ct);
        await notificador.NotificarPlanoAcaoAsync(idEmpresa, plano, ct);

        foreach (var alerta in novosAlertas)
            await notificador.NotificarAlertaAsync(idEmpresa, alerta, ct);

        logger.LogInformation(
            "Pipeline concluído para empresa {IdEmpresa} — {Alertas} alerta(s) novo(s), saúde {Saude}%",
            idEmpresa, novosAlertas.Count, radarDto.SaudeEmpresaPercentual);
    }
}
