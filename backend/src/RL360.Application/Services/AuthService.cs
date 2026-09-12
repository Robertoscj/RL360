using AutoMapper;
using RL360.Application.Abstractions;
using RL360.Domain.Entities;
using RL360.Domain.Enums;
using RL360.Shared.Dtos;
using RL360.Shared.Results;

namespace RL360.Application.Services;

public interface IAutenticacaoServico
{
    Task<Resultado<RespostaAutenticacao>> EntrarAsync(RequisicaoLogin requisicao, CancellationToken ct = default);
    Task<Resultado<RespostaAutenticacao>> RegistrarAsync(RequisicaoRegistro requisicao, CancellationToken ct = default);
    Task<Resultado<RespostaAutenticacao>> RenovarTokenAsync(RequisicaoRefreshToken requisicao, CancellationToken ct = default);
    Task<Resultado<UsuarioDto>> ObterAtualAsync(Guid idUsuario, CancellationToken ct = default);
}

public sealed class AutenticacaoServico(
    IUsuarioRepositorio usuarios,
    IEmpresaRepositorio empresas,
    IHashSenhaServico hashSenha,
    IJwtTokenServico tokenServico,
    IRefreshTokenServico refreshTokenServico,
    IMapper mapper) : IAutenticacaoServico
{
    public async Task<Resultado<RespostaAutenticacao>> EntrarAsync(RequisicaoLogin requisicao, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorEmailAsync(requisicao.Email, ct);
        if (usuario is null || !usuario.Ativo)
            return Resultado<RespostaAutenticacao>.Falha("Credenciais inválidas.");

        if (!hashSenha.Verificar(requisicao.Senha, usuario.HashSenha))
            return Resultado<RespostaAutenticacao>.Falha("Credenciais inválidas.");

        return Resultado<RespostaAutenticacao>.Ok(await MontarRespostaAsync(usuario, ct));
    }

    public async Task<Resultado<RespostaAutenticacao>> RegistrarAsync(RequisicaoRegistro requisicao, CancellationToken ct = default)
    {
        if (await usuarios.ExisteEmailAsync(requisicao.Email, ct))
            return Resultado<RespostaAutenticacao>.Falha("E-mail já cadastrado.");

        if (await empresas.ExisteDocumentoAsync(requisicao.DocumentoEmpresa, ct))
            return Resultado<RespostaAutenticacao>.Falha("Empresa já cadastrada com este documento.");

        var empresa = await empresas.CriarAsync(
            new Empresa(requisicao.NomeEmpresa, requisicao.DocumentoEmpresa.Trim(), "Geral", "starter"), ct);

        var usuario = await usuarios.CriarAsync(
            new Usuario(empresa.Id, requisicao.Nome, requisicao.Email,
                hashSenha.GerarHash(requisicao.Senha), PerfilUsuario.Proprietario), ct);

        return Resultado<RespostaAutenticacao>.Ok(await MontarRespostaAsync(usuario, ct));
    }

    public async Task<Resultado<RespostaAutenticacao>> RenovarTokenAsync(
        RequisicaoRefreshToken requisicao, CancellationToken ct = default)
    {
        var validacao = await refreshTokenServico.ValidarAsync(requisicao.RefreshToken, ct);
        if (validacao is null)
            return Resultado<RespostaAutenticacao>.Falha("Refresh token inválido ou expirado.");

        var (idUsuario, _) = validacao.Value;
        var usuario = await usuarios.ObterPorIdAsync(idUsuario, ct);
        if (usuario is null || !usuario.Ativo)
            return Resultado<RespostaAutenticacao>.Falha("Usuário não encontrado.");

        await refreshTokenServico.RevogarAsync(requisicao.RefreshToken, ct);
        return Resultado<RespostaAutenticacao>.Ok(await MontarRespostaAsync(usuario, ct));
    }

    public async Task<Resultado<UsuarioDto>> ObterAtualAsync(Guid idUsuario, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(idUsuario, ct);
        if (usuario is null)
            return Resultado<UsuarioDto>.Falha("Usuário não encontrado.");

        var dto = mapper.Map<UsuarioDto>(usuario);
        var empresa = await empresas.ObterPorIdAsync(usuario.IdEmpresa, ct);
        dto.NomeEmpresa = empresa?.Nome ?? string.Empty;
        return Resultado<UsuarioDto>.Ok(dto);
    }

    private async Task<RespostaAutenticacao> MontarRespostaAsync(Usuario usuario, CancellationToken ct)
    {
        var dto = mapper.Map<UsuarioDto>(usuario);
        var empresa = await empresas.ObterPorIdAsync(usuario.IdEmpresa, ct);
        dto.NomeEmpresa = empresa?.Nome ?? string.Empty;

        var (token, expira) = tokenServico.Gerar(dto);
        var (refresh, refreshExpira) = await refreshTokenServico.GerarAsync(usuario.Id, usuario.IdEmpresa, ct);

        return new RespostaAutenticacao
        {
            Token = token,
            ExpiraEmUtc = expira,
            RefreshToken = refresh,
            RefreshExpiraEmUtc = refreshExpira,
            Usuario = dto
        };
    }
}
