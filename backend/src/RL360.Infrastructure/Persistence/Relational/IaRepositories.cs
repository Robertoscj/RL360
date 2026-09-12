using Dapper;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;

namespace RL360.Infrastructure.Persistence.Relational;

public sealed class ConversaIaRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IConversaIaRepositorio
{
    public async Task<IReadOnlyList<MensagemConversaIa>> ObterHistoricoAsync(
        Guid idEmpresa, Guid idUsuario, int limite = 50, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        var prefixo = dialecto.SelecionarLimiteParametrizado("@limite");
        var sufixo = dialecto.SufixoLimiteParametrizado("@limite");
        return (await db.QueryAsync<MensagemConversaIa>($@"
            SELECT {prefixo} Id, TenantId AS IdEmpresa, UserId AS IdUsuario,
                   ConversationId AS IdConversa, Role AS Papel, Content AS Conteudo,
                   Mode AS Modo, CreatedAtUtc AS CriadoEmUtc
            FROM MensagensConversaIa
            WHERE TenantId = @idEmpresa AND UserId = @idUsuario
            ORDER BY CreatedAtUtc DESC{sufixo}",
            new { idEmpresa, idUsuario, limite }))
            .Reverse()
            .ToList();
    }

    public async Task<MensagemConversaIa> InserirAsync(MensagemConversaIa mensagem, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(@"
            INSERT INTO MensagensConversaIa
                (Id, TenantId, UserId, ConversationId, Role, Content, Mode, CreatedAtUtc)
            VALUES
                (@Id, @IdEmpresa, @IdUsuario, @IdConversa, @Papel, @Conteudo, @Modo, @CriadoEmUtc)",
            new
            {
                mensagem.Id,
                mensagem.IdEmpresa,
                mensagem.IdUsuario,
                mensagem.IdConversa,
                mensagem.Papel,
                mensagem.Conteudo,
                mensagem.Modo,
                mensagem.CriadoEmUtc
            });
        return mensagem;
    }

    public async Task LimparHistoricoAsync(Guid idEmpresa, Guid idUsuario, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        await db.ExecuteAsync(
            "DELETE FROM MensagensConversaIa WHERE TenantId = @idEmpresa AND UserId = @idUsuario",
            new { idEmpresa, idUsuario });
    }
}

public sealed class DocumentoConhecimentoRepositorio(IFabricaConexaoBanco fabrica, IDialectoSql dialecto) : IDocumentoConhecimentoRepositorio
{
    public async Task<IReadOnlyList<DocumentoConhecimento>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default)
    {
        using var db = fabrica.Criar();
        return (await db.QueryAsync<DocumentoConhecimento>($@"
            SELECT Id, TenantId AS IdEmpresa, Title AS Titulo, Category AS Categoria,
                   Content AS Conteudo, IsActive AS Ativo, CreatedAtUtc AS CriadoEmUtc
            FROM DocumentosConhecimento
            WHERE TenantId = @idEmpresa AND IsActive = {dialecto.LiteralVerdadeiro}",
            new { idEmpresa })).ToList();
    }

    public async Task<IReadOnlyList<DocumentoConhecimento>> BuscarAsync(
        Guid idEmpresa, string consulta, int limite = 3, CancellationToken ct = default)
    {
        var docs = await ObterAtivosAsync(idEmpresa, ct);
        if (docs.Count == 0) return [];

        var termos = consulta
            .ToLowerInvariant()
            .Split([' ', ',', '.', '?', '!'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => t.Length > 2)
            .Distinct()
            .ToList();

        if (termos.Count == 0)
            return docs.Take(limite).ToList();

        return docs
            .Select(d => new
            {
                Doc = d,
                Score = termos.Count(t =>
                    d.Titulo.Contains(t, StringComparison.OrdinalIgnoreCase)
                    || d.Conteudo.Contains(t, StringComparison.OrdinalIgnoreCase)
                    || d.Categoria.Contains(t, StringComparison.OrdinalIgnoreCase))
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(limite)
            .Select(x => x.Doc)
            .ToList();
    }
}
