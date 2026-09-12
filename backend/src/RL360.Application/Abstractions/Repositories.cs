using RL360.Domain.Entities;

namespace RL360.Application.Abstractions;

public interface IEmpresaRepositorio
{
    Task<Empresa?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Empresa>> ObterTodosAsync(CancellationToken ct = default);
    Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct = default);
    Task<Empresa> CriarAsync(Empresa empresa, CancellationToken ct = default);
}

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario> CriarAsync(Usuario usuario, CancellationToken ct = default);
}

public interface IFaturamentoRepositorio
{
    Task<SnapshotFaturamento?> ObterMaisRecenteAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<SnapshotFaturamento>> ObterSerieDiariaAsync(Guid idEmpresa, int dias, CancellationToken ct = default);
    Task<IReadOnlyList<SnapshotFaturamento>> ObterPorPeriodoAsync(Guid idEmpresa, DateOnly inicio, DateOnly fim, CancellationToken ct = default);
    Task<SnapshotFaturamento> InserirAsync(SnapshotFaturamento snapshot, CancellationToken ct = default);
}

public interface IVendaRepositorio
{
    Task<IReadOnlyList<Venda>> ObterMesAtualAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<Venda>> ObterPorPeriodoAsync(Guid idEmpresa, DateOnly inicio, DateOnly fim, CancellationToken ct = default);
    Task<Venda> InserirAsync(Venda venda, CancellationToken ct = default);
}

public interface IFunilRepositorio
{
    Task<IReadOnlyList<EtapaFunil>> ObterEtapasAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<EtapaFunil> InserirOuAtualizarEtapaAsync(EtapaFunil etapa, CancellationToken ct = default);
}

public interface IInadimplenciaRepositorio
{
    Task<IReadOnlyList<RegistroInadimplencia>> ObterAbertasAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<RegistroInadimplencia> InserirAsync(RegistroInadimplencia registro, CancellationToken ct = default);
}

public interface IGargaloRepositorio
{
    Task<IReadOnlyList<Gargalo>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<Gargalo> InserirAsync(Gargalo gargalo, CancellationToken ct = default);
}

public interface IClienteRepositorio
{
    Task<IReadOnlyList<Cliente>> ObterTodosAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IEquipeRepositorio
{
    Task<IReadOnlyList<MembroEquipe>> ObterTodosAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IMetaRepositorio
{
    Task<IReadOnlyList<Meta>> ObterAtivasAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IAlertaRepositorio
{
    Task<IReadOnlyList<Alerta>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<Alerta> InserirAsync(Alerta alerta, CancellationToken ct = default);
    Task ResolverAsync(Guid idEmpresa, Guid idAlerta, CancellationToken ct = default);
}

public interface ISnapshotDashboardRepositorio
{
    Task<SnapshotDashboard?> ObterMaisRecenteAsync(Guid idEmpresa, CancellationToken ct = default);
    Task SalvarAsync(SnapshotDashboard snapshot, CancellationToken ct = default);
}

public interface IPlanoAcaoRepositorio
{
    Task<IReadOnlyList<ItemPlanoAcaoRegistro>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IInsightRepositorio
{
    Task<IReadOnlyList<InsightInteligente>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IContaReceberRepositorio
{
    Task<IReadOnlyList<ContaReceber>> ObterProximos60DiasAsync(Guid idEmpresa, CancellationToken ct = default);
}

public interface IConversaIaRepositorio
{
    Task<IReadOnlyList<MensagemConversaIa>> ObterHistoricoAsync(
        Guid idEmpresa, Guid idUsuario, int limite = 50, CancellationToken ct = default);
    Task<MensagemConversaIa> InserirAsync(MensagemConversaIa mensagem, CancellationToken ct = default);
    Task LimparHistoricoAsync(Guid idEmpresa, Guid idUsuario, CancellationToken ct = default);
}

public interface IDocumentoConhecimentoRepositorio
{
    Task<IReadOnlyList<DocumentoConhecimento>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default);
    Task<IReadOnlyList<DocumentoConhecimento>> BuscarAsync(
        Guid idEmpresa, string consulta, int limite = 3, CancellationToken ct = default);
}
