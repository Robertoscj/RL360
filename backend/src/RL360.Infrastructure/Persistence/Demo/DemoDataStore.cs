using RL360.Application.Abstractions;
using RL360.Domain.Entities;
using RL360.Domain.Enums;

namespace RL360.Infrastructure.Persistence.Demo;

/// <summary>
/// Repositório em memória com dados de demonstração (Rl360:FonteDados=Demo).
/// </summary>
public sealed class ArmazenamentoDemo :
    IEmpresaRepositorio, IUsuarioRepositorio, IFaturamentoRepositorio, IVendaRepositorio, IFunilRepositorio,
    IInadimplenciaRepositorio, IGargaloRepositorio, IClienteRepositorio, IEquipeRepositorio,
    IMetaRepositorio, IAlertaRepositorio, ISnapshotDashboardRepositorio, IPlanoAcaoRepositorio,
    IInsightRepositorio, IContaReceberRepositorio, IConversaIaRepositorio, IDocumentoConhecimentoRepositorio
{
    public static readonly Guid IdEmpresaDemo = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid IdUsuarioDemo = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public const string EmailDemo = "ceo@rl360.com";
    public const string SenhaDemo = "rl360@2026";

    private readonly List<Empresa> _empresas = [];
    private readonly List<Usuario> _usuarios = [];
    private readonly List<SnapshotFaturamento> _faturamento = [];
    private readonly List<Venda> _vendas = [];
    private readonly List<EtapaFunil> _funil = [];
    private readonly List<RegistroInadimplencia> _inadimplencia = [];
    private readonly List<Gargalo> _gargalos = [];
    private readonly List<Cliente> _clientes = [];
    private readonly List<MembroEquipe> _equipe = [];
    private readonly List<Meta> _metas = [];
    private readonly List<Alerta> _alertas = [];
    private readonly Dictionary<Guid, SnapshotDashboard> _snapshots = [];
    private readonly List<ItemPlanoAcaoRegistro> _planoAcao = [];
    private readonly List<InsightInteligente> _insights = [];
    private readonly List<ContaReceber> _contasReceber = [];
    private readonly List<MensagemConversaIa> _conversasIa = [];
    private readonly List<DocumentoConhecimento> _documentos = [];

    public ArmazenamentoDemo(IHashSenhaServico hashSenha)
    {
        var empresaDemo = ComId(new Empresa("Grupo Aurora Distribuição", "12.345.678/0001-90", "Distribuição", "scale"), IdEmpresaDemo);
        var usuarioDemo = ComId(new Usuario(IdEmpresaDemo, "Roberto Silva", EmailDemo, hashSenha.GerarHash(SenhaDemo), PerfilUsuario.Proprietario), IdUsuarioDemo);
        _empresas.Add(empresaDemo);
        _usuarios.Add(usuarioDemo);

        SemearFaturamento();
        SemearVendas();
        SemearFunil();
        SemearInadimplencia();
        SemearGargalos();
        SemearClientes();
        SemearEquipe();
        SemearMetas();
        SemearAlertas();
        SemearInsights();
        SemearPlanoAcao();
        SemearContasReceber();
        SemearDocumentosConhecimento();
        _snapshots[IdEmpresaDemo] = SemearSnapshotDashboard();
    }

    public Task<Empresa?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_empresas.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<Empresa>> ObterTodosAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Empresa>>(_empresas);

    public Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct = default)
        => Task.FromResult(_empresas.Any(e => string.Equals(e.Documento, documento.Trim(), StringComparison.OrdinalIgnoreCase)));

    public Task<Empresa> CriarAsync(Empresa empresa, CancellationToken ct = default)
    {
        _empresas.Add(empresa);
        return Task.FromResult(empresa);
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult(_usuarios.FirstOrDefault(u =>
            string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)));

    Task<Usuario?> IUsuarioRepositorio.ObterPorIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_usuarios.FirstOrDefault(u => u.Id == id));

    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult(_usuarios.Any(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)));

    public Task<Usuario> CriarAsync(Usuario usuario, CancellationToken ct = default)
    {
        _usuarios.Add(usuario);
        return Task.FromResult(usuario);
    }

    Task<SnapshotFaturamento?> IFaturamentoRepositorio.ObterMaisRecenteAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult(_faturamento.Where(f => f.IdEmpresa == idEmpresa).OrderByDescending(r => r.DataReferencia).FirstOrDefault());

    public Task<IReadOnlyList<SnapshotFaturamento>> ObterSerieDiariaAsync(Guid idEmpresa, int dias, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<SnapshotFaturamento>>(
            _faturamento.Where(f => f.IdEmpresa == idEmpresa).OrderByDescending(r => r.DataReferencia).Take(dias).OrderBy(r => r.DataReferencia).ToList());

    Task<IReadOnlyList<SnapshotFaturamento>> IFaturamentoRepositorio.ObterPorPeriodoAsync(
        Guid idEmpresa, DateOnly inicio, DateOnly fim, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<SnapshotFaturamento>>(
            _faturamento
                .Where(f => f.IdEmpresa == idEmpresa
                    && DateOnly.FromDateTime(f.DataReferencia) >= inicio
                    && DateOnly.FromDateTime(f.DataReferencia) <= fim)
                .OrderBy(f => f.DataReferencia)
                .ToList());

    public Task<SnapshotFaturamento> InserirAsync(SnapshotFaturamento snapshot, CancellationToken ct = default)
    {
        _faturamento.Add(snapshot);
        return Task.FromResult(snapshot);
    }

    public Task<IReadOnlyList<Venda>> ObterMesAtualAsync(Guid idEmpresa, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Venda>>(_vendas.Where(v => v.IdEmpresa == idEmpresa).ToList());

    Task<IReadOnlyList<Venda>> IVendaRepositorio.ObterPorPeriodoAsync(
        Guid idEmpresa, DateOnly inicio, DateOnly fim, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Venda>>(
            _vendas.Where(v =>
                v.IdEmpresa == idEmpresa
                && DateOnly.FromDateTime(v.FechadaEmUtc) >= inicio
                && DateOnly.FromDateTime(v.FechadaEmUtc) <= fim).ToList());

    public Task<Venda> InserirAsync(Venda venda, CancellationToken ct = default)
    {
        _vendas.Add(venda);
        return Task.FromResult(venda);
    }

    public Task<IReadOnlyList<EtapaFunil>> ObterEtapasAsync(Guid idEmpresa, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<EtapaFunil>>(_funil.Where(f => f.IdEmpresa == idEmpresa).OrderBy(f => f.Ordem).ToList());

    public Task<EtapaFunil> InserirOuAtualizarEtapaAsync(EtapaFunil etapa, CancellationToken ct = default)
    {
        var existente = _funil.FirstOrDefault(f => f.IdEmpresa == etapa.IdEmpresa && f.Ordem == etapa.Ordem);
        if (existente is not null) _funil.Remove(existente);
        _funil.Add(etapa);
        return Task.FromResult(etapa);
    }

    public Task<IReadOnlyList<RegistroInadimplencia>> ObterAbertasAsync(Guid idEmpresa, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<RegistroInadimplencia>>(_inadimplencia.Where(d => d.IdEmpresa == idEmpresa).OrderByDescending(d => d.Valor).ToList());

    public Task<RegistroInadimplencia> InserirAsync(RegistroInadimplencia registro, CancellationToken ct = default)
    {
        _inadimplencia.Add(registro);
        return Task.FromResult(registro);
    }

    public Task<IReadOnlyList<Gargalo>> ObterAtivosAsync(Guid idEmpresa, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Gargalo>>(_gargalos.Where(g => g.IdEmpresa == idEmpresa).ToList());

    public Task<Gargalo> InserirAsync(Gargalo gargalo, CancellationToken ct = default)
    {
        _gargalos.Add(gargalo);
        return Task.FromResult(gargalo);
    }

    Task<IReadOnlyList<Alerta>> IAlertaRepositorio.ObterAtivosAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Alerta>>(_alertas.Where(a => a.IdEmpresa == idEmpresa && !a.Resolvido).OrderByDescending(a => a.Severidade).ToList());

    public Task<Alerta> InserirAsync(Alerta alerta, CancellationToken ct = default)
    {
        _alertas.Add(alerta);
        return Task.FromResult(alerta);
    }

    Task<IReadOnlyList<Cliente>> IClienteRepositorio.ObterTodosAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Cliente>>(_clientes.Where(c => c.IdEmpresa == idEmpresa).ToList());

    Task<IReadOnlyList<MembroEquipe>> IEquipeRepositorio.ObterTodosAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<MembroEquipe>>(_equipe.Where(e => e.IdEmpresa == idEmpresa).ToList());

    Task<IReadOnlyList<Meta>> IMetaRepositorio.ObterAtivasAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Meta>>(_metas.Where(m => m.IdEmpresa == idEmpresa).ToList());

    Task IAlertaRepositorio.ResolverAsync(Guid idEmpresa, Guid idAlerta, CancellationToken ct)
    {
        var alerta = _alertas.FirstOrDefault(a => a.Id == idAlerta && a.IdEmpresa == idEmpresa);
        alerta?.Resolver();
        return Task.CompletedTask;
    }

    Task<SnapshotDashboard?> ISnapshotDashboardRepositorio.ObterMaisRecenteAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult(_snapshots.TryGetValue(idEmpresa, out var snap) ? snap : null);

    Task ISnapshotDashboardRepositorio.SalvarAsync(SnapshotDashboard snapshot, CancellationToken ct)
    {
        _snapshots[snapshot.IdEmpresa] = snapshot;
        return Task.CompletedTask;
    }
    Task<IReadOnlyList<ItemPlanoAcaoRegistro>> IPlanoAcaoRepositorio.ObterAtivosAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<ItemPlanoAcaoRegistro>>(_planoAcao);

    Task<IReadOnlyList<InsightInteligente>> IInsightRepositorio.ObterAtivosAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<InsightInteligente>>(_insights);

    Task<IReadOnlyList<ContaReceber>> IContaReceberRepositorio.ObterProximos60DiasAsync(Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<ContaReceber>>(_contasReceber);

    Task<IReadOnlyList<MensagemConversaIa>> IConversaIaRepositorio.ObterHistoricoAsync(
        Guid idEmpresa, Guid idUsuario, int limite, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<MensagemConversaIa>>(_conversasIa
            .Where(m => m.IdEmpresa == idEmpresa && m.IdUsuario == idUsuario)
            .OrderByDescending(m => m.CriadoEmUtc)
            .Take(limite)
            .Reverse()
            .ToList());

    Task<MensagemConversaIa> IConversaIaRepositorio.InserirAsync(MensagemConversaIa mensagem, CancellationToken ct)
    {
        _conversasIa.Add(mensagem);
        return Task.FromResult(mensagem);
    }

    Task IConversaIaRepositorio.LimparHistoricoAsync(Guid idEmpresa, Guid idUsuario, CancellationToken ct)
    {
        _conversasIa.RemoveAll(m => m.IdEmpresa == idEmpresa && m.IdUsuario == idUsuario);
        return Task.CompletedTask;
    }

    Task<IReadOnlyList<DocumentoConhecimento>> IDocumentoConhecimentoRepositorio.ObterAtivosAsync(
        Guid idEmpresa, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<DocumentoConhecimento>>(_documentos.Where(d => d.IdEmpresa == idEmpresa).ToList());

    Task<IReadOnlyList<DocumentoConhecimento>> IDocumentoConhecimentoRepositorio.BuscarAsync(
        Guid idEmpresa, string consulta, int limite, CancellationToken ct)
    {
        var repo = new DocumentoConhecimentoRepositorioBuscaDemo(_documentos.Where(d => d.IdEmpresa == idEmpresa).ToList());
        return repo.BuscarAsync(idEmpresa, consulta, limite, ct);
    }

    private void SemearDocumentosConhecimento()
    {
        void Add(string titulo, string categoria, string conteudo)
            => _documentos.Add(new DocumentoConhecimento(IdEmpresaDemo, titulo, categoria, conteudo));

        Add(
            "Política de análise de crédito",
            "Crédito",
            "Prazo máximo de aprovação: 1 hora útil. Propostas acima de R$ 50.000 exigem segunda alçada. Taxa de conversão cai 3,2pp quando o SLA estoura.");
        Add(
            "Playbook comercial — Região Norte",
            "Vendas",
            "A inadimplência na Região Norte está 18% acima da média. Priorizar clientes com score acima de 720 e oferecer desconto à vista de 2% para antecipação.");
        Add(
            "Metas trimestrais Q2",
            "Metas",
            "Meta de faturamento: R$ 1,5M/mês. Meta de conversão: 25%. Meta de inadimplência máxima: 8% do faturamento. Cross-sell Produto A→B tem 70% de conversão histórica.");
        Add(
            "Régua de cobrança padrão",
            "Financeiro",
            "D+1: lembrete automático. D+7: contato SDR. D+15: escalonamento gerente. D+30: bloqueio de novos pedidos. Recuperação média: 40% entre D+7 e D+15.");
        Add(
            "Gargalo de aprovação comercial",
            "Operações",
            "Tempo médio atual: 3h45m por proposta. Causa principal: fila manual de análise. Automatizar pré-aprovação para tickets abaixo de R$ 15.000 reduz 60% do volume.");
    }

    private sealed class DocumentoConhecimentoRepositorioBuscaDemo(IReadOnlyList<DocumentoConhecimento> docs)
    {
        public Task<IReadOnlyList<DocumentoConhecimento>> BuscarAsync(
            Guid idEmpresa, string consulta, int limite, CancellationToken ct)
        {
            var termos = consulta.ToLowerInvariant()
                .Split([' ', ',', '.', '?', '!'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => t.Length > 2).Distinct().ToList();

            if (termos.Count == 0)
                return Task.FromResult<IReadOnlyList<DocumentoConhecimento>>(docs.Take(limite).ToList());

            var resultado = docs
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

            return Task.FromResult<IReadOnlyList<DocumentoConhecimento>>(resultado);
        }
    }

    private void SemearFaturamento()
    {
        var hoje = DateTime.UtcNow.Date;
        var rnd = new Random(360);
        decimal acumulado = 0;
        var valoresDiarios = new List<(DateTime data, decimal valor)>();
        for (var i = 29; i >= 0; i--)
        {
            var data = hoje.AddDays(-i);
            var fatorFimSemana = data.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 0.55m : 1m;
            var baseValor = 41000m + rnd.Next(-6000, 12000);
            var valor = Math.Round(baseValor * fatorFimSemana, 2);
            valoresDiarios.Add((data, valor));
            acumulado += valor;
        }

        var escala = 1_250_000m / acumulado;
        decimal acumuladoMes = 0;
        foreach (var (data, valor) in valoresDiarios)
        {
            var escalado = data == hoje ? 58_420m : Math.Round(valor * escala, 2);
            acumuladoMes += escalado;
            _faturamento.Add(new SnapshotFaturamento(
                IdEmpresaDemo, data,
                faturamentoDia: escalado,
                faturamentoMes: data == hoje ? 1_250_000m : Math.Round(acumuladoMes, 2),
                metaMensal: 1_500_000m,
                custoMes: 812_680m,
                custoFixoMes: 410_000m));
        }
    }

    private void SemearVendas()
    {
        void Adicionar(string canal, decimal valor, decimal margem, bool clienteNovo, int diasAtras)
            => _vendas.Add(new Venda(IdEmpresaDemo, null, canal, valor, margem,
                DateTime.UtcNow.AddDays(-diasAtras), clienteNovo));

        Adicionar("Inbound", 232_000m, 78_500m, true, 2);
        Adicionar("Outbound", 180_000m, 60_000m, true, 5);
        Adicionar("Indicação", 121_000m, 41_000m, true, 9);
        Adicionar("Marketplace", 104_000m, 35_000m, true, 14);
        Adicionar("Recorrência", 96_000m, 33_500m, false, 1);
        Adicionar("Recorrência", 88_000m, 30_100m, false, 3);
        Adicionar("Upsell", 64_000m, 22_400m, false, 7);
    }

    private void SemearFunil()
    {
        void Adicionar(string nome, int ordem, int quantidade, decimal potencial, decimal conv, decimal convBase)
            => _funil.Add(new EtapaFunil(IdEmpresaDemo, nome, ordem, quantidade, potencial, conv, convBase));

        Adicionar("Lead", 1, 1200, 700_000m, 0.35m, 0.45m);
        Adicionar("Qualificação", 2, 420, 600_000m, 0.28m, 0.34m);
        Adicionar("Proposta", 3, 150, 400_000m, 0.16m, 0.24m);
        Adicionar("Negociação", 4, 60, 200_000m, 0.09m, 0.17m);
    }

    private void SemearInadimplencia()
    {
        void Adicionar(string cliente, decimal valor, int dias, decimal probRecuperacao)
            => _inadimplencia.Add(new RegistroInadimplencia(IdEmpresaDemo, null, cliente, valor, dias, probRecuperacao));

        Adicionar("Atacado Boa Vista", 130_000m, 62, 0.40m);
        Adicionar("Comercial Andaza Ltda", 92_000m, 45, 0.30m);
        Adicionar("Mercado União", 55_000m, 38, 0.40m);
        Adicionar("Distribuidora Sul", 35_000m, 21, 0.7829m);
    }

    private void SemearGargalos()
    {
        void Adicionar(string titulo, string descricao, AreaGargalo area, decimal impacto, bool critico)
            => _gargalos.Add(new Gargalo(IdEmpresaDemo, titulo, descricao, area, impacto, critico));

        Adicionar("Tempo de resposta a leads acima de 12h", "Leads esfriando antes do primeiro contato.", AreaGargalo.Comercial, -42_000m, true);
        Adicionar("Propostas paradas sem follow-up", "62 propostas sem retorno há mais de 7 dias.", AreaGargalo.Comercial, -50_000m, true);
        Adicionar("Atraso recorrente na entrega", "SLA de entrega estourado em 18% dos pedidos.", AreaGargalo.Logistica, -38_000m, true);
        Adicionar("Fila de atendimento sobrecarregada", "Tempo médio de espera de 9 minutos.", AreaGargalo.Atendimento, -27_000m, true);
        Adicionar("Retrabalho no faturamento", "Notas reemitidas por erro de cadastro.", AreaGargalo.Operacional, -15_000m, false);
    }

    private void SemearClientes()
    {
        void Adicionar(string nome, decimal receitaMensal, StatusCliente status, decimal valorAtraso, decimal expansao, int saude, decimal ltv)
            => _clientes.Add(new Cliente(IdEmpresaDemo, nome, receitaMensal, status, valorAtraso, expansao, saude, ltv));

        Adicionar("Rede Horizonte", 120_000m, StatusCliente.OportunidadeExpansao, 0, 90_000m, 100, 1_450_000m);
        Adicionar("Supermercados Vale", 64_000m, StatusCliente.OportunidadeExpansao, 0, 50_000m, 84, 880_000m);
        Adicionar("Atacado Boa Vista", 56_000m, StatusCliente.EmRisco, 130_000m, 0, 35, 410_000m);
        Adicionar("Mercado União", 41_000m, StatusCliente.EmRisco, 55_000m, 0, 35, 220_000m);
        Adicionar("Distribuidora Norte", 98_000m, StatusCliente.Saudavel, 0, 0, 92, 1_120_000m);
        Adicionar("Grupo Primavera", 76_000m, StatusCliente.Saudavel, 0, 0, 88, 940_000m);
        Adicionar("Comercial Aurora", 52_000m, StatusCliente.Saudavel, 0, 0, 81, 610_000m);
        Adicionar("Comercial Andaza Ltda", 38_000m, StatusCliente.Inadimplente, 92_000m, 0, 22, 280_000m);
    }

    private void SemearEquipe()
    {
        void Adicionar(string nome, string cargo, decimal receita, decimal meta, int pontuacao)
            => _equipe.Add(new MembroEquipe(IdEmpresaDemo, nome, cargo, receita, meta, pontuacao));

        Adicionar("Ana Martins", "Closer Sênior", 320_000m, 280_000m, 92);
        Adicionar("Bruno Costa", "Closer Pleno", 245_000m, 230_000m, 80);
        Adicionar("Carla Dias", "SDR Líder", 198_000m, 150_000m, 88);
        Adicionar("Diego Souza", "SDR", 95_500m, 100_000m, 61);
    }

    private void SemearMetas()
    {
        var inicio = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim = inicio.AddMonths(1).AddDays(-1);
        void Adicionar(string nome, decimal meta, decimal atual)
            => _metas.Add(new Meta(IdEmpresaDemo, nome, meta, atual, inicio, fim));

        Adicionar("Faturamento mensal", 1_500_000m, 1_250_000m);
        Adicionar("Novos clientes", 40m, 27m);
        Adicionar("Redução de inadimplência (R$)", 150_000m, 96_000m);
        Adicionar("Conversão do funil (%)", 30m, 22m);
    }

    private void SemearAlertas()
    {
        void Adicionar(string titulo, string mensagem, SeveridadeAlerta severidade, decimal impacto, FatorRadar? fator)
            => _alertas.Add(new Alerta(IdEmpresaDemo, titulo, mensagem, severidade, impacto, fator));

        Adicionar("Inadimplência acima do normal", "Impacto financeiro relevante na região Norte.", SeveridadeAlerta.Critico, -183_000m, FatorRadar.Inadimplencia);
        Adicionar("Aprovação muito lenta", "Análise de crédito com atraso médio de 3h45m.", SeveridadeAlerta.Critico, -92_000m, FatorRadar.GargaloComercial);
        Adicionar("Queda na conversão", "Conversão caiu de 30% para 22%.", SeveridadeAlerta.Critico, -152_000m, FatorRadar.Conversao);
        Adicionar("Oportunidade na região Sul", "Potencial de expansão identificado.", SeveridadeAlerta.Info, 126_000m, FatorRadar.ExpansaoClientes);
        Adicionar("Produto em alta", "Produto A com forte demanda e cross-sell.", SeveridadeAlerta.Info, 186_000m, FatorRadar.VendasNovas);
        Adicionar("Risco de perda em 30 dias", "Você pode perder R$ 487.320 nos próximos 30 dias se nada for feito.", SeveridadeAlerta.Critico, -487_320m, FatorRadar.Inadimplencia);
    }

    private void SemearInsights()
    {
        void Adicionar(string tipo, string titulo, string descricao, string botao, string rota, decimal? impacto = null)
            => _insights.Add(new InsightInteligente(IdEmpresaDemo, tipo, titulo, descricao, botao, rota, impacto));

        Adicionar("AcaoUrgente", "Ação urgente recomendada",
            "Negocie com os clientes da Região Norte. 65% do aumento da inadimplência vem desta região.",
            "Ver clientes", "/clientes", -183_000m);
        Adicionar("Gargalo", "Gargalo identificado",
            "A etapa de análise de crédito está causando atraso médio de 3h45m.",
            "Ver gargalo", "/gargalos", -92_000m);
        Adicionar("Oportunidade", "Oportunidade real",
            "Clientes que compram o Produto A também têm 70% mais chance de comprar o Produto B.",
            "Ver oportunidade", "/vendas", 186_000m);
    }

    private void SemearPlanoAcao()
    {
        void Adicionar(string titulo, string justificativa, decimal impacto, PrioridadeAcao prioridade)
            => _planoAcao.Add(new ItemPlanoAcaoRegistro(IdEmpresaDemo, titulo, justificativa, impacto, prioridade));

        Adicionar("Acionar régua de cobrança nos maiores inadimplentes", "Recuperar parte dos R$ 183.000 em risco.", 73_200m, PrioridadeAcao.Urgente);
        Adicionar("Revisar etapas do funil com maior queda", "Recuperar conversão e destravar pipeline.", 53_200m, PrioridadeAcao.Alta);
        Adicionar("Ofertar expansão para clientes de alto potencial", "Potencial de R$ 132.000 em expansão.", 79_200m, PrioridadeAcao.Media);
        Adicionar("Eliminar gargalo comercial prioritário", "Destravar propostas paradas.", 46_000m, PrioridadeAcao.Alta);
        Adicionar("Reduzir tempo de análise de crédito", "Meta: abaixo de 1h por proposta.", 36_800m, PrioridadeAcao.Alta);
        Adicionar("Campanha cross-sell Produto A → B", "Aproveitar correlação de 70%.", 130_200m, PrioridadeAcao.Media);
        Adicionar("Reforçar equipe SDR na Região Norte", "Atacar origem da inadimplência.", 42_500m, PrioridadeAcao.Urgente);
    }

    private void SemearContasReceber()
    {
        var baseDate = DateTime.UtcNow.Date;
        void Adicionar(string cliente, decimal valor, int dias, string status)
            => _contasReceber.Add(new ContaReceber(IdEmpresaDemo, null, cliente, valor, baseDate.AddDays(dias), status));

        Adicionar("Rede Horizonte", 980_000m, 15, "AReceber");
        Adicionar("Distribuidora Norte", 720_000m, 28, "AReceber");
        Adicionar("Grupo Primavera", 542_000m, 45, "AReceber");
        Adicionar("Atacado Boa Vista", 420_000m, 10, "EmRisco");
        Adicionar("Mercado União", 222_000m, 5, "EmRisco");
        Adicionar("Comercial Andaza Ltda", 285_000m, -12, "Atrasado");
    }

    private SnapshotDashboard SemearSnapshotDashboard()
        => new(
            IdEmpresaDemo,
            faturamentoDia: 58_420m,
            faturamentoMes: 1_250_000m,
            lucroAtual: 1_247_850m,
            lucroEmRisco: 487_320m,
            valorOportunidade: 214_500m,
            saudeEmpresaPercentual: 78,
            gargalosCriticos: 4,
            valorInadimplencia: 312_000m,
            taxaConversao: 0.22m,
            metaMes: 1_500_000m,
            percentualMetaMes: 83.3m,
            previsaoResultado30Dias: 5_712_000m,
            metaResultado30Dias: 6_650_000m,
            percentualAbaixoMetaPrevisao: 14m,
            fluxoAReceber60Dias: 3_842_000m,
            fluxoEmRisco60Dias: 642_000m,
            fluxoAtrasado60Dias: 285_000m,
            percentualFluxoSaudavel: 78,
            payloadJson: "{}");

    private static T ComId<T>(T entidade, Guid id) where T : Domain.Common.Entity
    {
        typeof(Domain.Common.Entity)
            .GetProperty(nameof(Domain.Common.Entity.Id))!
            .SetValue(entidade, id);
        return entidade;
    }
}
