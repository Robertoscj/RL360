using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RL360.Application.Abstractions;
using RL360.Shared.Constants;

namespace RL360.Infrastructure.Messaging;

public sealed class ConfiguracaoRabbitMq
{
    public bool Habilitado { get; set; }
    public string Host { get; set; } = "localhost";
    public int Porta { get; set; } = 5672;
    public string Usuario { get; set; } = "guest";
    public string Senha { get; set; } = "guest";
}

public sealed record MensagemRecalculoRadar(Guid IdEmpresa, DateTime SolicitadoEmUtc);

public sealed class PublicadorEventosRabbitMq(ConfiguracaoRabbitMq config, ILogger<PublicadorEventosRabbitMq> logger)
    : IEventoPublicador, IAsyncDisposable
{
    private IConnection? _conexao;
    private IChannel? _canal;
    private readonly SemaphoreSlim _semaforo = new(1, 1);

    private async Task GarantirCanalAsync(CancellationToken ct)
    {
        if (_canal is { IsOpen: true }) return;

        await _semaforo.WaitAsync(ct);
        try
        {
            if (_canal is { IsOpen: true }) return;

            var factory = new ConnectionFactory
            {
                HostName = config.Host,
                Port = config.Porta,
                UserName = config.Usuario,
                Password = config.Senha
            };

            _conexao = await factory.CreateConnectionAsync(ct);
            _canal = await _conexao.CreateChannelAsync(cancellationToken: ct);

            await _canal.QueueDeclareAsync(Filas.RecalculoRadar, durable: true, exclusive: false,
                autoDelete: false, cancellationToken: ct);
        }
        finally
        {
            _semaforo.Release();
        }
    }

    public async Task PublicarRecalculoRadarAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        try
        {
            await GarantirCanalAsync(ct);
            var corpo = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(new MensagemRecalculoRadar(idEmpresa, DateTime.UtcNow)));

            await _canal!.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: Filas.RecalculoRadar,
                body: corpo,
                cancellationToken: ct);

            logger.LogInformation("Recálculo do radar publicado para empresa {IdEmpresa}", idEmpresa);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao publicar recálculo do radar para empresa {IdEmpresa}", idEmpresa);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_canal is not null) await _canal.DisposeAsync();
        if (_conexao is not null) await _conexao.DisposeAsync();
    }
}

public sealed class PublicadorEventosVazio(ILogger<PublicadorEventosVazio> logger) : IEventoPublicador
{
    public Task PublicarRecalculoRadarAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        logger.LogDebug("RabbitMQ desabilitado — ignorando publicação para empresa {IdEmpresa}", idEmpresa);
        return Task.CompletedTask;
    }
}
