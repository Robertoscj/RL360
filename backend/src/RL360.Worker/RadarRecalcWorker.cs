using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RL360.Application.Abstractions;
using RL360.Application.Services;
using RL360.Infrastructure.Messaging;
using RL360.Shared.Constants;

namespace RL360.Worker;

public sealed class WorkerRecalculoRadar(
    IServiceScopeFactory fabricaEscopo,
    ConfiguracaoRabbitMq rabbit,
    IConfiguration config,
    ILogger<WorkerRecalculoRadar> logger) : BackgroundService
{
    private IConnection? _conexao;
    private IChannel? _canal;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (rabbit.Habilitado)
            await IniciarConsumidorRabbitAsync(stoppingToken);
        else
            logger.LogInformation("RabbitMQ desabilitado — RL360.Worker em modo de atualização periódica.");

        var intervaloSegundos = config.GetValue("Worker:IntervaloAtualizacaoSegundos",
            config.GetValue("Worker:RefreshIntervalSeconds", 15));
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervaloSegundos));

        do
        {
            try
            {
                await RecalcularTodasEmpresasAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha na atualização periódica do radar");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RecalcularTodasEmpresasAsync(CancellationToken ct)
    {
        using var escopo = fabricaEscopo.CreateScope();
        var empresas = escopo.ServiceProvider.GetRequiredService<IEmpresaRepositorio>();
        var processador = escopo.ServiceProvider.GetRequiredService<IProcessadorDashboardServico>();

        foreach (var empresa in await empresas.ObterTodosAsync(ct))
            await processador.ProcessarEmpresaAsync(empresa.Id, ct);
    }

    private async Task RecalcularEmpresaAsync(Guid idEmpresa, CancellationToken ct)
    {
        using var escopo = fabricaEscopo.CreateScope();
        var processador = escopo.ServiceProvider.GetRequiredService<IProcessadorDashboardServico>();
        await processador.ProcessarEmpresaAsync(idEmpresa, ct);
    }

    private async Task IniciarConsumidorRabbitAsync(CancellationToken ct)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbit.Host,
                Port = rabbit.Porta,
                UserName = rabbit.Usuario,
                Password = rabbit.Senha
            };

            _conexao = await factory.CreateConnectionAsync(ct);
            _canal = await _conexao.CreateChannelAsync(cancellationToken: ct);
            await _canal.QueueDeclareAsync(Filas.RecalculoRadar, durable: true, exclusive: false,
                autoDelete: false, cancellationToken: ct);

            var consumidor = new AsyncEventingBasicConsumer(_canal);
            consumidor.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var msg = JsonSerializer.Deserialize<MensagemRecalculoRadar>(json);
                    if (msg is not null)
                    {
                        logger.LogInformation("Evento de recálculo recebido para empresa {IdEmpresa}", msg.IdEmpresa);
                        await RecalcularEmpresaAsync(msg.IdEmpresa, ct);
                    }
                    await _canal!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falha ao processar mensagem de recálculo");
                }
            };

            await _canal.BasicConsumeAsync(Filas.RecalculoRadar, autoAck: false, consumidor, ct);
            logger.LogInformation("RL360.Worker inscrito na fila {Fila}", Filas.RecalculoRadar);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Não foi possível iniciar consumidor RabbitMQ — usando apenas atualização periódica.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_canal is not null) await _canal.DisposeAsync();
        if (_conexao is not null) await _conexao.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
