using Back.Dtos.Autores;
using FluentValidation;

namespace Back.Validators
{
    public class CriarAutorDtoValidator : AbstractValidator<CriarAutorDto>
    {
        public CriarAutorDtoValidator()
        {
            RuleFor(autor => autor.Nome)
                .NotEmpty()
                .WithMessage("Nome do autor é obrigatório.")
                .MinimumLength(3)
                .WithMessage("Nome do autor deve ter no mínimo 3 caracteres.")
                .MaximumLength(100)
                .WithMessage("Nome do autor deve ter no máximo 100 caracteres.");

            RuleFor(autor => autor.DataNascimento)
                .LessThanOrEqualTo(DateTime.Today)
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
