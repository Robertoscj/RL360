using RL360.Application.Abstractions;

namespace RL360.Api.Middleware;

/// <summary>
/// Garante que requisições autenticadas possuem IdEmpresa no token
/// e que parâmetros de rota/query não acessem outra empresa.
/// </summary>
public sealed class ValidacaoEmpresaMiddleware(RequestDelegate next, ILogger<ValidacaoEmpresaMiddleware> logger)
{
    private static readonly HashSet<string> RotasPublicas = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health", "/swagger", "/api/auth/login", "/api/auth/register", "/api/auth/refresh"
    };

    public async Task InvokeAsync(HttpContext context, IUsuarioAtual usuarioAtual)
    {
        var caminho = context.Request.Path.Value ?? string.Empty;

        if (RotasPublicas.Any(r => caminho.StartsWith(r, StringComparison.OrdinalIgnoreCase))
            || caminho.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var idEmpresaToken = usuarioAtual.IdEmpresa;
        if (idEmpresaToken is null)
        {
            logger.LogWarning("Token sem id_empresa para {Metodo} {Caminho}", context.Request.Method, caminho);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { sucesso = false, mensagem = "Empresa não identificada no token." });
            return;
        }

        if (context.Request.Headers.TryGetValue("X-Id-Empresa", out var headerEmpresa)
            && Guid.TryParse(headerEmpresa, out var idHeader)
            && idHeader != idEmpresaToken)
        {
            logger.LogWarning("Tentativa cross-tenant: token={TokenEmpresa} header={HeaderEmpresa}", idEmpresaToken, idHeader);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { sucesso = false, mensagem = "Acesso negado a dados de outra empresa." });
            return;
        }

        context.Items["IdEmpresa"] = idEmpresaToken;
        await next(context);
    }
}
