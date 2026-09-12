using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface ISnapshotDashboardServico
{
    Task<ResumoDashboardDto?> TentarObterDoSqlAsync(Guid idEmpresa, CancellationToken ct = default);
    Task PersistirAsync(Guid idEmpresa, ResumoDashboardDto resumo, CancellationToken ct = default);
    Task InvalidarAsync(Guid idEmpresa, CancellationToken ct = default);
}

public sealed class SnapshotDashboardServico(
    ISnapshotDashboardRepositorio repositorio,
    ICacheServico cache,
    ILogger<SnapshotDashboardServico> logger) : ISnapshotDashboardServico
{
    private static string ChaveResumo(Guid idEmpresa) => $"rl360:dashboard:resumo:{idEmpresa}";
    private static readonly TimeSpan TtlCache = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<ResumoDashboardDto?> TentarObterDoSqlAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        var snapshot = await repositorio.ObterMaisRecenteAsync(idEmpresa, ct);
        if (snapshot is null) return null;

        if (!string.IsNullOrWhiteSpace(snapshot.PayloadJson) && snapshot.PayloadJson != "{}")
        {
            try
            {
                var resumo = JsonSerializer.Deserialize<ResumoDashboardDto>(snapshot.PayloadJson, JsonOpts);
                if (resumo is not null)
                {
                    logger.LogDebug("Resumo do dashboard carregado do PayloadJson SQL para empresa {IdEmpresa}", idEmpresa);
                    return resumo;
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "PayloadJson inválido no snapshot da empresa {IdEmpresa}", idEmpresa);
            }
        }

        return null;
    }

    public async Task PersistirAsync(Guid idEmpresa, ResumoDashboardDto resumo, CancellationToken ct = default)
    {
        var payload = JsonSerializer.Serialize(resumo, JsonOpts);
        var radar = resumo.Radar;
        var previsao = resumo.PrevisaoResultado;
        var fluxo = resumo.FluxoCaixaFuturo;

        var snapshot = new SnapshotDashboard(
            idEmpresa,
            radar.FaturamentoDia,
            radar.FaturamentoMes,
            radar.LucroAtual,
            radar.RiscoProximos30Dias,
            radar.ValorOportunidade,
            radar.SaudeEmpresaPercentual,
            radar.GargalosCriticos,
            radar.InadimplenciaAtual,
            radar.TaxaConversaoAtual,
            radar.MetaMensal,
            radar.PercentualMetaAtingida,
            previsao.CenarioMaisProvavel,
            previsao.Meta,
            previsao.PercentualAbaixoMeta,
            fluxo.AReceber,
            fluxo.EmRisco,
            fluxo.Atrasado,
            fluxo.PercentualSaudavel,
            payload);

        await repositorio.SalvarAsync(snapshot, ct);
        await cache.DefinirAsync(ChaveResumo(idEmpresa), resumo, TtlCache, ct);

        logger.LogInformation(
            "Snapshot persistido (Redis + SQL) para empresa {IdEmpresa} — lucro {Lucro}, saúde {Saude}%",
            idEmpresa, radar.LucroAtual, radar.SaudeEmpresaPercentual);
    }

    public async Task InvalidarAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        await cache.RemoverAsync(ChaveResumo(idEmpresa), ct);
        await cache.RemoverAsync($"rl360:radar:{idEmpresa}", ct);
    }
}
