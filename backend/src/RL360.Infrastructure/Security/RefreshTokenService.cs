using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using RL360.Application.Abstractions;
using RL360.Infrastructure.Security;

namespace RL360.Infrastructure.Security;

public sealed record DadosRefreshToken(Guid IdUsuario, Guid IdEmpresa, DateTime ExpiraEmUtc);

/// <summary>Persiste refresh tokens no cache (Redis ou memória).</summary>
public sealed class ServicoRefreshToken(
    ICacheServico cache,
    IOptions<ConfiguracaoJwt> options) : IRefreshTokenServico
{
    private static string Chave(string token) => $"rl360:refresh:{token}";

    public async Task<(string Token, DateTime ExpiraEmUtc)> GerarAsync(
        Guid idUsuario, Guid idEmpresa, CancellationToken ct = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expira = DateTime.UtcNow.AddDays(options.Value.RefreshExpiracaoDias);
        var dados = new DadosRefreshToken(idUsuario, idEmpresa, expira);
        await cache.DefinirAsync(Chave(token), dados, TimeSpan.FromDays(options.Value.RefreshExpiracaoDias), ct);
        return (token, expira);
    }

    public async Task<(Guid IdUsuario, Guid IdEmpresa)?> ValidarAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;
        var dados = await cache.ObterAsync<DadosRefreshToken>(Chave(refreshToken.Trim()), ct);
        if (dados is null || dados.ExpiraEmUtc <= DateTime.UtcNow) return null;
        return (dados.IdUsuario, dados.IdEmpresa);
    }

    public Task RevogarAsync(string refreshToken, CancellationToken ct = default)
        => cache.RemoverAsync(Chave(refreshToken.Trim()), ct);
}
