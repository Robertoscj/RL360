using System.Diagnostics;
using RL360.Application.Abstractions;

namespace RL360.Api.Middleware;

/// <summary>Logs estruturados por requisição com contexto de empresa e usuário.</summary>
public sealed class LogRequisicaoMiddleware(RequestDelegate next, ILogger<LogRequisicaoMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUsuarioAtual usuarioAtual)
    {
        var cronometro = Stopwatch.StartNew();
        var caminho = context.Request.Path.Value ?? "/";
        var metodo = context.Request.Method;

        try
        {
            await next(context);
        }
        finally
        {
            cronometro.Stop();
            logger.LogInformation(
                "HTTP {Metodo} {Caminho} => {StatusCode} em {DuracaoMs}ms | Empresa={IdEmpresa} Usuario={IdUsuario} Email={Email}",
                metodo,
                caminho,
                context.Response.StatusCode,
                cronometro.ElapsedMilliseconds,
                usuarioAtual.IdEmpresa,
                usuarioAtual.IdUsuario,
                usuarioAtual.Email);
        }
    }
}
