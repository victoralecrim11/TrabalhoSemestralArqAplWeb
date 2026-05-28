using Back.Models;
using FluentValidation;
using MongoDB.Bson;

namespace Back.Validators
{
    public class AutorValidator : AbstractValidator<Autor>
    {
        public AutorValidator()
        {
            RuleFor(autor => autor.Id)
                .Must(id => string.IsNullOrWhiteSpace(id) || ObjectId.TryParse(id, out _))
                .WithMessage("ID do autor deve ser válido.");

            RuleFor(autor => autor.Nome)
                .NotEmpty()
                .WithMessage("Nome do autor é obrigatório.")
                .MinimumLength(3)
                .WithMessage("Nome do autor deve ter no mínimo 3 caracteres.")
                .MaximumLength(100)
                .WithMessage("Nome do autor deve ter no máximo 100 caracteres.");

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

            RuleFor(autor => autor.DataAtualizacao)
                .GreaterThanOrEqualTo(autor => autor.DataCriacao)
                .When(autor => autor.DataAtualizacao.HasValue)
                .WithMessage("Data de atualização não pode ser anterior à data de criação.");
        }
    }
}
