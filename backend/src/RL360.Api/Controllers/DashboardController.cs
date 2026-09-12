using Microsoft.AspNetCore.Mvc;
using RL360.Application.Abstractions;
using RL360.Application.Services;

namespace RL360.Api.Controllers;

[Route("api")]
public sealed class DashboardController(
    IUsuarioAtual usuarioAtual,
    IModulosServico modulos) : ControladorBase(usuarioAtual)
{
    [HttpGet("faturamento")]
    public async Task<IActionResult> Faturamento(CancellationToken ct)
        => Ok(await modulos.ObterFaturamentoAsync(ExigirIdEmpresa(), ct));

    [HttpGet("vendas")]
    public async Task<IActionResult> Vendas(CancellationToken ct)
        => Ok(await modulos.ObterVendasAsync(ExigirIdEmpresa(), ct));

    [HttpGet("funil")]
    public async Task<IActionResult> Funil(CancellationToken ct)
        => Ok(await modulos.ObterFunilAsync(ExigirIdEmpresa(), ct));

    [HttpGet("inadimplencia")]
    public async Task<IActionResult> Inadimplencia(CancellationToken ct)
        => Ok(await modulos.ObterInadimplenciaAsync(ExigirIdEmpresa(), ct));

    [HttpGet("gargalos")]
    public async Task<IActionResult> Gargalos(CancellationToken ct)
        => Ok(await modulos.ObterGargalosAsync(ExigirIdEmpresa(), ct));

    [HttpGet("clientes")]
    public async Task<IActionResult> Clientes(CancellationToken ct)
        => Ok(await modulos.ObterClientesAsync(ExigirIdEmpresa(), ct));

    [HttpGet("equipe")]
    public async Task<IActionResult> Equipe(CancellationToken ct)
        => Ok(await modulos.ObterEquipeAsync(ExigirIdEmpresa(), ct));

    [HttpGet("metas")]
    public async Task<IActionResult> Metas(CancellationToken ct)
        => Ok(await modulos.ObterMetasAsync(ExigirIdEmpresa(), ct));

    [HttpGet("alertas")]
    public async Task<IActionResult> Alertas(CancellationToken ct)
        => Ok(await modulos.ObterAlertasAsync(ExigirIdEmpresa(), ct));
}
