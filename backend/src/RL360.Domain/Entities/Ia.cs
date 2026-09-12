namespace RL360.Domain.Entities;

/// <summary>Mensagem persistida do chat "Pergunte para a IA".</summary>
public sealed class MensagemConversaIa : Common.Entity, Common.IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public Guid IdUsuario { get; private set; }
    public Guid IdConversa { get; private set; }
    public string Papel { get; private set; } = string.Empty;
    public string Conteudo { get; private set; } = string.Empty;
    public string Modo { get; private set; } = "Consultor";

    private MensagemConversaIa() { }

    public MensagemConversaIa(
        Guid idEmpresa,
        Guid idUsuario,
        Guid idConversa,
        string papel,
        string conteudo,
        string modo)
    {
        IdEmpresa = idEmpresa;
        IdUsuario = idUsuario;
        IdConversa = idConversa;
        Papel = papel;
        Conteudo = conteudo;
        Modo = modo;
    }
}

/// <summary>Fragmento de conhecimento indexado para RAG.</summary>
public sealed class DocumentoConhecimento : Common.Entity, Common.IEmpresaProprietaria
{
    public Guid IdEmpresa { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public string Conteudo { get; private set; } = string.Empty;
    public bool Ativo { get; private set; } = true;

    private DocumentoConhecimento() { }

    public DocumentoConhecimento(Guid idEmpresa, string titulo, string categoria, string conteudo)
    {
        IdEmpresa = idEmpresa;
        Titulo = titulo;
        Categoria = categoria;
        Conteudo = conteudo;
    }
}
