using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RL360.Application.Abstractions;
using RL360.Infrastructure.Caching;
using RL360.Infrastructure.Messaging;
using RL360.Infrastructure.Persistence;
using RL360.Infrastructure.Persistence.Demo;
using RL360.Infrastructure.Security;
using StackExchange.Redis;

namespace RL360.Infrastructure;

public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ConfiguracaoJwt>(config.GetSection("Jwt"));
        services.AddSingleton<IHashSenhaServico, ServicoHashSenhaBcrypt>();
        services.AddSingleton<IJwtTokenServico, ServicoJwtToken>();
        services.AddSingleton<IRefreshTokenServico, ServicoRefreshToken>();

        var fonteDados = config["Rl360:FonteDados"] ?? config["Rl360:DataSource"] ?? "Demo";
        if (!RegistroRepositoriosRelacionais.TentarRegistrar(services, config, fonteDados))
        {
            services.AddSingleton<ArmazenamentoDemo>();
            services.AddSingleton<IEmpresaRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IUsuarioRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IFaturamentoRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IVendaRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IFunilRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IInadimplenciaRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IGargaloRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IClienteRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IEquipeRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IMetaRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IAlertaRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<ISnapshotDashboardRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IPlanoAcaoRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IInsightRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IContaReceberRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IConversaIaRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
            services.AddSingleton<IDocumentoConhecimentoRepositorio>(sp => sp.GetRequiredService<ArmazenamentoDemo>());
        }

        var redisHabilitado = config.GetValue("Redis:Habilitado", config.GetValue("Redis:Enabled", false));
        if (redisHabilitado)
        {
            var redisCs = config["Redis:ConnectionString"] ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisCs));
            services.AddSingleton<ICacheServico, ServicoCacheRedis>();
        }
        else
        {
            services.AddSingleton<ICacheServico, ServicoCacheMemoria>();
        }

        var rabbit = config.GetSection("RabbitMq").Get<ConfiguracaoRabbitMq>() ?? new ConfiguracaoRabbitMq();
        if (!config.GetSection("RabbitMq").Exists() || rabbit.Habilitado == false && config.GetValue("RabbitMq:Enabled", false))
            rabbit.Habilitado = config.GetValue("RabbitMq:Enabled", false);
        services.AddSingleton(rabbit);
        if (rabbit.Habilitado)
            services.AddSingleton<IEventoPublicador, PublicadorEventosRabbitMq>();
        else
            services.AddSingleton<IEventoPublicador, PublicadorEventosVazio>();

        return services;
    }
}
