using RL360.Application;
using RL360.Application.Abstractions;
using RL360.Infrastructure;
using RL360.Worker;
using RL360.Worker.Realtime;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AdicionarAplicacao();
builder.Services.AdicionarInfraestrutura(builder.Configuration);

var urlBaseApi = builder.Configuration["Api:UrlBase"] ?? builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5080";
builder.Services.AddSingleton<NotificadorDashboardClienteSignalR>(sp => new NotificadorDashboardClienteSignalR(
    sp.GetRequiredService<IJwtTokenServico>(),
    urlBaseApi,
    sp.GetRequiredService<ILogger<NotificadorDashboardClienteSignalR>>()));
builder.Services.AddSingleton<IRadarNotificador>(sp => sp.GetRequiredService<NotificadorDashboardClienteSignalR>());
builder.Services.AddSingleton<IDashboardNotificador>(sp => sp.GetRequiredService<NotificadorDashboardClienteSignalR>());

builder.Services.AddHostedService<WorkerRecalculoRadar>();

var host = builder.Build();
host.Run();
