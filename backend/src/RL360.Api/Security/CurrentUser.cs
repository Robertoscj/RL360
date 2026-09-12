using System.Security.Claims;
using RL360.Application.Abstractions;

namespace RL360.Api.Security;

public sealed class UsuarioAtual(IHttpContextAccessor accessor) : IUsuarioAtual
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool Autenticado => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? IdUsuario => TentarGuid(Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                                    ?? Principal?.FindFirstValue("sub"));

    public Guid? IdEmpresa => TentarGuid(Principal?.FindFirstValue("id_empresa")
                              ?? Principal?.FindFirstValue("tenant_id"));

    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email)
                            ?? Principal?.FindFirstValue("email");

    private static Guid? TentarGuid(string? valor) => Guid.TryParse(valor, out var g) ? g : null;
}
