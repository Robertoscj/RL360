using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RL360.Application.Abstractions;
using RL360.Shared.Dtos;

namespace RL360.Infrastructure.Security;

public sealed class ServicoJwtToken(IOptions<ConfiguracaoJwt> options) : IJwtTokenServico
{
    private readonly ConfiguracaoJwt _config = options.Value;

    public (string Token, DateTime ExpiraEmUtc) Gerar(UsuarioDto usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.ChaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddMinutes(_config.ExpiracaoMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil),
            new("id_empresa", usuario.IdEmpresa.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config.Emissor,
            audience: _config.Audiencia,
            claims: claims,
            expires: expira,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
