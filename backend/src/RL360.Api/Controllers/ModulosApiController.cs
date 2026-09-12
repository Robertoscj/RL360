using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RL360.Application.Abstractions;
using RL360.Application.Services;
using RL360.Shared.Dtos;

namespace RL360.Api.Controllers;

/// <summary>Endpoints REST conforme especificação (/api/revenue, /api/sales, etc.).</summary>
[Route("api")]
public sealed class ModulosApiController(
    IUsuarioAtual usuarioAtual,
    IModulosServico leitura,
    IModulosEscritaServico escrita,
    IValidator<RequisicaoFaturamento> validadorFaturamento,
    IValidator<RequisicaoVenda> validadorVenda,
    IValidator<RequisicaoEtapaFunil> validadorFunil,
    IValidator<RequisicaoInadimplencia> validadorInadimplencia,
    IValidator<RequisicaoGargalo> validadorGargalo) : ControladorBase(usuarioAtual)
{
    [HttpGet("revenue")]
    public async Task<IActionResult> ObterFaturamento(
        [FromQuery] DateOnly? inicio,
        [FromQuery] DateOnly? fim,
        CancellationToken ct)
        => OkComDados(await leitura.ObterFaturamentoAsync(ExigirIdEmpresa(), inicio, fim, ct));

    [HttpPost("revenue")]
    public async Task<IActionResult> RegistrarFaturamento([FromBody] RequisicaoFaturamento req, CancellationToken ct)
    {
        var validacao = await validadorFaturamento.ValidateAsync(req, ct);
        if (!validacao.IsValid) return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));
        return OkComDados(await escrita.RegistrarFaturamentoAsync(ExigirIdEmpresa(), req, ct), "Faturamento registrado.");
    }

    [HttpGet("sales")]
    public async Task<IActionResult> ObterVendas(
        [FromQuery] DateOnly? inicio,
        [FromQuery] DateOnly? fim,
        CancellationToken ct)
        => OkComDados(await leitura.ObterVendasAsync(ExigirIdEmpresa(), inicio, fim, ct));

    [HttpPost("sales")]
    public async Task<IActionResult> RegistrarVenda([FromBody] RequisicaoVenda req, CancellationToken ct)
    {
        var validacao = await validadorVenda.ValidateAsync(req, ct);
        if (!validacao.IsValid) return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));
        return OkComDados(await escrita.RegistrarVendaAsync(ExigirIdEmpresa(), req, ct), "Venda registrada.");
    }

    [HttpGet("funnel")]
    public async Task<IActionResult> ObterFunil(CancellationToken ct)
        => OkComDados(await leitura.ObterFunilAsync(ExigirIdEmpresa(), ct));

    [HttpPost("funnel/stage")]
    public async Task<IActionResult> RegistrarEtapaFunil([FromBody] RequisicaoEtapaFunil req, CancellationToken ct)
    {
        var validacao = await validadorFunil.ValidateAsync(req, ct);
        if (!validacao.IsValid) return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));
        return OkComDados(await escrita.RegistrarEtapaFunilAsync(ExigirIdEmpresa(), req, ct), "Etapa do funil registrada.");
    }

    [HttpGet("delinquency")]
    public async Task<IActionResult> ObterInadimplencia(CancellationToken ct)
        => OkComDados(await leitura.ObterInadimplenciaAsync(ExigirIdEmpresa(), ct));

    [HttpPost("delinquency")]
    public async Task<IActionResult> RegistrarInadimplencia([FromBody] RequisicaoInadimplencia req, CancellationToken ct)
    {
        var validacao = await validadorInadimplencia.ValidateAsync(req, ct);
        if (!validacao.IsValid) return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));
        return OkComDados(await escrita.RegistrarInadimplenciaAsync(ExigirIdEmpresa(), req, ct), "Inadimplência registrada.");
    }

    [HttpGet("bottlenecks")]
    public async Task<IActionResult> ObterGargalos(CancellationToken ct)
        => OkComDados(await leitura.ObterGargalosAsync(ExigirIdEmpresa(), ct));

    [HttpPost("bottlenecks")]
    public async Task<IActionResult> RegistrarGargalo([FromBody] RequisicaoGargalo req, CancellationToken ct)
    {
        var validacao = await validadorGargalo.ValidateAsync(req, ct);
        if (!validacao.IsValid) return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));
        return OkComDados(await escrita.RegistrarGargaloAsync(ExigirIdEmpresa(), req, ct), "Gargalo registrado.");
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> ObterAlertas(CancellationToken ct)
        => OkComDados(await leitura.ObterAlertasAsync(ExigirIdEmpresa(), ct));
}
