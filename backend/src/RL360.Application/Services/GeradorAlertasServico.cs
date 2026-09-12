using AutoMapper;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;
using RL360.Domain.Enums;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public sealed class GeradorAlertasServico(
    IAlertaRepositorio alertas,
    IInadimplenciaRepositorio inadimplencia,
    IGargaloRepositorio gargalos,
    IFunilRepositorio funil,
    IMapper mapper)
{
    public async Task<IReadOnlyList<AlertaDto>> GerarSeNecessarioAsync(
        Guid idEmpresa, SnapshotRadarDto radar, CancellationToken ct = default)
    {
        var ativos = await alertas.ObterAtivosAsync(idEmpresa, ct);
        var titulosAtivos = ativos.Select(a => a.Titulo).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var novos = new List<Alerta>();

        if (radar.InadimplenciaAtual >= 250_000m && !titulosAtivos.Contains("Inadimplência acima do normal"))
        {
            var total = (await inadimplencia.ObterAbertasAsync(idEmpresa, ct)).Sum(i => i.Valor);
            novos.Add(new Alerta(
                idEmpresa,
                "Inadimplência acima do normal",
                $"Inadimplência aberta totaliza R$ {total:N0}.",
                SeveridadeAlerta.Critico,
                -Math.Min(total * 0.35m, 183_000m),
                FatorRadar.Inadimplencia));
        }

        if (radar.QuedaConversao >= 0.05m && !titulosAtivos.Contains("Queda na conversão"))
        {
            novos.Add(new Alerta(
                idEmpresa,
                "Queda na conversão",
                $"Conversão caiu {radar.QuedaConversao:P0} em relação à base.",
                SeveridadeAlerta.Atencao,
                -152_000m,
                FatorRadar.Conversao));
        }

        var gargalosCriticos = (await gargalos.ObterAtivosAsync(idEmpresa, ct)).Count(g => g.Critico);
        if (gargalosCriticos >= 2 && !titulosAtivos.Contains("Aprovação muito lenta"))
        {
            novos.Add(new Alerta(
                idEmpresa,
                "Aprovação muito lenta",
                $"{gargalosCriticos} gargalos críticos impactam o fluxo operacional.",
                SeveridadeAlerta.Critico,
                -92_000m,
                FatorRadar.GargaloComercial));
        }

        var etapas = await funil.ObterEtapasAsync(idEmpresa, ct);
        if (etapas.Any(e => e.ValorPotencial >= 100_000m) && !titulosAtivos.Contains("Oportunidade na região Sul"))
        {
            var potencial = etapas.Max(e => e.ValorPotencial);
            novos.Add(new Alerta(
                idEmpresa,
                "Oportunidade na região Sul",
                $"Pipeline com potencial de R$ {potencial:N0} nas etapas avançadas.",
                SeveridadeAlerta.Info,
                126_000m,
                FatorRadar.ExpansaoClientes));
        }

        if (radar.ValorOportunidade >= 150_000m && !titulosAtivos.Contains("Produto em alta"))
        {
            novos.Add(new Alerta(
                idEmpresa,
                "Produto em alta",
                "Cross-sell identificado com alta taxa de conversão.",
                SeveridadeAlerta.Info,
                186_000m,
                FatorRadar.VendasNovas));
        }

        var dtos = new List<AlertaDto>();
        foreach (var alerta in novos)
        {
            await alertas.InserirAsync(alerta, ct);
            dtos.Add(mapper.Map<AlertaDto>(alerta));
        }

        return dtos;
    }
}
