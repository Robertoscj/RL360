using RL360.Shared.Dtos;

namespace RL360.Application.Services.Ia;

internal static class GeradorInsightsPainel
{
    public static List<InsightInteligenteDto> Montar(ResumoDashboardDto resumo)
    {
        var radar = resumo.Radar;
        var fluxo = resumo.FluxoCaixaFuturo;
        var lista = new List<InsightInteligenteDto>();

        if (radar.InadimplenciaAtual > 0)
        {
            var peso = radar.FaturamentoMes <= 0
                ? 0
                : radar.InadimplenciaAtual / radar.FaturamentoMes * 100m;
            lista.Add(new InsightInteligenteDto
            {
                Id = Guid.NewGuid(),
                Tipo = "AcaoUrgente",
                Titulo = "Cobrança da carteira em atraso",
                Descricao = $"Há R$ {radar.InadimplenciaAtual:N0} em atraso ({peso:N1}% do faturamento), com perda projetada de R$ {radar.InadimplenciaProjetada:N0}. Acione a régua nos maiores devedores.",
                TextoBotao = "Ver inadimplência",
                RotaAcao = "/inadimplencia",
                ImpactoFinanceiro = -radar.InadimplenciaProjetada
            });
        }

        if (radar.GargalosCriticos > 0 || radar.QuedaConversao > 0)
        {
            lista.Add(new InsightInteligenteDto
            {
                Id = Guid.NewGuid(),
                Tipo = "Gargalo",
                Titulo = "Funil e aprovação travando resultado",
                Descricao = $"Conversão em {(radar.TaxaConversaoAtual <= 1 ? radar.TaxaConversaoAtual * 100m : radar.TaxaConversaoAtual):N1}% (queda {radar.QuedaConversao:N1} pp) e {radar.GargalosCriticos} gargalo(s) crítico(s). Reduza o tempo de crédito para menos de 1 hora.",
                TextoBotao = "Ver vendas",
                RotaAcao = "/vendas",
                ImpactoFinanceiro = radar.Fatores.FirstOrDefault(f => f.Fator == "GargaloComercial")?.ImpactoFinanceiro
            });
        }

        if (radar.ValorOportunidade > 0)
        {
            lista.Add(new InsightInteligenteDto
            {
                Id = Guid.NewGuid(),
                Tipo = "Oportunidade",
                Titulo = "Pipeline pronto para converter",
                Descricao = $"Há R$ {radar.ValorOportunidade:N0} em oportunidade. Fluxo a receber de R$ {fluxo.AReceber:N0} nos próximos {fluxo.Dias} dias — priorize fechamento e cross-sell da base saudável.",
                TextoBotao = "Ver vendas",
                RotaAcao = "/vendas",
                ImpactoFinanceiro = radar.ValorOportunidade
            });
        }

        return lista;
    }
}
