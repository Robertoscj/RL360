using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RL360.Api.Extensions;
using RL360.Application.Abstractions;
using RL360.Shared.Responses;

namespace RL360.Api.Controllers;

[ApiController]
[Authorize]
public abstract class ControladorBase(IUsuarioAtual usuarioAtual) : ControllerBase
{
    protected IUsuarioAtual UsuarioAtual { get; } = usuarioAtual;

    protected Guid ExigirIdEmpresa()
        => UsuarioAtual.IdEmpresa ?? throw new UnauthorizedAccessException("Empresa não identificada no token.");

    protected IActionResult OkComDados<T>(T dados, string mensagem = "Operação realizada com sucesso.")
        => Ok(RespostaModel.Ok(dados, mensagem));

    protected IActionResult Falha(string mensagem, int status = 400)
        => status switch
        {
            401 => Unauthorized(RespostaModel.Falha(mensagem)),
            404 => NotFound(RespostaModel.Falha(mensagem)),
            501 => StatusCode(501, RespostaModel.Falha(mensagem)),
            _ => BadRequest(RespostaModel.Falha(mensagem))
        };
}
