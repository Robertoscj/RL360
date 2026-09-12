using RL360.Domain.Enums;
using RL360.Domain.Radar;

namespace RL360.Domain.Services;

/// <summary>
/// Serviço de domínio puro que transforma métricas brutas em um <see cref="SnapshotRadar"/> completo.
/// </summary>
public sealed class CalculadoraRadar
{
    public SnapshotRadar Calcular(EntradasRadar entradas)
    {
        var lucroAtual = decimal.Round(entradas.FaturamentoMes - entradas.CustoMes, 2);

        var gapConversao = entradas.TaxaConversaoBase - entradas.TaxaConversaoAtual;
        var impactoConversao = decimal.Round(-Math.Max(0, gapConversao) * entradas.ValorPipeline
                                             + Math.Max(0, -gapConversao) * entradas.ValorPipeline, 2);

        var fatores = new List<ResultadoFatorRadar>
        {
            MontarFator(FatorRadar.Inadimplencia, "Inadimplência",
                -Math.Abs(entradas.PerdaProjetadaInadimplencia),
                "Perda projetada com clientes inadimplentes nos próximos 30 dias."),

            MontarFator(FatorRadar.Conversao, "Conversão",
                impactoConversao,
                "Impacto da variação de conversão do funil sobre o pipeline ativo."),

            MontarFator(FatorRadar.VendasNovas, "Vendas novas",
                Math.Abs(entradas.MargemProjetadaVendasNovas),
                "Margem projetada das novas vendas em andamento."),

            MontarFator(FatorRadar.Produtividade, "Produtividade",
                entradas.ImpactoProdutividade,
                "Ganho/perda associado à produtividade da equipe comercial."),

            MontarFator(FatorRadar.ExpansaoClientes, "Expansão de clientes",
                Math.Abs(entradas.PotencialExpansaoPonderado),
                "Receita adicional ponderada por probabilidade de expansão da base."),

            MontarFator(FatorRadar.GargaloComercial, "Gargalo comercial",
                -Math.Abs(entradas.ImpactoGargaloComercial),
                "Resultado travado por gargalos no processo comercial.")
        };

        var magnitudeNegativos = fatores.Where(f => f.ImpactoFinanceiro < 0).Sum(f => Math.Abs(f.ImpactoFinanceiro));
        var riscoProximos30 = decimal.Round(magnitudeNegativos + Math.Abs(entradas.PerdaProjetadaClientesEmRisco), 2);
        var oportunidade = decimal.Round(entradas.PipelineProntoFechar, 2);

        var percentualMeta = entradas.MetaMensal <= 0 ? 0 : decimal.Round(entradas.FaturamentoMes / entradas.MetaMensal * 100m, 1);
        var quedaConversao = decimal.Round(Math.Max(0, gapConversao) * 100m, 1);
        var fluxoCaixaFuturo = decimal.Round(lucroAtual + oportunidade - riscoProximos30, 2);

        var saude = CalcularSaude(percentualMeta, entradas, riscoProximos30);
        var status = saude >= 80 ? StatusSaude.Saudavel
                   : saude >= 60 ? StatusSaude.Atencao
                   : StatusSaude.Critico;

        var planoAcao = MontarPlanoAcao(fatores, entradas, quedaConversao);

        return new SnapshotRadar
        {
            IdEmpresa = entradas.IdEmpresa,
            NomeEmpresa = entradas.NomeEmpresa,
            GeradoEmUtc = DateTime.UtcNow,
            LucroAtual = lucroAtual,
            LucroEmRisco = riscoProximos30,
            RiscoProximos30Dias = riscoProximos30,
            ValorOportunidade = oportunidade,
            GargalosCriticos = entradas.QuantidadeGargalosCriticos,
            SaudeEmpresaPercentual = saude,
            StatusSaude = status,
            FaturamentoDia = decimal.Round(entradas.FaturamentoDia, 2),
            FaturamentoMes = decimal.Round(entradas.FaturamentoMes, 2),
            MetaMensal = decimal.Round(entradas.MetaMensal, 2),
            PercentualMetaAtingida = percentualMeta,
            InadimplenciaAtual = decimal.Round(entradas.InadimplenciaAtual, 2),
            InadimplenciaProjetada = decimal.Round(entradas.PerdaProjetadaInadimplencia, 2),
            TaxaConversaoAtual = decimal.Round(entradas.TaxaConversaoAtual * 100m, 1),
            QuedaConversao = quedaConversao,
            FluxoCaixaFuturo = fluxoCaixaFuturo,
            Fatores = fatores,
            PlanoAcao = planoAcao
        };
    }

    private static ResultadoFatorRadar MontarFator(FatorRadar fator, string rotulo, decimal impacto, string descricao)
    {
        impacto = decimal.Round(impacto, 2);
        return new ResultadoFatorRadar(
            fator,
            rotulo,
            impacto,
            impacto >= 0 ? "positivo" : "negativo",
            descricao);
    }

    private static int CalcularSaude(decimal percentualMeta, EntradasRadar entradas, decimal riscoProximos30)
    {
        var saudeConversao = entradas.TaxaConversaoBase <= 0
            ? 100m
            : Math.Clamp(entradas.TaxaConversaoAtual / entradas.TaxaConversaoBase * 100m, 0, 100);

        var saudeInadimplencia = entradas.FaturamentoMes <= 0
            ? 100m
            : Math.Clamp(100m - (entradas.PerdaProjetadaInadimplencia / entradas.FaturamentoMes * 100m), 0, 100);

        var penalidadeRisco = entradas.FaturamentoMes <= 0 ? 0 : (riscoProximos30 / entradas.FaturamentoMes) * 10m;

        var bruto = 0.45m * Math.Clamp(percentualMeta, 0, 100)
                + 0.25m * saudeConversao
                + 0.30m * saudeInadimplencia
                - penalidadeRisco;

        return (int)Math.Clamp(Math.Round(bruto, MidpointRounding.AwayFromZero), 0, 100);
    }

    private static IReadOnlyList<ItemPlanoAcao> MontarPlanoAcao(
        IReadOnlyList<ResultadoFatorRadar> fatores, EntradasRadar entradas, decimal quedaConversao)
    {
        var plano = new List<ItemPlanoAcao>();

        var inad = fatores.First(f => f.Fator == FatorRadar.Inadimplencia);
        if (inad.ImpactoFinanceiro < 0)
        {
            plano.Add(new ItemPlanoAcao(
                "Acionar régua de cobrança nos maiores inadimplentes",
                $"Recuperar parte dos {Math.Abs(inad.ImpactoFinanceiro):C0} em risco por inadimplência.",
                Math.Round(Math.Abs(inad.ImpactoFinanceiro) * 0.4m, 2),
                PrioridadeAcao.Urgente));
        }

        if (quedaConversao > 0)
        {
            var conv = fatores.First(f => f.Fator == FatorRadar.Conversao);
            plano.Add(new ItemPlanoAcao(
                "Revisar etapas do funil com maior queda de conversão",
                $"Conversão caiu {quedaConversao}pp; recuperar parte de {Math.Abs(conv.ImpactoFinanceiro):C0} do pipeline.",
                Math.Round(Math.Abs(conv.ImpactoFinanceiro) * 0.35m, 2),
                PrioridadeAcao.Alta));
        }

        if (entradas.PotencialExpansaoPonderado > 0)
        {
            plano.Add(new ItemPlanoAcao(
                "Ofertar expansão para clientes saudáveis de alto potencial",
                $"Potencial de {entradas.PotencialExpansaoPonderado:C0} em expansão da base atual.",
                Math.Round(entradas.PotencialExpansaoPonderado * 0.6m, 2),
                PrioridadeAcao.Media));
        }

        var gargalo = fatores.First(f => f.Fator == FatorRadar.GargaloComercial);
        if (gargalo.ImpactoFinanceiro < 0)
        {
            plano.Add(new ItemPlanoAcao(
                "Eliminar gargalo comercial prioritário",
                $"Destravar até {Math.Abs(gargalo.ImpactoFinanceiro):C0} parados no processo comercial.",
                Math.Round(Math.Abs(gargalo.ImpactoFinanceiro) * 0.5m, 2),
                PrioridadeAcao.Alta));
        }

        return plano
            .OrderByDescending(p => (int)p.Prioridade)
            .ThenByDescending(p => p.ImpactoEsperado)
            .ToList();
    }
}
