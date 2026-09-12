namespace RL360.Domain.Common;

/// <summary>
/// Tipo base de todas as entidades de domínio.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CriadoEmUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEmUtc { get; protected set; }

    public void Atualizar() => AtualizadoEmUtc = DateTime.UtcNow;
}

/// <summary>
/// Marca agregados que pertencem a uma empresa (multi-tenant SaaS).
/// </summary>
public interface IEmpresaProprietaria
{
    Guid IdEmpresa { get; }
}
