using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using RL360.Application.Abstractions;
using RL360.Shared.Constants;
using RL360.Shared.Dtos;

namespace RL360.Worker.Realtime;

public sealed class NotificadorDashboardClienteSignalR : IRadarNotificador, IAsyncDisposable
{
    private readonly HubConnection _conexao;
    private readonly ILogger<NotificadorDashboardClienteSignalR> _logger;
    private readonly SemaphoreSlim _semaforo = new(1, 1);

    public NotificadorDashboardClienteSignalR(
        IJwtTokenServico tokenServico, string urlBaseApi, ILogger<NotificadorDashboardClienteSignalR> logger)
    {
        _logger = logger;

        var (token, _) = tokenServico.Gerar(new UsuarioDto
        {
            Id = Guid.Empty,
            IdEmpresa = Guid.Empty,
            Nome = "RL360 Worker",
            Email = "worker@rl360.local",
            Perfil = "Service"
        });

        var urlHub = $"{urlBaseApi.TrimEnd('/')}{CanaisTempoReal.CaminhoHubDashboard}";
        _conexao = new HubConnectionBuilder()
            .WithUrl(urlHub, opts => opts.AccessTokenProvider = () => Task.FromResult<string?>(token))
            .WithAutomaticReconnect()
            .Build();
    }

    public Task NotificarDashboardAsync(ResumoDashboardDto resumo, CancellationToken ct = default)
        => InvocarHubAsync("RepassarDashboard", ct, resumo);

    public Task NotificarRadarAsync(SnapshotRadarDto snapshot, CancellationToken ct = default)
        => InvocarHubAsync("RepassarRadar", ct, snapshot);

    public Task NotificarFaturamentoAsync(Guid idEmpresa, FaturamentoDto faturamento, CancellationToken ct = default)
        => InvocarHubAsync("RepassarFaturamento", ct, idEmpresa, faturamento);

    public Task NotificarAlertaAsync(Guid idEmpresa, AlertaDto alerta, CancellationToken ct = default)
        => InvocarHubAsync("RepassarAlerta", ct, idEmpresa, alerta);

    public Task NotificarGargaloAsync(Guid idEmpresa, GargaloDto gargalo, CancellationToken ct = default)
        => InvocarHubAsync("RepassarGargalo", ct, idEmpresa, gargalo);

    public Task NotificarPlanoAcaoAsync(Guid idEmpresa, IReadOnlyList<ItemPlanoAcaoDto> plano, CancellationToken ct = default)
        => InvocarHubAsync("RepassarPlanoAcao", ct, idEmpresa, plano);

    private async Task InvocarHubAsync(string metodo, CancellationToken ct, params object?[] args)
    {
        try
        {
            await GarantirConectadoAsync(ct);
            await _conexao.InvokeAsync(metodo, ct, args);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível repassar evento {Metodo} para o hub da API", metodo);
        }
    }

    private async Task GarantirConectadoAsync(CancellationToken ct)
    {
        if (_conexao.State == HubConnectionState.Connected) return;

        await _semaforo.WaitAsync(ct);
        try
        {
            if (_conexao.State == HubConnectionState.Disconnected)
                await _conexao.StartAsync(ct);
        }
        finally
        {
            _semaforo.Release();
        }
    }

    public async ValueTask DisposeAsync() => await _conexao.DisposeAsync();
}
