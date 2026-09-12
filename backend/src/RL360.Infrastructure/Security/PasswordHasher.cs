using RL360.Application.Abstractions;

namespace RL360.Infrastructure.Security;

public sealed class ServicoHashSenhaBcrypt : IHashSenhaServico
{
    public string GerarHash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 11);

    public bool Verificar(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch
        {
            return false;
        }
    }
}
