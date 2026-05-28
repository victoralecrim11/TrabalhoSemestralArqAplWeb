using Back.Dtos.Autores;
using FluentValidation;

namespace Back.Validators
{
    public class AtualizarAutorDtoValidator : AbstractValidator<AtualizarAutorDto>
    {
        public AtualizarAutorDtoValidator()
        {
            RuleFor(autor => autor.Nome)
                .MinimumLength(3)
                .WithMessage("Nome do autor deve ter no mínimo 3 caracteres.")
                .MaximumLength(100)
                .WithMessage("Nome do autor deve ter no máximo 100 caracteres.")
                .When(autor => !string.IsNullOrWhiteSpace(autor.Nome));

            RuleFor(autor => autor.DataNascimento)
                .LessThanOrEqualTo(DateTime.Today)
                .When(autor => autor.DataNascimento.HasValue)
                .WithMessage("Data de nascimento não pode ser uma data futura.");

            RuleFor(autor => autor.Nacionalidade)
                .MaximumLength(60)
                .WithMessage("Nacionalidade deve ter no máximo 60 caracteres.");

            RuleFor(autor => autor.Biografia)
                .MaximumLength(2000)
                .WithMessage("Biografia deve ter no máximo 2000 caracteres.");
        }
    }
}
