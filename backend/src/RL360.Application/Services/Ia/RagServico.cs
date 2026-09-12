using Microsoft.Extensions.Options;
using RL360.Application.Abstractions;
using RL360.Shared.Options;

namespace RL360.Application.Services.Ia;

public interface IRagServico
{
    Task<string> ObterContextoAsync(Guid idEmpresa, string pergunta, CancellationToken ct = default);
}

public sealed class RagServico(
    IDocumentoConhecimentoRepositorio documentos,
    IOptions<ConfiguracaoIa> configuracao) : IRagServico
{
    public async Task<string> ObterContextoAsync(Guid idEmpresa, string pergunta, CancellationToken ct = default)
    {
        var config = configuracao.Value;
        if (!config.HabilitarRag) return string.Empty;

        var encontrados = await documentos.BuscarAsync(idEmpresa, pergunta, config.MaxDocumentosRag, ct);
        if (encontrados.Count == 0) return string.Empty;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Documentos internos relevantes:");
        foreach (var doc in encontrados)
            sb.AppendLine($"- [{doc.Categoria}] {doc.Titulo}: {doc.Conteudo}");

        return sb.ToString();
    }
}
