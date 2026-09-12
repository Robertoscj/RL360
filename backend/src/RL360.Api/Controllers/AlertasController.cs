using Microsoft.AspNetCore.Mvc;
using RL360.Api.Extensions;
using RL360.Application.Abstractions;

namespace RL360.Api.Controllers;

[Route("api/alerts")]
public sealed class AlertasController(
    IUsuarioAtual usuarioAtual,
    IAlertaRepositorio alertas) : ControladorBase(usuarioAtual)
{
    [HttpPut("{id:guid}/resolve")]
    public async Task<IActionResult> Resolver(Guid id, CancellationToken ct)
    {
        await alertas.ResolverAsync(ExigirIdEmpresa(), id, ct);
        return OkComDados(true, "Alerta marcado como resolvido.");
    }
}
