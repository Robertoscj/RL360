namespace RL360.Shared.Options;

public sealed class ConfiguracaoIa
{
    /// <summary>Demo | OpenAI | AzureOpenAI</summary>
    public string Provedor { get; set; } = "Demo";
    public string ApiKey { get; set; } = string.Empty;
    public string Modelo { get; set; } = "gpt-4o-mini";
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    /// <summary>Nome do deployment Azure (usa Modelo se vazio).</summary>
    public string? NomeDeployment { get; set; }
    public string VersaoApi { get; set; } = "2024-08-01-preview";
    public int MaxTokens { get; set; } = 600;
    public bool HabilitarRag { get; set; } = true;
    public int MaxDocumentosRag { get; set; } = 3;
}
