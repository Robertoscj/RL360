using RL360.Shared.Dtos;

namespace RL360.Application.Services.Ia;

internal static class IaRespostaDemo
{
    public static string Gerar(ResumoDashboardDto resumo, string pergunta, string contextoRag)
    {
        var p = pergunta.ToLowerInvariant();
        var radar = resumo.Radar;
        var rag = string.IsNullOrWhiteSpace(contextoRag) ? "" : $"\n\nCom base nos documentos internos: {ExtrairResumoRag(contextoRag)}";

        if (Contem(p, "convers", "funil", "vendas"))
        {
            return $"""
                A conversão geral está em {radar.TaxaConversaoAtual:P1}, com queda de {radar.QuedaConversao:N1} pontos percentuais no período.

                No radar, o fator Conversão impacta cerca de R$ {ObterImpactoFator(radar, "Conversao"):N0} no lucro. Isso indica propostas entrando, mas fechando abaixo da meta.

                Recomendo revisar as etapas com maior abandono (especialmente aprovação de crédito) e priorizar follow-up nas oportunidades quentes do funil.{rag}
                """;
        }

        if (Contem(p, "inadimpl", "atraso", "cobran"))
        {
            return $"""
                A inadimplência atual é de R$ {radar.InadimplenciaAtual:N0}, com perda projetada relevante no radar (R$ {ObterImpactoFator(radar, "Inadimplencia"):N0}).

                Esse é o maior risco imediato: representa {PercentualDoFaturamento(radar.InadimplenciaAtual, radar.FaturamentoMes):N1}% do faturamento do mês.

                Ação sugerida: acionar régua de cobrança nos maiores devedores e renegociar prazos nos casos recuperáveis.{rag}
                """;
        }

        if (Contem(p, "gargalo", "aprova", "lento", "demora"))
        {
            return $"""
                Há {radar.GargalosCriticos} gargalos críticos ativos. O principal no radar é o gargalo comercial, com impacto estimado de R$ {ObterImpactoFator(radar, "GargaloComercial"):N0}.

                O alerta de aprovação lenta indica tempo médio elevado por proposta, o que trava conversão e aumenta o risco de perda de negócios.

                Priorize eliminar o gargalo de análise de crédito e reduzir o tempo médio para abaixo de 1 hora por proposta.{rag}
                """;
        }

        if (Contem(p, "lucro", "resultado", "margem"))
        {
            return $"""
                O lucro atual do período é R$ {radar.LucroAtual:N0}, com saúde da empresa em {radar.SaudeEmpresaPercentual}%.

                Fatores positivos: expansão de clientes (+R$ {ObterImpactoFator(radar, "ExpansaoClientes"):N0}), vendas novas (+R$ {ObterImpactoFator(radar, "VendasNovas"):N0}) e produtividade (+R$ {ObterImpactoFator(radar, "Produtividade"):N0}).

                Fatores negativos: inadimplência, conversão e gargalo comercial somam pressão de R$ {SomaImpactosNegativos(radar):N0}.{rag}
                """;
        }

        if (Contem(p, "meta", "faturamento", "receita"))
        {
            return $"""
                O faturamento do mês está em R$ {radar.FaturamentoMes:N0}, com {radar.PercentualMetaAtingida:N1}% da meta mensal de R$ {radar.MetaMensal:N0}.

                A previsão para os próximos {resumo.PrevisaoResultado.Dias} dias é R$ {resumo.PrevisaoResultado.CenarioMaisProvavel:N0}, cerca de {resumo.PrevisaoResultado.PercentualAbaixoMeta:N0}% abaixo da meta de R$ {resumo.PrevisaoResultado.Meta:N0}.

                Para fechar o gap, combine recuperação de conversão com ações de cross-sell (R$ {radar.ValorOportunidade:N0} em oportunidades).{rag}
                """;
        }

        if (Contem(p, "fluxo", "caixa", "receber"))
        {
            var fluxo = resumo.FluxoCaixaFuturo;
            return $"""
                O fluxo de caixa futuro ({fluxo.Dias} dias) está {fluxo.PercentualSaudavel}% saudável.

                A receber: R$ {fluxo.AReceber:N0} | Em risco: R$ {fluxo.EmRisco:N0} | Atrasado: R$ {fluxo.Atrasado:N0}.

                O volume em risco e atrasado exige cobrança ativa esta semana.{rag}
                """;
        }

        if (Contem(p, "oportun", "crescer", "expandir"))
        {
            return $"""
                Identificamos R$ {radar.ValorOportunidade:N0} em oportunidades no radar, com expansão de clientes contribuindo +R$ {ObterImpactoFator(radar, "ExpansaoClientes"):N0}.

                Há potencial adicional em cross-sell e regiões com pipeline forte.{rag}
                """;
        }

        return $"""
            Com base no radar atual da {resumo.NomeEmpresa}:

            • Lucro: R$ {radar.LucroAtual:N0} | Saúde: {radar.SaudeEmpresaPercentual}%
            • Risco 30 dias: R$ {radar.RiscoProximos30Dias:N0} | Oportunidade: R$ {radar.ValorOportunidade:N0}
            • Conversão: {radar.TaxaConversaoAtual:P1} | Inadimplência: R$ {radar.InadimplenciaAtual:N0}

            Pergunte de forma específica (ex.: "Por que a conversão caiu?") para análise detalhada.{rag}

            Modo demo — configure OpenAI ou Azure OpenAI em appsettings para respostas generativas.
            """;
    }

    private static string ExtrairResumoRag(string contexto)
    {
        var linhas = contexto.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return linhas.Length <= 1 ? contexto : string.Join(' ', linhas.Skip(1).Take(2));
    }

    private static bool Contem(string texto, params string[] termos)
        => termos.Any(t => texto.Contains(t, StringComparison.OrdinalIgnoreCase));

    private static decimal ObterImpactoFator(SnapshotRadarDto radar, string fator)
        => Math.Abs(radar.Fatores.FirstOrDefault(f => f.Fator == fator)?.ImpactoFinanceiro ?? 0);

    private static decimal SomaImpactosNegativos(SnapshotRadarDto radar)
        => radar.Fatores.Where(f => f.ImpactoFinanceiro < 0).Sum(f => Math.Abs(f.ImpactoFinanceiro));

    private static decimal PercentualDoFaturamento(decimal valor, decimal faturamento)
        => faturamento <= 0 ? 0 : valor / faturamento * 100m;
}
