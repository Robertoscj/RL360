using System.Text;
using RL360.Shared.Dtos;

namespace RL360.Application.Services.Ia;

internal static class IaPromptBuilder
{
    public static string MontarPromptSistema(ResumoDashboardDto resumo, string contextoRag)
    {
        var radar = resumo.Radar;
        var sb = new StringBuilder();
        sb.AppendLine("Você é o consultor financeiro do RL360. Fale em português do Brasil, como um colega de confiança.");
        sb.AppendLine("Converse. Não recapitule o radar inteiro em toda resposta.");
        sb.AppendLine("Use os números abaixo só quando a pergunta pedir dado, risco, meta ou ação.");
        sb.AppendLine("Se o usuário só cumprimentar ou pedir para ver a conversa, responda curto e humano — sem dump de métricas.");
        sb.AppendLine("Não transcreva o histórico. Ele já aparece no chat.");
        sb.AppendLine("Respostas objetivas: no máximo 3 parágrafos curtos, ou uma lista de 2–3 ações quando fizer sentido.");
        sb.AppendLine();
        sb.AppendLine("Dados internos (consulta, não listar todos):");
        sb.AppendLine($"Empresa: {resumo.NomeEmpresa}");
        sb.AppendLine($"Lucro: R$ {radar.LucroAtual:N0} | Saúde: {radar.SaudeEmpresaPercentual}%");
        sb.AppendLine($"Risco 30d: R$ {radar.RiscoProximos30Dias:N0} | Oportunidade: R$ {radar.ValorOportunidade:N0}");
        sb.AppendLine($"Faturamento: R$ {radar.FaturamentoMes:N0} / meta R$ {radar.MetaMensal:N0} ({radar.PercentualMetaAtingida:N1}%)");
        sb.AppendLine($"Conversão: {(radar.TaxaConversaoAtual <= 1 ? radar.TaxaConversaoAtual * 100m : radar.TaxaConversaoAtual):N1}% (queda {radar.QuedaConversao:N1} pp)");
        sb.AppendLine($"Inadimplência: R$ {radar.InadimplenciaAtual:N0} (perda proj. R$ {radar.InadimplenciaProjetada:N0})");
        sb.AppendLine($"Gargalos críticos: {radar.GargalosCriticos}");

        if (resumo.AlertasCriticos.Count > 0)
        {
            sb.AppendLine("Alertas:");
            foreach (var a in resumo.AlertasCriticos.Take(3))
                sb.AppendLine($"- {a.Titulo}: R$ {a.ImpactoFinanceiro:N0}");
        }

        if (!string.IsNullOrWhiteSpace(contextoRag))
        {
            sb.AppendLine();
            sb.AppendLine(contextoRag);
        }

        return sb.ToString();
    }
}

public sealed record MensagemConversaIaResumo(string Papel, string Conteudo);
