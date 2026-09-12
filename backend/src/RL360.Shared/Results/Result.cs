namespace RL360.Shared.Results;

/// <summary>Resultado de operação usado nos serviços de aplicação.</summary>
public class Resultado
{
    public bool Sucesso { get; protected set; }
    public string? Erro { get; protected set; }
    public IReadOnlyList<string> Erros { get; protected set; } = [];

    public static Resultado Ok() => new() { Sucesso = true };
    public static Resultado Falha(string erro) => new() { Sucesso = false, Erro = erro, Erros = [erro] };
    public static Resultado Falha(IEnumerable<string> erros)
    {
        var lista = erros.ToList();
        return new Resultado { Sucesso = false, Erro = lista.FirstOrDefault(), Erros = lista };
    }
}

public sealed class Resultado<T> : Resultado
{
    public T? Valor { get; private set; }

    public static Resultado<T> Ok(T valor) => new() { Sucesso = true, Valor = valor };
    public static new Resultado<T> Falha(string erro) => new() { Sucesso = false, Erro = erro, Erros = [erro] };
    public static new Resultado<T> Falha(IEnumerable<string> erros)
    {
        var lista = erros.ToList();
        return new Resultado<T> { Sucesso = false, Erro = lista.FirstOrDefault(), Erros = lista };
    }
}
