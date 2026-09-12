using FluentValidation;
using RL360.Shared.Dtos;

namespace RL360.Application.Validation;

public sealed class ValidadorRequisicaoRegistro : AbstractValidator<RequisicaoRegistro>
{
    public ValidadorRequisicaoRegistro()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório.").MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres.");
        RuleFor(x => x.NomeEmpresa).NotEmpty().WithMessage("Nome da empresa é obrigatório.");
        RuleFor(x => x.DocumentoEmpresa).NotEmpty().WithMessage("CNPJ/documento é obrigatório.");
    }
}

public sealed class ValidadorRequisicaoFaturamento : AbstractValidator<RequisicaoFaturamento>
{
    public ValidadorRequisicaoFaturamento()
    {
        RuleFor(x => x.FaturamentoMes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MetaMensal).GreaterThan(0);
    }
}

public sealed class ValidadorRequisicaoVenda : AbstractValidator<RequisicaoVenda>
{
    public ValidadorRequisicaoVenda()
    {
        RuleFor(x => x.Canal).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}

public sealed class ValidadorRequisicaoEtapaFunil : AbstractValidator<RequisicaoEtapaFunil>
{
    public ValidadorRequisicaoEtapaFunil()
    {
        RuleFor(x => x.Nome).NotEmpty();
        RuleFor(x => x.Ordem).GreaterThan(0);
    }
}

public sealed class ValidadorRequisicaoInadimplencia : AbstractValidator<RequisicaoInadimplencia>
{
    public ValidadorRequisicaoInadimplencia()
    {
        RuleFor(x => x.NomeCliente).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.ProbabilidadeRecuperacao).InclusiveBetween(0, 1);
    }
}

public sealed class ValidadorRequisicaoGargalo : AbstractValidator<RequisicaoGargalo>
{
    public ValidadorRequisicaoGargalo()
    {
        RuleFor(x => x.Titulo).NotEmpty();
        RuleFor(x => x.Area).NotEmpty();
    }
}
