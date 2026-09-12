using System.Collections.Concurrent;
using System.Text.Json;
using RL360.Application.Abstractions;
using StackExchange.Redis;

namespace RL360.Infrastructure.Caching;

public sealed class ServicoCacheRedis(IConnectionMultiplexer mux) : ICacheServico
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public async Task<T?> ObterAsync<T>(string chave, CancellationToken ct = default)
    {
        var valor = await mux.GetDatabase().StringGetAsync(chave);
        return valor.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>((string)valor!, JsonOpts);
    }

    public Task DefinirAsync<T>(string chave, T valor, TimeSpan? ttl = null, CancellationToken ct = default)
        => mux.GetDatabase().StringSetAsync(chave, JsonSerializer.Serialize(valor, JsonOpts), ttl);

    public Task RemoverAsync(string chave, CancellationToken ct = default)
        => mux.GetDatabase().KeyDeleteAsync(chave);
}

public sealed class ServicoCacheMemoria : ICacheServico
{
    private sealed record Entrada(string Json, DateTime? ExpiraEmUtc);

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);
    private readonly ConcurrentDictionary<string, Entrada> _armazenamento = new();

    public Task<T?> ObterAsync<T>(string chave, CancellationToken ct = default)
    {
        if (_armazenamento.TryGetValue(chave, out var entrada))
        {
            if (entrada.ExpiraEmUtc is null || entrada.ExpiraEmUtc > DateTime.UtcNow)
                return Task.FromResult(JsonSerializer.Deserialize<T>(entrada.Json, JsonOpts));

            _armazenamento.TryRemove(chave, out _);
        }
        return Task.FromResult<T?>(default);
    }

    public Task DefinirAsync<T>(string chave, T valor, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var expira = ttl is null ? (DateTime?)null : DateTime.UtcNow.Add(ttl.Value);
        _armazenamento[chave] = new Entrada(JsonSerializer.Serialize(valor, JsonOpts), expira);
        return Task.CompletedTask;
    }

    public Task RemoverAsync(string chave, CancellationToken ct = default)
    {
        _armazenamento.TryRemove(chave, out _);
        return Task.CompletedTask;
    }
}
