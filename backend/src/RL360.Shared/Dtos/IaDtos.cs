namespace RL360.Shared.Dtos;

public sealed class RequisicaoIaPergunta
{
    public string Pergunta { get; set; } = string.Empty;
    public DateOnly? Inicio { get; set; }
    public DateOnly? Fim { get; set; }
    public Guid? IdConversa { get; set; }
}

public sealed class RespostaIaPergunta
{
    public string Resposta { get; set; } = string.Empty;
    public string Modo { get; set; } = "Demo";
    public Guid IdConversa { get; set; }
    public DateTime GeradoEmUtc { get; set; }
}

public sealed class MensagemIaDto
{
    public Guid Id { get; set; }
    public Guid IdConversa { get; set; }
    public string Papel { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string Modo { get; set; } = string.Empty;
    public DateTime CriadoEmUtc { get; set; }
}

public sealed class HistoricoIaDto
{
    public Guid IdConversa { get; set; }
    public List<MensagemIaDto> Mensagens { get; set; } = [];
}

public sealed class EventoStreamIaDto
{
    public string Tipo { get; set; } = string.Empty;
    public string? Delta { get; set; }
    public string? Modo { get; set; }
    public Guid? IdConversa { get; set; }
    public string? Erro { get; set; }
}
