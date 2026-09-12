using RL360.Domain.Common;
using RL360.Domain.Enums;

namespace RL360.Domain.Entities;

public sealed class Usuario : Entity, IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string HashSenha { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; } = PerfilUsuario.Analista;
    public bool Ativo { get; private set; } = true;

    private Usuario() { }

    public Usuario(Guid idEmpresa, string nome, string email, string hashSenha, PerfilUsuario perfil)
    {
        IdEmpresa = idEmpresa;
        Nome = nome;
        Email = email.Trim().ToLowerInvariant();
        HashSenha = hashSenha;
        Perfil = perfil;
    }

    public void DefinirSenha(string hashSenha)
    {
        HashSenha = hashSenha;
        Atualizar();
    }
}
