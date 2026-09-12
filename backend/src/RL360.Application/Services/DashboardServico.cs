using AutoMapper;
using Microsoft.Extensions.Logging;
using RL360.Application.Abstractions;
using RL360.Application.Services.Ia;
using RL360.Domain.Entities;
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
    IContaReceberRepositorio contasReceber,
    IFaturamentoRepositorio faturamento,
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
            if (emCache is not null) return ComInsightsVivos(emCache);

            var doSql = await snapshotServico.TentarObterDoSqlAsync(idEmpresa, ct);
            if (doSql is not null)
            {
                var vivo = ComInsightsVivos(doSql);
                await cache.DefinirAsync(chave, vivo, Ttl, ct);
                return vivo;
            }
        }

        var resumo = ComInsightsVivos(await MontarResumoAsync(idEmpresa, inicio, fim, forcarAtualizacao, ct));
        await cache.DefinirAsync(chave, resumo, Ttl, ct);
        return resumo;
    }

    public Task<SnapshotRadarDto> ObterRadarAsync(Guid idEmpresa, CancellationToken ct = default)
        => radar.ObterSnapshotAsync(idEmpresa, ct: ct);

    public Task<FaturamentoDto> ObterFaturamentoAsync(Guid idEmpresa, CancellationToken ct = default)
        => modulos.ObterFaturamentoAsync(idEmpresa, ct: ct);

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
        var fatRecente = await faturamento.ObterMaisRecenteAsync(idEmpresa, ct);
        var insightsDb = await insights.ObterAtivosAsync(idEmpresa, ct);
        var alertas = await modulos.ObterAlertasAsync(idEmpresa, ct);
        var plano = await ObterPlanoAcaoAsync(idEmpresa, ct);
        var fluxo = await MontarFluxoAsync(idEmpresa, snapshotDb, ct);
        var previsao = MontarPrevisao(diasPeriodo, inicio, fim, snapshotDb, fatRecente);

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

    private async Task<FluxoCaixaFuturoDto> MontarFluxoAsync(
        Guid idEmpresa,
        SnapshotDashboard? snapshotDb,
        CancellationToken ct)
    {
        var contas = await contasReceber.ObterProximos60DiasAsync(idEmpresa, ct);
        var aReceber = contas.Where(c => c.Status == "AReceber").Sum(c => c.Valor);
        var emRisco = contas.Where(c => c.Status == "EmRisco").Sum(c => c.Valor);
        var atrasado = contas.Where(c => c.Status == "Atrasado").Sum(c => c.Valor);
        var total = aReceber + emRisco + atrasado;

        if (total <= 0 && snapshotDb is not null)
        {
            aReceber = snapshotDb.FluxoAReceber60Dias;
            emRisco = snapshotDb.FluxoEmRisco60Dias;
            atrasado = snapshotDb.FluxoAtrasado60Dias;
            total = aReceber + emRisco + atrasado;
        }

        var percentual = total <= 0 ? 0 : (int)Math.Round(aReceber / total * 100m);

        return new FluxoCaixaFuturoDto
        {
            Dias = 60,
            AReceber = aReceber,
            EmRisco = emRisco,
            Atrasado = atrasado,
            PercentualSaudavel = percentual,
            Segmentos =
            [
                new SegmentoDonutDto { Rotulo = "Saudável", Valor = aReceber, Cor = "#22c55e" },
                new SegmentoDonutDto { Rotulo = "Em risco", Valor = emRisco, Cor = "#f97316" },
                new SegmentoDonutDto { Rotulo = "Atrasado", Valor = atrasado, Cor = "#ef4444" }
            ]
        };
    }

    private static PrevisaoResultadoDto MontarPrevisao(
        int diasPeriodo,
        DateOnly? inicio,
        DateOnly? fim,
        SnapshotDashboard? snapshotDb,
        SnapshotFaturamento? fatRecente)
    {
        var cenario = snapshotDb?.PrevisaoResultado30Dias
                      ?? (fatRecente is null ? 0 : fatRecente.FaturamentoDia * diasPeriodo);
        var meta = snapshotDb?.MetaResultado30Dias
                   ?? (fatRecente is null || fatRecente.MetaMensal <= 0
                       ? 0
                       : fatRecente.MetaMensal / DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month) * diasPeriodo);
        var abaixo = snapshotDb?.PercentualAbaixoMetaPrevisao
                     ?? (meta <= 0 ? 0 : Math.Round((meta - cenario) / meta * 100m, 1));

        return new PrevisaoResultadoDto
        {
            Dias = diasPeriodo,
            CenarioMaisProvavel = cenario,
            Meta = meta,
            PercentualAbaixoMeta = abaixo,
            Serie = GerarSeriePrevisao(cenario, meta, inicio, fim)
        };
    }

    private static ResumoDashboardDto ComInsightsVivos(ResumoDashboardDto resumo)
    {
        resumo.Insights = GeradorInsightsPainel.Montar(resumo);
        return resumo;
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
