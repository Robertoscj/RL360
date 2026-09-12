namespace RL360.Shared.Responses;

/// <summary>Padrão de resposta unificado de todas as APIs RL360.</summary>
public sealed class RespostaModel<T>
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;
    public T? Dados { get; init; }
    public IReadOnlyList<string> Erros { get; init; } = [];

    public static RespostaModel<T> Ok(T dados, string mensagem = "Operação realizada com sucesso.")
        => new() { Sucesso = true, Mensagem = mensagem, Dados = dados };

    public static RespostaModel<T> Falha(string mensagem, IEnumerable<string>? erros = null)
        => new()
        {
            Sucesso = false,
            Mensagem = mensagem,
            Erros = erros?.ToList() ?? [mensagem]
        };
}

public static class RespostaModel
{
    public static RespostaModel<T> Ok<T>(T dados, string mensagem = "Operação realizada com sucesso.")
        => RespostaModel<T>.Ok(dados, mensagem);

    public static RespostaModel<object> Falha(string mensagem, IEnumerable<string>? erros = null)
        => RespostaModel<object>.Falha(mensagem, erros);
}
