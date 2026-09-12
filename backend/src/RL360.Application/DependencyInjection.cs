using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RL360.Application.Services;
using RL360.Application.Services.Ia;
using RL360.Domain.Services;

namespace RL360.Application;

public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => { }, assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddSingleton<CalculadoraRadar>();

        services.AddScoped<MontadorEntradasRadar>();
        services.AddScoped<IRadarServico, RadarServico>();
        services.AddScoped<IDashboardServico, DashboardServico>();
        services.AddScoped<ISnapshotDashboardServico, SnapshotDashboardServico>();
        services.AddScoped<IProcessadorDashboardServico, ProcessadorDashboardServico>();
        services.AddScoped<GeradorAlertasServico>();
        services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();
        services.AddScoped<IModulosServico, ModulosServico>();
        services.AddScoped<IModulosEscritaServico, ModulosEscritaServico>();
        services.AddScoped<IRagServico, RagServico>();
        services.AddScoped<IaProvedorChat>();
        services.AddScoped<IIaConsultorServico, IaConsultorServico>();

        return services;
    }
}
