namespace RL360.Shared.Dtos;

public sealed class RequisicaoLogin
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public sealed class RespostaAutenticacao
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshExpiraEmUtc { get; set; }
    public UsuarioDto Usuario { get; set; } = new();
}

public sealed class UsuarioDto
{
    public Guid Id { get; set; }
    public Guid IdEmpresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public string NomeEmpresa { get; set; } = string.Empty;
}
