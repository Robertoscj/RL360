namespace RL360.Infrastructure.Security;

public sealed class ConfiguracaoJwt
{
    public string Emissor { get; set; } = "RL360";
    public string Audiencia { get; set; } = "RL360.Clients";
    public string ChaveSecreta { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 480;
    public int RefreshExpiracaoDias { get; set; } = 30;
}
