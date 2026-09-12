using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RL360.Shared.Dtos;
using RL360.Shared.Options;

namespace RL360.Application.Services.Ia;

public sealed class IaProvedorChat(
    IHttpClientFactory httpClientFactory,
    IOptions<ConfiguracaoIa> configuracao,
    ILogger<IaProvedorChat> logger)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public bool ProvedorExternoDisponivel => IaProvedorConfig.UsaProvedorExterno(configuracao.Value);

    public async Task<(string Texto, string Modo)> CompletarAsync(
        ResumoDashboardDto resumo,
        string pergunta,
        string promptSistema,
        CancellationToken ct)
    {
        var config = configuracao.Value;
        var modo = IaProvedorConfig.ObterNomeModo(config);
        var texto = await EnviarChatAsync(config, promptSistema, pergunta, stream: false, ct);
        return (texto, modo);
    }

    public async IAsyncEnumerable<string> CompletarStreamAsync(
        ResumoDashboardDto resumo,
        string pergunta,
        string promptSistema,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var config = configuracao.Value;

        if (!ProvedorExternoDisponivel)
        {
            var demo = IaRespostaDemo.Gerar(resumo, pergunta, ExtrairRagDoPrompt(promptSistema));
            foreach (var chunk in DividirEmChunks(demo, 12))
            {
                yield return chunk;
                await Task.Delay(18, ct);
            }
            yield break;
        }

        await foreach (var delta in LerStreamOpenAiAsync(config, promptSistema, pergunta, ct))
            yield return delta;
    }

    private async Task<string> EnviarChatAsync(
        ConfiguracaoIa config,
        string promptSistema,
        string pergunta,
        bool stream,
        CancellationToken ct)
    {
        var client = CriarCliente(config);
        var payload = MontarPayload(config, promptSistema, pergunta, stream);
        var url = IaProvedorConfig.ResolverUrlChat(config);

        using var response = await client.PostAsJsonAsync(url, payload, JsonOpts, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<OpenAiResposta>(JsonOpts, ct)
                   ?? throw new InvalidOperationException("Resposta vazia do provedor de IA.");

        var texto = json.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
        if (string.IsNullOrWhiteSpace(texto))
            throw new InvalidOperationException("Conteúdo vazio retornado pelo provedor de IA.");

        return texto;
    }

    private async IAsyncEnumerable<string> LerStreamOpenAiAsync(
        ConfiguracaoIa config,
        string promptSistema,
        string pergunta,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var client = CriarCliente(config);
        var payload = MontarPayload(config, promptSistema, pergunta, stream: true);
        var url = IaProvedorConfig.ResolverUrlChat(config);

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload, options: JsonOpts)
        };

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var linha = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(linha) || !linha.StartsWith("data: ", StringComparison.Ordinal))
                continue;

            var dados = linha["data: ".Length..].Trim();
            if (dados == "[DONE]") yield break;

            OpenAiStreamChunk? chunk;
            try
            {
                chunk = JsonSerializer.Deserialize<OpenAiStreamChunk>(dados, JsonOpts);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Chunk SSE ignorado.");
                continue;
            }

            var delta = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;
            if (!string.IsNullOrEmpty(delta))
                yield return delta;
        }
    }

    private HttpClient CriarCliente(ConfiguracaoIa config)
    {
        var client = httpClientFactory.CreateClient("OpenAI");
        IaProvedorConfig.ConfigurarAutenticacao(client, config);
        return client;
    }

    private static object MontarPayload(ConfiguracaoIa config, string promptSistema, string pergunta, bool stream)
        => new
        {
            model = IaProvedorConfig.ObterModelo(config),
            temperature = 0.3,
            max_tokens = config.MaxTokens,
            stream,
            messages = new object[]
            {
                new { role = "system", content = promptSistema },
                new { role = "user", content = pergunta }
            }
        };

    private static IEnumerable<string> DividirEmChunks(string texto, int tamanho)
    {
        for (var i = 0; i < texto.Length; i += tamanho)
            yield return texto.Substring(i, Math.Min(tamanho, texto.Length - i));
    }

    private static string ExtrairRagDoPrompt(string prompt)
    {
        const string marcador = "Documentos internos relevantes:";
        var idx = prompt.IndexOf(marcador, StringComparison.Ordinal);
        return idx < 0 ? string.Empty : prompt[idx..];
    }

    private sealed class OpenAiResposta
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private sealed class OpenAiChoice
    {
        public OpenAiMessage? Message { get; set; }
    }

    private sealed class OpenAiMessage
    {
        public string? Content { get; set; }
    }

    private sealed class OpenAiStreamChunk
    {
        public List<OpenAiStreamChoice>? Choices { get; set; }
    }

    private sealed class OpenAiStreamChoice
    {
        public OpenAiStreamDelta? Delta { get; set; }
    }

    private sealed class OpenAiStreamDelta
    {
        public string? Content { get; set; }
    }
}

internal static class IaProvedorConfig
{
    public static bool UsaProvedorExterno(ConfiguracaoIa config)
        => (string.Equals(config.Provedor, "OpenAI", StringComparison.OrdinalIgnoreCase)
            || string.Equals(config.Provedor, "AzureOpenAI", StringComparison.OrdinalIgnoreCase))
           && !string.IsNullOrWhiteSpace(config.ApiKey);

    public static string ObterNomeModo(ConfiguracaoIa config)
        => string.Equals(config.Provedor, "AzureOpenAI", StringComparison.OrdinalIgnoreCase)
            ? "AzureOpenAI"
            : "OpenAI";

    public static string ObterModelo(ConfiguracaoIa config)
        => string.Equals(config.Provedor, "AzureOpenAI", StringComparison.OrdinalIgnoreCase)
            ? (config.NomeDeployment ?? config.Modelo)
            : config.Modelo;

    public static string ResolverUrlChat(ConfiguracaoIa config)
    {
        if (string.Equals(config.Provedor, "AzureOpenAI", StringComparison.OrdinalIgnoreCase))
        {
            var deployment = config.NomeDeployment ?? config.Modelo;
            return $"{config.BaseUrl.TrimEnd('/')}/openai/deployments/{deployment}/chat/completions?api-version={config.VersaoApi}";
        }

        return $"{config.BaseUrl.TrimEnd('/')}/chat/completions";
    }

    public static void ConfigurarAutenticacao(HttpClient client, ConfiguracaoIa config)
    {
        client.DefaultRequestHeaders.Authorization = null;
        client.DefaultRequestHeaders.Remove("api-key");

        if (string.Equals(config.Provedor, "AzureOpenAI", StringComparison.OrdinalIgnoreCase))
            client.DefaultRequestHeaders.Add("api-key", config.ApiKey);
        else
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiKey);
    }
}
