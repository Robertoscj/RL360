using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;
using RL360.Shared.Dtos;
using RL360.Shared.Options;

namespace RL360.Application.Services.Ia;

public interface IIaConsultorServico
{
    Task<RespostaIaPergunta> PerguntarAsync(
        Guid idEmpresa,
        Guid idUsuario,
        RequisicaoIaPergunta requisicao,
        CancellationToken ct = default);

    IAsyncEnumerable<EventoStreamIaDto> PerguntarStreamAsync(
        Guid idEmpresa,
        Guid idUsuario,
        RequisicaoIaPergunta requisicao,
        CancellationToken ct = default);

    Task<HistoricoIaDto> ObterHistoricoAsync(
        Guid idEmpresa,
        Guid idUsuario,
        int limite = 50,
        CancellationToken ct = default);

    Task LimparHistoricoAsync(Guid idEmpresa, Guid idUsuario, CancellationToken ct = default);
}

public sealed class IaConsultorServico(
    IDashboardServico dashboard,
    IConversaIaRepositorio conversas,
    IRagServico rag,
    IaProvedorChat provedor,
    IOptions<ConfiguracaoIa> configuracao,
    ILogger<IaConsultorServico> logger) : IIaConsultorServico
{
    public async Task<RespostaIaPergunta> PerguntarAsync(
        Guid idEmpresa,
        Guid idUsuario,
        RequisicaoIaPergunta requisicao,
        CancellationToken ct = default)
    {
        var idConversa = requisicao.IdConversa ?? Guid.NewGuid();
        var contexto = await PrepararContextoAsync(idEmpresa, idUsuario, requisicao, ct);

        await SalvarMensagemAsync(idEmpresa, idUsuario, idConversa, "usuario", requisicao.Pergunta, "Usuario", ct);

        string resposta;
        string modo;

        if (provedor.ProvedorExternoDisponivel)
        {
            try
            {
                (resposta, modo) = await provedor.CompletarAsync(
                    contexto.Resumo, requisicao.Pergunta, contexto.PromptSistema, contexto.Historico, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha no provedor externo; usando consultor do radar.");
                resposta = IaConsultorLocal.Gerar(contexto.Resumo, requisicao.Pergunta, contexto.ContextoRag);
                modo = "Consultor";
            }
        }
        else
        {
            resposta = IaConsultorLocal.Gerar(contexto.Resumo, requisicao.Pergunta, contexto.ContextoRag);
            modo = "Consultor";
        }

        await SalvarMensagemAsync(idEmpresa, idUsuario, idConversa, "assistente", resposta, modo, ct);

        return new RespostaIaPergunta
        {
            Resposta = resposta,
            Modo = modo,
            IdConversa = idConversa,
            GeradoEmUtc = DateTime.UtcNow
        };
    }

    public async IAsyncEnumerable<EventoStreamIaDto> PerguntarStreamAsync(
        Guid idEmpresa,
        Guid idUsuario,
        RequisicaoIaPergunta requisicao,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var idConversa = requisicao.IdConversa ?? Guid.NewGuid();
        var contexto = await PrepararContextoAsync(idEmpresa, idUsuario, requisicao, ct);

        await SalvarMensagemAsync(idEmpresa, idUsuario, idConversa, "usuario", requisicao.Pergunta, "Usuario", ct);

        var sb = new System.Text.StringBuilder();
        var modo = IaProvedorConfig.ObterNomeModo(configuracao.Value);

        await foreach (var delta in provedor.CompletarStreamAsync(
            contexto.Resumo, requisicao.Pergunta, contexto.PromptSistema, contexto.Historico, ct))
        {
            sb.Append(delta);
            yield return new EventoStreamIaDto { Tipo = "delta", Delta = delta };
        }

        var respostaFinal = sb.ToString();
        await SalvarMensagemAsync(idEmpresa, idUsuario, idConversa, "assistente", respostaFinal, modo, ct);

        yield return new EventoStreamIaDto
        {
            Tipo = "done",
            Modo = modo,
            IdConversa = idConversa
        };
    }

    public async Task<HistoricoIaDto> ObterHistoricoAsync(
        Guid idEmpresa,
        Guid idUsuario,
        int limite = 50,
        CancellationToken ct = default)
    {
        var mensagens = await conversas.ObterHistoricoAsync(idEmpresa, idUsuario, limite, ct);
        var idConversa = mensagens.LastOrDefault()?.IdConversa ?? Guid.Empty;

        return new HistoricoIaDto
        {
            IdConversa = idConversa,
            Mensagens = mensagens.Select(m => new MensagemIaDto
            {
                Id = m.Id,
                IdConversa = m.IdConversa,
                Papel = m.Papel,
                Conteudo = m.Conteudo,
                Modo = m.Modo,
                CriadoEmUtc = m.CriadoEmUtc
            }).ToList()
        };
    }

    public Task LimparHistoricoAsync(Guid idEmpresa, Guid idUsuario, CancellationToken ct = default)
        => conversas.LimparHistoricoAsync(idEmpresa, idUsuario, ct);

    private async Task<ContextoIa> PrepararContextoAsync(
        Guid idEmpresa,
        Guid idUsuario,
        RequisicaoIaPergunta requisicao,
        CancellationToken ct)
    {
        var resumo = await dashboard.ObterResumoAsync(
            idEmpresa, false, requisicao.Inicio, requisicao.Fim, ct);

        var contextoRag = await rag.ObterContextoAsync(idEmpresa, requisicao.Pergunta, ct);
        var historicoDb = await conversas.ObterHistoricoAsync(idEmpresa, idUsuario, 12, ct);
        var historico = historicoDb
            .Select(m => new MensagemConversaIaResumo(m.Papel, m.Conteudo))
            .ToList();

        var prompt = IaPromptBuilder.MontarPromptSistema(resumo, contextoRag);

        return new ContextoIa(resumo, contextoRag, prompt, historico);
    }

    private Task SalvarMensagemAsync(
        Guid idEmpresa,
        Guid idUsuario,
        Guid idConversa,
        string papel,
        string conteudo,
        string modo,
        CancellationToken ct)
        => conversas.InserirAsync(
            new MensagemConversaIa(idEmpresa, idUsuario, idConversa, papel, conteudo, modo), ct);

    private sealed record ContextoIa(
        ResumoDashboardDto Resumo,
        string ContextoRag,
        string PromptSistema,
        IReadOnlyList<MensagemConversaIaResumo> Historico);
}
