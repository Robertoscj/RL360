using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RL360.Application.Abstractions;
using RL360.Infrastructure.Persistence.Relational;

namespace RL360.Infrastructure.Persistence;

internal static class RegistroRepositoriosRelacionais
{
    internal static void Registrar(IServiceCollection services)
    {
        services.AddScoped<IEmpresaRepositorio, EmpresaRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IFaturamentoRepositorio, FaturamentoRepositorio>();
        services.AddScoped<IVendaRepositorio, VendaRepositorio>();
        services.AddScoped<IFunilRepositorio, FunilRepositorio>();
        services.AddScoped<IInadimplenciaRepositorio, InadimplenciaRepositorio>();
        services.AddScoped<IGargaloRepositorio, GargaloRepositorio>();
        services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        services.AddScoped<IEquipeRepositorio, EquipeRepositorio>();
        services.AddScoped<IMetaRepositorio, MetaRepositorio>();
        services.AddScoped<IAlertaRepositorio, AlertaRepositorio>();
        services.AddScoped<ISnapshotDashboardRepositorio, SnapshotDashboardRepositorio>();
        services.AddScoped<IPlanoAcaoRepositorio, PlanoAcaoRepositorio>();
        services.AddScoped<IInsightRepositorio, InsightRepositorio>();
        services.AddScoped<IContaReceberRepositorio, ContaReceberRepositorio>();
        services.AddScoped<IConversaIaRepositorio, ConversaIaRepositorio>();
        services.AddScoped<IDocumentoConhecimentoRepositorio, DocumentoConhecimentoRepositorio>();
    }

    internal static bool TentarRegistrar(
        IServiceCollection services,
        IConfiguration config,
        string fonteDados)
    {
        if (EhPostgres(fonteDados))
        {
            var cs = config.GetConnectionString("Postgres")
                     ?? throw new InvalidOperationException(
                         "ConnectionStrings:Postgres é obrigatório para FonteDados=Postgres.");
            services.AddSingleton<IDialectoSql, DialectoPostgres>();
            services.AddSingleton<IFabricaConexaoBanco>(new FabricaConexaoPostgres(cs));
            Registrar(services);
            return true;
        }

        if (EhSqlServer(fonteDados))
        {
            var cs = config.GetConnectionString("SqlServer")
                     ?? throw new InvalidOperationException(
                         "ConnectionStrings:SqlServer é obrigatório para FonteDados=SqlServer.");
            services.AddSingleton<IDialectoSql, DialectoSqlServer>();
            services.AddSingleton<IFabricaConexaoBanco>(new FabricaConexaoSqlServer(cs));
            Registrar(services);
            return true;
        }

        return false;
    }

    private static bool EhPostgres(string fonteDados) =>
        fonteDados.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
        || fonteDados.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase);

    private static bool EhSqlServer(string fonteDados) =>
        fonteDados.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
        || fonteDados.Equals("SQLServer", StringComparison.OrdinalIgnoreCase);
}
