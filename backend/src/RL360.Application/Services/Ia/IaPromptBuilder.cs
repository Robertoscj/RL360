using System.Text;
using RL360.Shared.Dtos;

namespace RL360.Application.Services.Ia;

internal static class IaPromptBuilder
{
    public static string MontarPromptSistema(ResumoDashboardDto resumo, string contextoRag, IReadOnlyList<MensagemConversaIaResumo>? historico)
    {
        var radar = resumo.Radar;
        var sb = new StringBuilder();
        sb.AppendLine("Você é o consultor financeiro do RL360 (Radar de Lucro). Responda em português do Brasil, de forma clara e executiva.");
        sb.AppendLine("Use os dados do radar e documentos internos quando disponíveis. Se não souber, diga o que falta medir.");
        sb.AppendLine();
        sb.AppendLine($"Empresa: {resumo.NomeEmpresa}");
        sb.AppendLine($"Lucro atual: R$ {radar.LucroAtual:N0}");
        sb.AppendLine($"Saúde da empresa: {radar.SaudeEmpresaPercentual}%");
        sb.AppendLine($"Risco 30 dias: R$ {radar.RiscoProximos30Dias:N0}");
        sb.AppendLine($"Oportunidade: R$ {radar.ValorOportunidade:N0}");
        sb.AppendLine($"Gargalos críticos: {radar.GargalosCriticos}");
        sb.AppendLine($"Faturamento mês: R$ {radar.FaturamentoMes:N0}");
        sb.AppendLine($"Meta mensal: R$ {radar.MetaMensal:N0} ({radar.PercentualMetaAtingida:N1}% atingido)");
        sb.AppendLine($"Taxa de conversão: {radar.TaxaConversaoAtual:P1} (queda {radar.QuedaConversao:N1} pp)");
        sb.AppendLine($"Inadimplência atual: R$ {radar.InadimplenciaAtual:N0}");
        sb.AppendLine($"Previsão {resumo.PrevisaoResultado.Dias}d: R$ {resumo.PrevisaoResultado.CenarioMaisProvavel:N0} (meta R$ {resumo.PrevisaoResultado.Meta:N0}, {resumo.PrevisaoResultado.PercentualAbaixoMeta:N0}% abaixo)");
        sb.AppendLine();
        sb.AppendLine("Fatores do radar:");
        foreach (var f in radar.Fatores)
            sb.AppendLine($"- {f.Rotulo}: R$ {f.ImpactoFinanceiro:N0} ({f.Direcao}) — {f.Descricao}");

        if (resumo.AlertasCriticos.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Alertas críticos:");
            foreach (var a in resumo.AlertasCriticos.Take(5))
                sb.AppendLine($"- {a.Titulo}: impacto R$ {a.ImpactoFinanceiro:N0}");
        }

        if (!string.IsNullOrWhiteSpace(contextoRag))
        {
            sb.AppendLine();
            sb.AppendLine(contextoRag);
        }

        if (historico is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("Histórico recente da conversa:");
            foreach (var h in historico.TakeLast(6))
                sb.AppendLine($"- {h.Papel}: {h.Conteudo}");
        }

        sb.AppendLine();
        sb.AppendLine("Responda em até 4 parágrafos curtos, com números quando possível e 1–2 ações práticas.");
        return sb.ToString();
    }
}

internal sealed record MensagemConversaIaResumo(string Papel, string Conteudo);
