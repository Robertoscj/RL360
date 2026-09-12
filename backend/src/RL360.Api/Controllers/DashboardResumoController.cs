using Microsoft.AspNetCore.Mvc;
using RL360.Api.Extensions;
using RL360.Application.Abstractions;
using RL360.Application.Services;

namespace RL360.Api.Controllers;

[Route("api/dashboard")]
public sealed class DashboardResumoController(
    IUsuarioAtual usuarioAtual,
    IDashboardServico dashboard) : ControladorBase(usuarioAtual)
{
    [HttpGet("summary")]
    public async Task<IActionResult> Resumo(
        [FromQuery] bool atualizar = false,
        [FromQuery] DateOnly? inicio = null,
        [FromQuery] DateOnly? fim = null,
        CancellationToken ct = default)
        => OkComDados(await dashboard.ObterResumoAsync(ExigirIdEmpresa(), atualizar, inicio, fim, ct));

    [HttpGet("radar")]
    public async Task<IActionResult> Radar(CancellationToken ct = default)
        => OkComDados(await dashboard.ObterRadarAsync(ExigirIdEmpresa(), ct));

    [HttpGet("revenue")]
    public async Task<IActionResult> Faturamento(CancellationToken ct = default)
        => OkComDados(await dashboard.ObterFaturamentoAsync(ExigirIdEmpresa(), ct));

    [HttpGet("alerts")]
    public async Task<IActionResult> Alertas(CancellationToken ct = default)
        => OkComDados(await dashboard.ObterAlertasAsync(ExigirIdEmpresa(), ct));

    [HttpGet("action-plan")]
    public async Task<IActionResult> PlanoAcao(CancellationToken ct = default)
        => OkComDados(await dashboard.ObterPlanoAcaoAsync(ExigirIdEmpresa(), ct));
}
