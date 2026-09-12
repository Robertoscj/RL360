using FluentValidation;
using RL360.Shared.Dtos;

namespace RL360.Application.Validation;

public sealed class ValidadorRequisicaoLogin : AbstractValidator<RequisicaoLogin>
{
    public ValidadorRequisicaoLogin()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(4).WithMessage("Senha muito curta.");
    }
}
