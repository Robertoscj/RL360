using FluentValidation;
using RL360.Shared.Dtos;

namespace RL360.Application.Validation;

public sealed class ValidadorRequisicaoIa : AbstractValidator<RequisicaoIaPergunta>
{
    public ValidadorRequisicaoIa()
    {
        RuleFor(x => x.Pergunta)
            .NotEmpty().WithMessage("Digite uma pergunta.")
            .MinimumLength(3).WithMessage("Pergunta muito curta.")
            .MaximumLength(500).WithMessage("Pergunta limitada a 500 caracteres.");
    }
}
