using RL360.Domain.Common;

namespace RL360.Domain.Entities;

/// <summary>Empresa / tenant da plataforma SaaS.</summary>
public sealed class Empresa : Entity
{
    public string Nome { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty; // CNPJ
    public string Segmento { get; private set; } = string.Empty;
    public string CodigoPlano { get; private set; } = "pro";
    public bool Ativo { get; private set; } = true;

    private Empresa() { }

    public Empresa(string nome, string documento, string segmento, string codigoPlano = "pro")
    {
        Nome = nome;
        Documento = documento;
        Segmento = segmento;
        CodigoPlano = codigoPlano;
    }
}
