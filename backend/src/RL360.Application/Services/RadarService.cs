using AutoMapper;
using Microsoft.Extensions.Logging;
using RL360.Application.Abstractions;
using RL360.Domain.Services;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface IRadarServico
{
    Task<SnapshotRadarDto> ObterSnapshotAsync(Guid idEmpresa, bool forcarAtualizacao = false, CancellationToken ct = default);
    Task<SnapshotRadarDto> RecalcularAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<SnapshotRadarDto> RecalcularENotificarAsync(Guid idEmpresa, CancellationToken ct = default);
}

public sealed class RadarServico(
    MontadorEntradasRadar montador,
    CalculadoraRadar calculadora,
    ICacheServico cache,
    IMapper mapper,
    IDashboardNotificador notificador,
    ILogger<RadarServico> logger) : IRadarServico
{
    private static string ChaveCache(Guid idEmpresa) => $"rl360:radar:{idEmpresa}";
    private static readonly TimeSpan TtlCache = TimeSpan.FromSeconds(30);

    public async Task<SnapshotRadarDto> ObterSnapshotAsync(Guid idEmpresa, bool forcarAtualizacao = false, CancellationToken ct = default)
    {
        if (!forcarAtualizacao)
        {
            var emCache = await cache.ObterAsync<SnapshotRadarDto>(ChaveCache(idEmpresa), ct);
            if (emCache is not null)
            {
                logger.LogDebug("Snapshot do radar servido do cache para empresa {IdEmpresa}", idEmpresa);
                return emCache;
            }
        }

        var dto = await CalcularAsync(idEmpresa, ct);
        await cache.DefinirAsync(ChaveCache(idEmpresa), dto, TtlCache, ct);
        return dto;
    }

    public async Task<SnapshotRadarDto> RecalcularAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        var dto = await CalcularAsync(idEmpresa, ct);
        await cache.DefinirAsync(ChaveCache(idEmpresa), dto, TtlCache, ct);
        logger.LogInformation("Radar recalculado para empresa {IdEmpresa} (saúde {Saude}%)",
            idEmpresa, dto.SaudeEmpresaPercentual);
        return dto;
    }

    public async Task<SnapshotRadarDto> RecalcularENotificarAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        var dto = await RecalcularAsync(idEmpresa, ct);
        await notificador.NotificarRadarAsync(dto, ct);
        return dto;
    }

    private async Task<SnapshotRadarDto> CalcularAsync(Guid idEmpresa, CancellationToken ct)
    {
        var entradas = await montador.MontarAsync(idEmpresa, ct);
        var snapshot = calculadora.Calcular(entradas);
        return mapper.Map<SnapshotRadarDto>(snapshot);
    }
}
