using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RL360.Application.Abstractions;
using RL360.Application.Services.Ia;
using RL360.Shared.Dtos;

namespace RL360.Api.Controllers;

[Route("api/ia")]
public sealed class IaController(
    IUsuarioAtual usuarioAtual,
    IIaConsultorServico ia,
    IValidator<RequisicaoIaPergunta> validador) : ControladorBase(usuarioAtual)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [HttpPost("perguntar")]
    public async Task<IActionResult> Perguntar([FromBody] RequisicaoIaPergunta req, CancellationToken ct)
    {
        var validacao = await ValidarAsync(req, ct);
        if (validacao is not null) return validacao;

        var resposta = await ia.PerguntarAsync(ExigirIdEmpresa(), ExigirIdUsuario(), req, ct);
        return OkComDados(resposta, "Análise gerada.");
    }

    [HttpPost("perguntar/stream")]
    public async Task PerguntarStream([FromBody] RequisicaoIaPergunta req, CancellationToken ct)
    {
        var erroValidacao = await ObterErroValidacaoAsync(req, ct);
        if (erroValidacao is not null)
        {
            Response.StatusCode = 400;
            Response.ContentType = "application/json";
            await Response.WriteAsJsonAsync(new { sucesso = false, mensagem = erroValidacao }, ct);
            return;
        }

        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        Response.ContentType = "text/event-stream";

        await foreach (var evt in ia.PerguntarStreamAsync(ExigirIdEmpresa(), ExigirIdUsuario(), req, ct))
        {
            var json = JsonSerializer.Serialize(evt, JsonOpts);
            await Response.WriteAsync($"data: {json}\n\n", ct);
            await Response.Body.FlushAsync(ct);
        }
    }

    [HttpGet("historico")]
    public async Task<IActionResult> Historico([FromQuery] int limite = 50, CancellationToken ct = default)
        => OkComDados(await ia.ObterHistoricoAsync(ExigirIdEmpresa(), ExigirIdUsuario(), limite, ct));

    [HttpDelete("historico")]
    public async Task<IActionResult> LimparHistorico(CancellationToken ct)
    {
        await ia.LimparHistoricoAsync(ExigirIdEmpresa(), ExigirIdUsuario(), ct);
        return OkComDados(true, "Histórico limpo.");
    }

    private async Task<IActionResult?> ValidarAsync(RequisicaoIaPergunta req, CancellationToken ct)
    {
        var erro = await ObterErroValidacaoAsync(req, ct);
        return erro is null ? null : Falha(erro);
    }

    private async Task<string?> ObterErroValidacaoAsync(RequisicaoIaPergunta req, CancellationToken ct)
    {
        var validacao = await validador.ValidateAsync(req, ct);
        return validacao.IsValid ? null : string.Join(" ", validacao.Errors.Select(e => e.ErrorMessage));
    }

    private Guid ExigirIdUsuario()
        => UsuarioAtual.IdUsuario ?? throw new UnauthorizedAccessException("Usuário não identificado no token.");
}
