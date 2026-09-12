using Microsoft.AspNetCore.Mvc;
using RL360.Shared.Responses;

namespace RL360.Api.Extensions;

public static class RespostaExtensions
{
    public static IActionResult Responder<T>(this ControllerBase _, RespostaModel<T> resposta)
        => resposta.Sucesso ? new OkObjectResult(resposta) : new BadRequestObjectResult(resposta);

    public static IActionResult OkComDados<T>(this ControllerBase controller, T dados, string mensagem = "Operação realizada com sucesso.")
        => controller.Ok(RespostaModel.Ok(dados, mensagem));

    public static IActionResult Falha(this ControllerBase controller, string mensagem, int status = 400)
        => status switch
        {
            401 => controller.Unauthorized(RespostaModel.Falha(mensagem)),
            404 => controller.NotFound(RespostaModel.Falha(mensagem)),
            _ => controller.BadRequest(RespostaModel.Falha(mensagem))
        };
}
