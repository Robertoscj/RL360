using RL360.Shared.Dtos;

namespace RL360.Application.Abstractions;

public interface IHashSenhaServico
{
    string GerarHash(string senha);
    bool Verificar(string senha, string hash);
}

public interface IJwtTokenServico
{
    (string Token, DateTime ExpiraEmUtc) Gerar(UsuarioDto usuario);
}

public interface IRefreshTokenServico
{
    Task<(string Token, DateTime ExpiraEmUtc)> GerarAsync(Guid idUsuario, Guid idEmpresa, CancellationToken ct = default);
    Task<(Guid IdUsuario, Guid IdEmpresa)?> ValidarAsync(string refreshToken, CancellationToken ct = default);
    Task RevogarAsync(string refreshToken, CancellationToken ct = default);
}

public interface ICacheServico
{
    Task<T?> ObterAsync<T>(string chave, CancellationToken ct = default);
    Task DefinirAsync<T>(string chave, T valor, TimeSpan? ttl = null, CancellationToken ct = default);
    Task RemoverAsync(string chave, CancellationToken ct = default);
}

public interface IEventoPublicador
{
    Task PublicarRecalculoRadarAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IDashboardNotificador
{
    Task NotificarDashboardAsync(ResumoDashboardDto resumo, CancellationToken ct = default);
    Task NotificarRadarAsync(SnapshotRadarDto snapshot, CancellationToken ct = default);
    Task NotificarFaturamentoAsync(Guid idEmpresa, FaturamentoDto faturamento, CancellationToken ct = default);
    Task NotificarAlertaAsync(Guid idEmpresa, AlertaDto alerta, CancellationToken ct = default);
    Task NotificarGargaloAsync(Guid idEmpresa, GargaloDto gargalo, CancellationToken ct = default);
    Task NotificarPlanoAcaoAsync(Guid idEmpresa, IReadOnlyList<ItemPlanoAcaoDto> plano, CancellationToken ct = default);
}

/// <summary>Alias retrocompatível.</summary>
public interface IRadarNotificador : IDashboardNotificador;

public interface IUsuarioAtual
{
    Guid? IdUsuario { get; }
    Guid? IdEmpresa { get; }
    string? Email { get; }
    bool Autenticado { get; }
}
