using RL360.Shared.Responses;

namespace RL360.Api.Middleware;

public sealed class TratamentoExcecoesMiddleware(RequestDelegate next, ILogger<TratamentoExcecoesMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Acesso não autorizado");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(RespostaModel.Falha(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado em {Caminho}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(RespostaModel.Falha("Erro interno do servidor."));
        }
    }
}
