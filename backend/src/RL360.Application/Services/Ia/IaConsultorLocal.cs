using RL360.Shared.Dtos;

namespace RL360.Application.Services.Ia;

/// <summary>
/// Consultor financeiro do Radar. Usa números ao vivo do SQL e playbooks da empresa.
/// É o motor padrão quando não há chave de provedor externo.
/// </summary>
internal static class IaConsultorLocal
{
    public static string Gerar(ResumoDashboardDto resumo, string pergunta, string contextoRag)
    {
        var p = pergunta.ToLowerInvariant();
        var radar = resumo.Radar;
        var rag = MontarCitacaoPlaybook(contextoRag);

        if (Contem(p, "convers", "funil", "vendas", "proposta"))
            return Conversao(resumo, rag);
        if (Contem(p, "inadimpl", "atraso", "cobran", "devedor"))
            return Inadimplencia(resumo, rag);
        if (Contem(p, "gargalo", "aprova", "lento", "demora", "crédito", "credito"))
            return Gargalo(resumo, rag);
        if (Contem(p, "fluxo", "caixa", "receber", "liquidez"))
            return Fluxo(resumo, rag);
        if (Contem(p, "oportun", "crescer", "expandir", "cross"))
            return Oportunidade(resumo, rag);
        if (Contem(p, "meta", "faturamento", "receita"))
            return Meta(resumo, rag);
        if (Contem(p, "lucro", "resultado", "margem", "saúde", "saude"))
            return Lucro(resumo, rag);
        if (Contem(p, "risco", "perder", "30 dia", "urgente"))
            return Risco(resumo, rag);

        return VisaoGeral(resumo, rag);
    }

    private static string Conversao(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var impacto = Impacto(radar, "Conversao");
        var acao = PrimeiraAcao(radar, "funil", "convers");
        return $"""
            A conversão do funil está em {Pct(radar.TaxaConversaoAtual)}, com queda de {radar.QuedaConversao:N1} pontos no período. No radar, isso pressiona cerca de {BRL(impacto)} do resultado.

            Propostas entram, mas fecham abaixo da base histórica. Cada ponto perdido no funil reduz o pipeline pronto para fechar ({BRL(radar.ValorOportunidade)} em oportunidade).

            Ação imediata: {acao} Priorize follow-up nas etapas finais e revise o SLA de crédito — ele costuma ser o abandono mais caro.
            {rag}
            """;
    }

    private static string Inadimplencia(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var perda = Impacto(radar, "Inadimplencia");
        var peso = Percentual(radar.InadimplenciaAtual, radar.FaturamentoMes);
        var acao = PrimeiraAcao(radar, "cobran", "inadimpl");
        return $"""
            A carteira em atraso soma {BRL(radar.InadimplenciaAtual)}, com perda projetada de {BRL(radar.InadimplenciaProjetada)} se nada for feito. Isso equivale a {peso:N1}% do faturamento do período ({BRL(radar.FaturamentoMes)}).

            É o maior risco imediato do radar: o fator Inadimplência impacta {BRL(perda)}. Os próximos 30 dias concentram {BRL(radar.RiscoProximos30Dias)} em risco total.

            Ação imediata: {acao} Use a régua D+1 / D+7 / D+15 e renegocie só os casos com probabilidade de recuperação acima de 40%.
            {rag}
            """;
    }

    private static string Gargalo(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var impacto = Impacto(radar, "GargaloComercial");
        var acao = PrimeiraAcao(radar, "gargalo", "crédito", "credito", "aprova");
        return $"""
            Há {radar.GargalosCriticos} gargalo(s) crítico(s) ativo(s). O gargalo comercial trava cerca de {BRL(impacto)} em resultado.

            Atraso na análise de crédito alonga o ciclo e derruba conversão ({Pct(radar.TaxaConversaoAtual)}, queda de {radar.QuedaConversao:N1} pp). Tickets menores deveriam passar por pré-aprovação automática.

            Ação imediata: {acao} Meta operacional: menos de 1 hora útil por proposta.
            {rag}
            """;
    }

    private static string Fluxo(ResumoDashboardDto resumo, string rag)
    {
        var fluxo = resumo.FluxoCaixaFuturo;
        var total = fluxo.AReceber + fluxo.EmRisco + fluxo.Atrasado;
        return $"""
            O fluxo dos próximos {fluxo.Dias} dias está {fluxo.PercentualSaudavel}% saudável, de um total de {BRL(total)} a vencer ou vencido.

            A receber: {BRL(fluxo.AReceber)} · Em risco: {BRL(fluxo.EmRisco)} · Atrasado: {BRL(fluxo.Atrasado)}. O volume em risco + atrasado exige cobrança ativa nesta semana.

            Ação imediata: priorize títulos atrasados e os em risco com vencimento em 10 dias. Bloqueie novos pedidos acima de D+30 conforme a régua interna.
            {rag}
            """;
    }

    private static string Oportunidade(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var expansao = Impacto(radar, "ExpansaoClientes");
        var acao = PrimeiraAcao(radar, "expans", "cross", "oportun");
        return $"""
            O radar aponta {BRL(radar.ValorOportunidade)} em oportunidades prontas, com expansão de clientes contribuindo {BRL(expansao)}.

            Há folga para crescer sem depender só de venda nova: a base saudável e o cross-sell (Produto A → B, conversão histórica alta) são o caminho mais rápido.

            Ação imediata: {acao}
            {rag}
            """;
    }

    private static string Meta(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var prev = resumo.PrevisaoResultado;
        return $"""
            O faturamento do período está em {BRL(radar.FaturamentoMes)}, {radar.PercentualMetaAtingida:N1}% da meta de {BRL(radar.MetaMensal)}.

            A previsão para os próximos {prev.Dias} dias é {BRL(prev.CenarioMaisProvavel)}, cerca de {prev.PercentualAbaixoMeta:N0}% abaixo da meta de {BRL(prev.Meta)}.

            Para fechar o gap, combine recuperação de conversão ({Pct(radar.TaxaConversaoAtual)}) com as {BRL(radar.ValorOportunidade)} em oportunidades e a cobrança da inadimplência ({BRL(radar.InadimplenciaAtual)}).
            {rag}
            """;
    }

    private static string Lucro(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var positivos = radar.Fatores.Where(f => f.ImpactoFinanceiro > 0).ToList();
        var negativos = radar.Fatores.Where(f => f.ImpactoFinanceiro < 0).ToList();
        var txtPos = positivos.Count == 0
            ? "nenhum fator positivo relevante agora"
            : string.Join(", ", positivos.Select(f => $"{f.Rotulo} (+{BRL(f.ImpactoFinanceiro)})"));
        var txtNeg = negativos.Count == 0
            ? "sem pressão negativa material"
            : string.Join(", ", negativos.Select(f => $"{f.Rotulo} (−{BRL(Math.Abs(f.ImpactoFinanceiro))})"));
        return $"""
            O lucro atual é {BRL(radar.LucroAtual)}, com saúde da empresa em {radar.SaudeEmpresaPercentual}% ({radar.StatusSaude}).

            Puxam o resultado: {txtPos}.
            Pressionam o resultado: {txtNeg}.

            O lucro em risco nos próximos 30 dias é {BRL(radar.RiscoProximos30Dias)}. Ataque primeiro inadimplência e conversão — são os dois alavancadores com maior retorno nesta semana.
            {rag}
            """;
    }

    private static string Risco(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var pior = radar.Fatores
            .Where(f => f.ImpactoFinanceiro < 0)
            .OrderBy(f => f.ImpactoFinanceiro)
            .FirstOrDefault();
        var nomePior = pior?.Rotulo ?? "Inadimplência";
        var valorPior = pior is null ? radar.InadimplenciaProjetada : Math.Abs(pior.ImpactoFinanceiro);
        return $"""
            O maior risco agora é {nomePior}, com impacto de {BRL(valorPior)}. No total, os próximos 30 dias concentram {BRL(radar.RiscoProximos30Dias)} em risco.

            A inadimplência aberta é {BRL(radar.InadimplenciaAtual)} (perda projetada {BRL(radar.InadimplenciaProjetada)}). Conversão em {Pct(radar.TaxaConversaoAtual)} e {radar.GargalosCriticos} gargalo(s) crítico(s) ampliam a pressão.

            Ação imediata: acione a régua nos maiores devedores hoje e desative o gargalo que mais trava o funil. Cada dia parado reduz a chance de receber e de fechar.
            {rag}
            """;
    }

    private static string VisaoGeral(ResumoDashboardDto resumo, string rag)
    {
        var radar = resumo.Radar;
        var pior = radar.Fatores
            .Where(f => f.ImpactoFinanceiro < 0)
            .OrderBy(f => f.ImpactoFinanceiro)
            .FirstOrDefault();
        return $"""
            Visão do radar da {resumo.NomeEmpresa}: lucro {BRL(radar.LucroAtual)}, saúde {radar.SaudeEmpresaPercentual}%, risco 30 dias {BRL(radar.RiscoProximos30Dias)} e {BRL(radar.ValorOportunidade)} em oportunidade.

            Faturamento {BRL(radar.FaturamentoMes)} ({radar.PercentualMetaAtingida:N1}% da meta). Conversão {Pct(radar.TaxaConversaoAtual)} (queda {radar.QuedaConversao:N1} pp). Inadimplência {BRL(radar.InadimplenciaAtual)}.

            Prioridade desta semana: {(pior is null ? "proteger o caixa e acelerar o funil." : $"{pior.Rotulo.ToLowerInvariant()} ({BRL(Math.Abs(pior.ImpactoFinanceiro))}).")} Pergunte sobre conversão, inadimplência, fluxo ou meta para o plano de ação detalhado.
            {rag}
            """;
    }

    private static string MontarCitacaoPlaybook(string contextoRag)
    {
        if (string.IsNullOrWhiteSpace(contextoRag)) return string.Empty;
        var linhas = contextoRag.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(l => l.StartsWith("- ", StringComparison.Ordinal))
            .Take(2)
            .Select(l => l[2..].Trim())
            .ToList();
        if (linhas.Count == 0) return string.Empty;
        return "\nPlaybook interno: " + string.Join(" ", linhas);
    }

    private static string PrimeiraAcao(SnapshotRadarDto radar, params string[] termos)
    {
        var acao = radar.PlanoAcao.FirstOrDefault(a =>
            termos.Any(t =>
                a.Titulo.Contains(t, StringComparison.OrdinalIgnoreCase)
                || a.Justificativa.Contains(t, StringComparison.OrdinalIgnoreCase)));
        acao ??= radar.PlanoAcao.FirstOrDefault();
        return acao is null
            ? "defina um responsável e um prazo de 7 dias."
            : $"{acao.Titulo} (impacto esperado {BRL(acao.ImpactoEsperado)}).";
    }

    private static bool Contem(string texto, params string[] termos)
        => termos.Any(t => texto.Contains(t, StringComparison.OrdinalIgnoreCase));

    private static decimal Impacto(SnapshotRadarDto radar, string fator)
        => Math.Abs(radar.Fatores.FirstOrDefault(f => f.Fator == fator)?.ImpactoFinanceiro ?? 0);

    private static decimal Percentual(decimal valor, decimal baseCalculo)
        => baseCalculo <= 0 ? 0 : valor / baseCalculo * 100m;

    private static string BRL(decimal valor) => $"R$ {valor:N0}";

    private static string Pct(decimal valor)
        => valor <= 1 ? $"{valor * 100m:N1}%" : $"{valor:N1}%";
}
