using AutoMapper;
using RL360.Application.Abstractions;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface IModulosServico
{
    Task<FaturamentoDto> ObterFaturamentoAsync(
        Guid idEmpresa,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken ct = default);
    Task<IReadOnlyList<VendaDto>> ObterVendasAsync(
        Guid idEmpresa,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken ct = default);
    Task<IReadOnlyList<EtapaFunilDto>> ObterFunilAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<InadimplenciaDto>> ObterInadimplenciaAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<GargaloDto>> ObterGargalosAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<ClienteDto>> ObterClientesAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<MembroEquipeDto>> ObterEquipeAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<MetaDto>> ObterMetasAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<AlertaDto>> ObterAlertasAsync(Guid idEmpresa, CancellationToken ct = default);
}

public sealed class ModulosServico(
    IFaturamentoRepositorio faturamento,
    IVendaRepositorio vendas,
    IFunilRepositorio funil,
    IInadimplenciaRepositorio inadimplencia,
    IGargaloRepositorio gargalos,
    IClienteRepositorio clientes,
    IEquipeRepositorio equipe,
    IMetaRepositorio metas,
    IAlertaRepositorio alertas,
    IMapper mapper) : IModulosServico
{
    public async Task<FaturamentoDto> ObterFaturamentoAsync(
        Guid idEmpresa,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken ct = default)
    {
        var recente = await faturamento.ObterMaisRecenteAsync(idEmpresa, ct);
        var serie = inicio is not null && fim is not null
            ? await faturamento.ObterPorPeriodoAsync(idEmpresa, inicio.Value, fim.Value, ct)
            : await faturamento.ObterSerieDiariaAsync(idEmpresa, 30, ct);
        var vendasPeriodo = inicio is not null && fim is not null
            ? await vendas.ObterPorPeriodoAsync(idEmpresa, inicio.Value, fim.Value, ct)
            : await vendas.ObterMesAtualAsync(idEmpresa, ct);

        var fatMes = serie.Count > 0 ? serie.Sum(s => s.FaturamentoDia) : recente?.FaturamentoMes ?? 0;
        var meta = recente?.MetaMensal ?? serie.LastOrDefault()?.MetaMensal ?? 0;
        var custoCheio = recente?.CustoMes ?? 0;
        var diasSerie = Math.Max(1, serie.Count);
        var diasMes = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
        var custo = inicio is not null && fim is not null
            ? Math.Round(custoCheio * diasSerie / diasMes, 2)
            : custoCheio;
        var ultimo = serie.LastOrDefault() ?? recente;
        var qtd = vendasPeriodo.Count;
        var ticket = qtd <= 0 ? 0 : Math.Round(vendasPeriodo.Sum(v => v.Valor) / qtd, 2);

        return new FaturamentoDto
        {
            FaturamentoDia = ultimo?.FaturamentoDia ?? 0,
            FaturamentoMes = fatMes,
            MetaMensal = meta,
            PercentualMetaAtingida = meta <= 0 ? 0 : Math.Round(fatMes / meta * 100m, 1),
            CustoMes = custo,
            Lucro = fatMes - custo,
            QuantidadeOperacoes = qtd,
            TicketMedio = ticket,
            SerieDiaria = serie
                .OrderBy(s => s.DataReferencia)
                .Select(s => new PontoSerieTemporalDto
                {
                    Rotulo = s.DataReferencia.ToString("dd/MM"),
                    Valor = s.FaturamentoDia,
                    Meta = s.MetaMensal / DateTime.DaysInMonth(s.DataReferencia.Year, s.DataReferencia.Month)
                })
                .ToList()
        };
    }

    public async Task<IReadOnlyList<VendaDto>> ObterVendasAsync(
        Guid idEmpresa,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken ct = default)
    {
        var lista = inicio is not null && fim is not null
            ? await vendas.ObterPorPeriodoAsync(idEmpresa, inicio.Value, fim.Value, ct)
            : await vendas.ObterMesAtualAsync(idEmpresa, ct);
        return mapper.Map<List<VendaDto>>(lista);
    }

    public async Task<IReadOnlyList<EtapaFunilDto>> ObterFunilAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<EtapaFunilDto>>(await funil.ObterEtapasAsync(idEmpresa, ct));

    public async Task<IReadOnlyList<InadimplenciaDto>> ObterInadimplenciaAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<InadimplenciaDto>>(await inadimplencia.ObterAbertasAsync(idEmpresa, ct));

    public async Task<IReadOnlyList<GargaloDto>> ObterGargalosAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<GargaloDto>>(await gargalos.ObterAtivosAsync(idEmpresa, ct));

    public async Task<IReadOnlyList<ClienteDto>> ObterClientesAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<ClienteDto>>(await clientes.ObterTodosAsync(idEmpresa, ct));

    public async Task<IReadOnlyList<MembroEquipeDto>> ObterEquipeAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<MembroEquipeDto>>(await equipe.ObterTodosAsync(idEmpresa, ct));

    public async Task<IReadOnlyList<MetaDto>> ObterMetasAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<MetaDto>>(await metas.ObterAtivasAsync(idEmpresa, ct));

    public async Task<IReadOnlyList<AlertaDto>> ObterAlertasAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<AlertaDto>>(await alertas.ObterAtivosAsync(idEmpresa, ct));
}
