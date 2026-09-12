using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RL360.Application.Abstractions;
using RL360.Application.Services;
using RL360.Shared.Dtos;

namespace RL360.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AutenticacaoController(
    IAutenticacaoServico autenticacao,
    IValidator<RequisicaoLogin> validadorLogin,
    IValidator<RequisicaoRegistro> validadorRegistro,
    IUsuarioAtual usuarioAtual) : ControladorBase(usuarioAtual)
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Entrar([FromBody] RequisicaoLogin requisicao, CancellationToken ct)
    {
        var validacao = await validadorLogin.ValidateAsync(requisicao, ct);
        if (!validacao.IsValid)
            return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));

        var resultado = await autenticacao.EntrarAsync(requisicao, ct);
        return resultado.Sucesso
            ? OkComDados(resultado.Valor!, "Login realizado com sucesso.")
            : Falha(resultado.Erro ?? "Credenciais inválidas.", 401);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] RequisicaoRegistro requisicao, CancellationToken ct)
    {
        var validacao = await validadorRegistro.ValidateAsync(requisicao, ct);
        if (!validacao.IsValid)
            return Falha(string.Join("; ", validacao.Errors.Select(e => e.ErrorMessage)));

        var resultado = await autenticacao.RegistrarAsync(requisicao, ct);
        return resultado.Sucesso
            ? OkComDados(resultado.Valor!, "Conta criada com sucesso.")
            : Falha(resultado.Erro ?? "Não foi possível registrar.");
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Renovar([FromBody] RequisicaoRefreshToken requisicao, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(requisicao.RefreshToken))
            return Falha("Refresh token é obrigatório.");

        var resultado = await autenticacao.RenovarTokenAsync(requisicao, ct);
        return resultado.Sucesso
            ? OkComDados(resultado.Valor!, "Token renovado com sucesso.")
            : Falha(resultado.Erro ?? "Refresh token inválido.", 401);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Eu(CancellationToken ct)
    {
        if (usuarioAtual.IdUsuario is not { } idUsuario)
            return Falha("Não autenticado.", 401);

        var resultado = await autenticacao.ObterAtualAsync(idUsuario, ct);
        return resultado.Sucesso
            ? OkComDados(resultado.Valor!)
            : Falha(resultado.Erro ?? "Usuário não encontrado.", 404);
    }
}
