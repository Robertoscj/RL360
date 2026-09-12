using AutoMapper;
using RL360.Application.Abstractions;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface IModulosServico
{
    Task<FaturamentoDto> ObterFaturamentoAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<VendaDto>> ObterVendasAsync(Guid idEmpresa, CancellationToken ct = default);
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
    public async Task<FaturamentoDto> ObterFaturamentoAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        var recente = await faturamento.ObterMaisRecenteAsync(idEmpresa, ct);
        var serie = await faturamento.ObterSerieDiariaAsync(idEmpresa, 30, ct);

        var fatMes = recente?.FaturamentoMes ?? 0;
        var meta = recente?.MetaMensal ?? 0;
        var custo = recente?.CustoMes ?? 0;

        return new FaturamentoDto
        {
            FaturamentoDia = recente?.FaturamentoDia ?? 0,
            FaturamentoMes = fatMes,
            MetaMensal = meta,
            PercentualMetaAtingida = meta <= 0 ? 0 : Math.Round(fatMes / meta * 100m, 1),
            CustoMes = custo,
            Lucro = fatMes - custo,
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

    public async Task<IReadOnlyList<VendaDto>> ObterVendasAsync(Guid idEmpresa, CancellationToken ct = default)
        => mapper.Map<List<VendaDto>>(await vendas.ObterMesAtualAsync(idEmpresa, ct));

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
