using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RL360.Application.Abstractions;
using RL360.Shared.Constants;
using RL360.Shared.Dtos;

namespace RL360.Api.Realtime;

[Authorize]
public sealed class HubDashboard(IUsuarioAtual usuarioAtual) : Hub
{
    public static string GrupoPara(Guid idEmpresa) => $"empresa:{idEmpresa}";

    public override async Task OnConnectedAsync()
    {
        if (usuarioAtual.IdEmpresa is { } idEmpresa)
            await Groups.AddToGroupAsync(Context.ConnectionId, GrupoPara(idEmpresa));

        await base.OnConnectedAsync();
    }

    public Task RepassarDashboard(ResumoDashboardDto resumo)
        => Clients.Group(GrupoPara(resumo.IdEmpresa))
            .SendAsync(CanaisTempoReal.EventoDashboardAtualizado, resumo);

    public Task RepassarRadar(SnapshotRadarDto snapshot)
        => Clients.Group(GrupoPara(snapshot.IdEmpresa))
            .SendAsync(CanaisTempoReal.EventoRadarAtualizado, snapshot);

    public Task RepassarFaturamento(Guid idEmpresa, FaturamentoDto faturamento)
        => Clients.Group(GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoFaturamentoAtualizado, faturamento);

    public Task RepassarAlerta(Guid idEmpresa, AlertaDto alerta)
        => Clients.Group(GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoAlertaCriado, alerta);

    public Task RepassarGargalo(Guid idEmpresa, GargaloDto gargalo)
        => Clients.Group(GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoGargaloDetectado, gargalo);

    public Task RepassarPlanoAcao(Guid idEmpresa, IReadOnlyList<ItemPlanoAcaoDto> plano)
        => Clients.Group(GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoPlanoAcaoAtualizado, plano);
}

public sealed class NotificadorDashboardHub(IHubContext<HubDashboard> hub) : IRadarNotificador
{
    public Task NotificarDashboardAsync(ResumoDashboardDto resumo, CancellationToken ct = default)
        => hub.Clients.Group(HubDashboard.GrupoPara(resumo.IdEmpresa))
            .SendAsync(CanaisTempoReal.EventoDashboardAtualizado, resumo, ct);

    public Task NotificarRadarAsync(SnapshotRadarDto snapshot, CancellationToken ct = default)
        => hub.Clients.Group(HubDashboard.GrupoPara(snapshot.IdEmpresa))
            .SendAsync(CanaisTempoReal.EventoRadarAtualizado, snapshot, ct);

    public Task NotificarFaturamentoAsync(Guid idEmpresa, FaturamentoDto faturamento, CancellationToken ct = default)
        => hub.Clients.Group(HubDashboard.GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoFaturamentoAtualizado, faturamento, ct);

    public Task NotificarAlertaAsync(Guid idEmpresa, AlertaDto alerta, CancellationToken ct = default)
        => hub.Clients.Group(HubDashboard.GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoAlertaCriado, alerta, ct);

    public Task NotificarGargaloAsync(Guid idEmpresa, GargaloDto gargalo, CancellationToken ct = default)
        => hub.Clients.Group(HubDashboard.GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoGargaloDetectado, gargalo, ct);

    public Task NotificarPlanoAcaoAsync(Guid idEmpresa, IReadOnlyList<ItemPlanoAcaoDto> plano, CancellationToken ct = default)
        => hub.Clients.Group(HubDashboard.GrupoPara(idEmpresa))
            .SendAsync(CanaisTempoReal.EventoPlanoAcaoAtualizado, plano, ct);
}
