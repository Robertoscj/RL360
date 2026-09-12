using AutoMapper;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;
using RL360.Domain.Enums;
using RL360.Shared.Dtos;

namespace RL360.Application.Services;

public interface IModulosEscritaServico
{
    Task<FaturamentoDto> RegistrarFaturamentoAsync(Guid idEmpresa, RequisicaoFaturamento req, CancellationToken ct = default);
    Task<VendaDto> RegistrarVendaAsync(Guid idEmpresa, RequisicaoVenda req, CancellationToken ct = default);
    Task<EtapaFunilDto> RegistrarEtapaFunilAsync(Guid idEmpresa, RequisicaoEtapaFunil req, CancellationToken ct = default);
    Task<InadimplenciaDto> RegistrarInadimplenciaAsync(Guid idEmpresa, RequisicaoInadimplencia req, CancellationToken ct = default);
    Task<GargaloDto> RegistrarGargaloAsync(Guid idEmpresa, RequisicaoGargalo req, CancellationToken ct = default);
}

public sealed class ModulosEscritaServico(
    IFaturamentoRepositorio faturamento,
    IVendaRepositorio vendas,
    IFunilRepositorio funil,
    IInadimplenciaRepositorio inadimplencia,
    IGargaloRepositorio gargalos,
    IModulosServico modulosLeitura,
    IProcessadorDashboardServico processador,
    IDashboardNotificador notificador,
    IMapper mapper) : IModulosEscritaServico
{
    public async Task<FaturamentoDto> RegistrarFaturamentoAsync(
        Guid idEmpresa, RequisicaoFaturamento req, CancellationToken ct = default)
    {
        await faturamento.InserirAsync(new SnapshotFaturamento(
            idEmpresa, DateTime.UtcNow.Date,
            req.FaturamentoDia, req.FaturamentoMes, req.MetaMensal,
            req.CustoMes, req.CustoFixoMes), ct);

        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
        return await modulosLeitura.ObterFaturamentoAsync(idEmpresa, ct: ct);
    }

    public async Task<VendaDto> RegistrarVendaAsync(
        Guid idEmpresa, RequisicaoVenda req, CancellationToken ct = default)
    {
        var venda = await vendas.InserirAsync(new Venda(
            idEmpresa, req.IdCliente, req.Canal, req.Valor, req.Margem,
            DateTime.UtcNow, req.ClienteNovo), ct);

        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
        return mapper.Map<VendaDto>(venda);
    }

    public async Task<EtapaFunilDto> RegistrarEtapaFunilAsync(
        Guid idEmpresa, RequisicaoEtapaFunil req, CancellationToken ct = default)
    {
        var etapa = await funil.InserirOuAtualizarEtapaAsync(new EtapaFunil(
            idEmpresa, req.Nome, req.Ordem, req.Quantidade,
            req.ValorPotencial, req.TaxaConversao, req.TaxaConversaoBase), ct);

        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
        return mapper.Map<EtapaFunilDto>(etapa);
    }

    public async Task<InadimplenciaDto> RegistrarInadimplenciaAsync(
        Guid idEmpresa, RequisicaoInadimplencia req, CancellationToken ct = default)
    {
        var registro = await inadimplencia.InserirAsync(new RegistroInadimplencia(
            idEmpresa, req.IdCliente, req.NomeCliente, req.Valor,
            req.DiasEmAtraso, req.ProbabilidadeRecuperacao), ct);

        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
        return mapper.Map<InadimplenciaDto>(registro);
    }

    public async Task<GargaloDto> RegistrarGargaloAsync(
        Guid idEmpresa, RequisicaoGargalo req, CancellationToken ct = default)
    {
        if (!Enum.TryParse<AreaGargalo>(req.Area, true, out var area))
            area = AreaGargalo.Comercial;

        var gargalo = await gargalos.InserirAsync(new Gargalo(
            idEmpresa, req.Titulo, req.Descricao, area, req.ImpactoFinanceiro, req.Critico), ct);

        var dto = mapper.Map<GargaloDto>(gargalo);
        await notificador.NotificarGargaloAsync(idEmpresa, dto, ct);
        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
        return dto;
    }
}
