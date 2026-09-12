using RL360.Application.Abstractions;
using RL360.Domain.Enums;
using RL360.Domain.Radar;

namespace RL360.Application.Services;

/// <summary>
/// Orquestra os repositórios para montar as <see cref="EntradasRadar"/> agregadas.
/// </summary>
public sealed class MontadorEntradasRadar(
    IEmpresaRepositorio empresas,
    IFaturamentoRepositorio faturamento,
    IVendaRepositorio vendas,
    IFunilRepositorio funil,
    IInadimplenciaRepositorio inadimplencia,
    IGargaloRepositorio gargalos,
    IClienteRepositorio clientes,
    IEquipeRepositorio equipe)
{
    public async Task<EntradasRadar> MontarAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        var empresa = await empresas.ObterPorIdAsync(idEmpresa, ct);
        var fat = await faturamento.ObterMaisRecenteAsync(idEmpresa, ct);
        var vendasMes = await vendas.ObterMesAtualAsync(idEmpresa, ct);
        var etapas = await funil.ObterEtapasAsync(idEmpresa, ct);
        var inadAberta = await inadimplencia.ObterAbertasAsync(idEmpresa, ct);
        var gargalosAtivos = await gargalos.ObterAtivosAsync(idEmpresa, ct);
        var todosClientes = await clientes.ObterTodosAsync(idEmpresa, ct);
        var membrosEquipe = await equipe.ObterTodosAsync(idEmpresa, ct);

        var inadimplenciaAtual = inadAberta.Sum(d => d.Valor);
        var perdaInadimplencia = inadAberta.Sum(d => d.PerdaProjetada);

        var conversaoAtual = etapas.Count > 0 ? etapas.Average(s => s.TaxaConversao) : 0m;
        var conversaoBase = etapas.Count > 0 ? etapas.Average(s => s.TaxaConversaoBase) : 0m;
        var valorPipeline = etapas.Sum(s => s.ValorPotencial);

        var margemVendasNovas = vendasMes.Where(s => s.ClienteNovo).Sum(s => s.Margem);

        var impactoProdutividade = membrosEquipe.Sum(m => m.ReceitaGerada - m.Meta);

        var potencialExpansao = todosClientes
            .Where(c => c.Status == StatusCliente.OportunidadeExpansao)
            .Sum(c => Math.Round(c.PotencialExpansao * (c.PontuacaoSaude / 100m), 2));

        var impactoGargaloComercial = gargalosAtivos
            .Where(b => b.Area == AreaGargalo.Comercial)
            .Sum(b => Math.Abs(b.ImpactoFinanceiro));

        var perdaClientesRisco = todosClientes
            .Where(c => c.Status == StatusCliente.EmRisco)
            .Sum(c => Math.Round(c.ReceitaMensal * ((100 - c.PontuacaoSaude) / 100m), 2));

        var gargalosCriticos = gargalosAtivos.Count(b => b.Critico);

        return new EntradasRadar
        {
            IdEmpresa = idEmpresa,
            NomeEmpresa = empresa?.Nome ?? "Empresa",
            FaturamentoDia = fat?.FaturamentoDia ?? 0,
            FaturamentoMes = fat?.FaturamentoMes ?? 0,
            MetaMensal = fat?.MetaMensal ?? 0,
            CustoMes = fat?.CustoMes ?? 0,
            InadimplenciaAtual = inadimplenciaAtual,
            PerdaProjetadaInadimplencia = perdaInadimplencia,
            TaxaConversaoAtual = conversaoAtual,
            TaxaConversaoBase = conversaoBase,
            ValorPipeline = valorPipeline,
            PipelineProntoFechar = margemVendasNovas,
            MargemProjetadaVendasNovas = margemVendasNovas,
            ImpactoProdutividade = impactoProdutividade,
            PotencialExpansaoPonderado = potencialExpansao,
            ImpactoGargaloComercial = impactoGargaloComercial,
            PerdaProjetadaClientesEmRisco = perdaClientesRisco,
            QuantidadeGargalosCriticos = gargalosCriticos
        };
    }
}
