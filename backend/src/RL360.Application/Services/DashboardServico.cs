using AutoMapper;
using Microsoft.Extensions.Logging;
using RL360.Application.Abstractions;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface IDashboardServico
{
    Task<ResumoDashboardDto> ObterResumoAsync(
        Guid idEmpresa,
        bool forcarAtualizacao = false,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken ct = default);
    Task<SnapshotRadarDto> ObterRadarAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<FaturamentoDto> ObterFaturamentoAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<AlertaDto>> ObterAlertasAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<ItemPlanoAcaoDto>> ObterPlanoAcaoAsync(Guid idEmpresa, CancellationToken ct = default);
}

public sealed class DashboardServico(
    ISnapshotDashboardRepositorio snapshots,
    ISnapshotDashboardServico snapshotServico,
    IRadarServico radar,
    IModulosServico modulos,
    IPlanoAcaoRepositorio planoAcao,
    IInsightRepositorio insights,
    IEmpresaRepositorio empresas,
    ICacheServico cache,
    IMapper mapper,
    ILogger<DashboardServico> _) : IDashboardServico
{
    private static string ChaveResumo(Guid id, DateOnly? inicio, DateOnly? fim)
        => $"rl360:dashboard:resumo:{id}:{inicio?.ToString("yyyy-MM-dd") ?? "padrao"}:{fim?.ToString("yyyy-MM-dd") ?? "padrao"}";
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(30);

    public async Task<ResumoDashboardDto> ObterResumoAsync(
        Guid idEmpresa,
        bool forcarAtualizacao = false,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken ct = default)
    {
        var chave = ChaveResumo(idEmpresa, inicio, fim);
        if (!forcarAtualizacao)
        {
            var emCache = await cache.ObterAsync<ResumoDashboardDto>(chave, ct);
            if (emCache is not null) return emCache;

            var doSql = await snapshotServico.TentarObterDoSqlAsync(idEmpresa, ct);
            if (doSql is not null)
            {
                await cache.DefinirAsync(chave, doSql, Ttl, ct);
                return doSql;
            }
        }

        var resumo = await MontarResumoAsync(idEmpresa, inicio, fim, forcarAtualizacao, ct);
        await cache.DefinirAsync(chave, resumo, Ttl, ct);
        return resumo;
    }

    public Task<SnapshotRadarDto> ObterRadarAsync(Guid idEmpresa, CancellationToken ct = default)
        => radar.ObterSnapshotAsync(idEmpresa, ct: ct);

    public Task<FaturamentoDto> ObterFaturamentoAsync(Guid idEmpresa, CancellationToken ct = default)
        => modulos.ObterFaturamentoAsync(idEmpresa, ct);

    public Task<IReadOnlyList<AlertaDto>> ObterAlertasAsync(Guid idEmpresa, CancellationToken ct = default)
        => modulos.ObterAlertasAsync(idEmpresa, ct);

    public async Task<IReadOnlyList<ItemPlanoAcaoDto>> ObterPlanoAcaoAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        var radarDto = await radar.ObterSnapshotAsync(idEmpresa, ct: ct);
        if (radarDto.PlanoAcao.Count > 0)
            return radarDto.PlanoAcao;

        var itens = await planoAcao.ObterAtivosAsync(idEmpresa, ct);
        return itens.Select(i => new ItemPlanoAcaoDto
        {
            Titulo = i.Titulo,
            Justificativa = i.Justificativa,
            ImpactoEsperado = i.ImpactoEsperado,
            Prioridade = i.Prioridade.ToString()
        }).ToList();
    }

    private async Task<ResumoDashboardDto> MontarResumoAsync(
        Guid idEmpresa,
        DateOnly? inicio,
        DateOnly? fim,
        bool forcarAtualizacao,
        CancellationToken ct)
    {
        var diasPeriodo = ObterDiasPeriodo(inicio, fim);
        var empresa = await empresas.ObterPorIdAsync(idEmpresa, ct);
        var snapshotDb = await snapshots.ObterMaisRecenteAsync(idEmpresa, ct);
        var radarDto = await radar.ObterSnapshotAsync(idEmpresa, forcarAtualizacao, ct);
        var insightsDb = await insights.ObterAtivosAsync(idEmpresa, ct);
        var alertas = await modulos.ObterAlertasAsync(idEmpresa, ct);
        var plano = await ObterPlanoAcaoAsync(idEmpresa, ct);

        // Valores do snapshot persistido têm prioridade sobre cálculo dinâmico (mock visual).
        var lucroAtual = snapshotDb?.LucroAtual ?? radarDto.LucroAtual;
        if (snapshotDb is not null)
        {
            radarDto.LucroAtual = lucroAtual;
            radarDto.RiscoProximos30Dias = snapshotDb.LucroEmRisco;
            radarDto.ValorOportunidade = snapshotDb.ValorOportunidade;
            radarDto.GargalosCriticos = snapshotDb.GargalosCriticos;
            radarDto.SaudeEmpresaPercentual = snapshotDb.SaudeEmpresaPercentual;
        }

        var previsao = new PrevisaoResultadoDto
        {
            Dias = diasPeriodo,
            CenarioMaisProvavel = snapshotDb?.PrevisaoResultado30Dias ?? 5_712_000m,
            Meta = snapshotDb?.MetaResultado30Dias ?? 6_650_000m,
            PercentualAbaixoMeta = snapshotDb?.PercentualAbaixoMetaPrevisao ?? 14m,
            Serie = GerarSeriePrevisao(
                snapshotDb?.PrevisaoResultado30Dias ?? 5_712_000m,
                snapshotDb?.MetaResultado30Dias ?? 6_650_000m,
                inicio,
                fim)
        };

        var fluxo = new FluxoCaixaFuturoDto
        {
            Dias = 60,
            AReceber = snapshotDb?.FluxoAReceber60Dias ?? 3_842_000m,
            EmRisco = snapshotDb?.FluxoEmRisco60Dias ?? 642_000m,
            Atrasado = snapshotDb?.FluxoAtrasado60Dias ?? 285_000m,
            PercentualSaudavel = snapshotDb?.PercentualFluxoSaudavel ?? 78,
            Segmentos =
            [
                new SegmentoDonutDto { Rotulo = "Saudável", Valor = snapshotDb?.FluxoAReceber60Dias ?? 3_842_000m, Cor = "#22c55e" },
                new SegmentoDonutDto { Rotulo = "Em risco", Valor = snapshotDb?.FluxoEmRisco60Dias ?? 642_000m, Cor = "#f97316" },
                new SegmentoDonutDto { Rotulo = "Atrasado", Valor = snapshotDb?.FluxoAtrasado60Dias ?? 285_000m, Cor = "#ef4444" }
            ]
        };

        return new ResumoDashboardDto
        {
            IdEmpresa = idEmpresa,
            NomeEmpresa = empresa?.Nome ?? radarDto.NomeEmpresa,
            GeradoEmUtc = DateTime.UtcNow,
            CardsTopo = new CardsTopoDto
            {
                RiscoProximos30Dias = radarDto.RiscoProximos30Dias,
                ValorOportunidade = radarDto.ValorOportunidade,
                GargalosCriticos = radarDto.GargalosCriticos,
                SaudeEmpresaPercentual = radarDto.SaudeEmpresaPercentual,
                StatusSaude = radarDto.StatusSaude
            },
            Radar = radarDto,
            PrevisaoResultado = previsao,
            FluxoCaixaFuturo = fluxo,
            Insights = mapper.Map<List<InsightInteligenteDto>>(insightsDb),
            AlertasCriticos = alertas.Select(a => new AlertaCriticoDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Tipo = a.ImpactoFinanceiro >= 0 ? "Positivo" : "Negativo",
                ImpactoFinanceiro = a.ImpactoFinanceiro,
                Severidade = a.Severidade
            }).ToList(),
            Rodape = new RodapeDashboardDto
            {
                QuantidadeAcoesPlano = plano.Count,
                PlanoAcao = plano.ToList()
            }
        };
    }

    private static int ObterDiasPeriodo(DateOnly? inicio, DateOnly? fim)
    {
        if (inicio is null || fim is null) return 30;
        return Math.Max(1, fim.Value.DayNumber - inicio.Value.DayNumber + 1);
    }

    private static List<PontoSerieTemporalDto> GerarSeriePrevisao(
        decimal estimado,
        decimal meta,
        DateOnly? inicio,
        DateOnly? fim)
    {
        var dias = ObterDiasPeriodo(inicio, fim);
        var dataInicio = inicio?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow.Date;
        return Enumerable.Range(0, dias)
            .Select(i => new PontoSerieTemporalDto
            {
                Rotulo = dataInicio.AddDays(i).ToString("dd/MM"),
                Valor = Math.Round(estimado / dias * (0.85m + i * 0.01m), 0),
                Meta = Math.Round(meta / dias, 0)
            })
            .ToList();
    }
}
