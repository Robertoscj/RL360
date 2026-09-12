using Microsoft.AspNetCore.Mvc;
using RL360.Application.Abstractions;
using RL360.Application.Services;

namespace RL360.Api.Controllers;

[Route("api/radar")]
public sealed class RadarController(
    IUsuarioAtual usuarioAtual,
    IRadarServico radar,
    IProcessadorDashboardServico processador,
    IEventoPublicador eventos) : ControladorBase(usuarioAtual)
{
    [HttpGet]
    public async Task<IActionResult> Obter([FromQuery] bool atualizar = false, CancellationToken ct = default)
        => Ok(await radar.ObterSnapshotAsync(ExigirIdEmpresa(), atualizar, ct));

    [HttpPost("recalcular")]
    public async Task<IActionResult> Recalcular(CancellationToken ct)
    {
        var idEmpresa = ExigirIdEmpresa();
        await eventos.PublicarRecalculoRadarAsync(idEmpresa, ct);
        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
        var snapshot = await radar.ObterSnapshotAsync(idEmpresa, forcarAtualizacao: true, ct);
        return Ok(snapshot);
    }
}
