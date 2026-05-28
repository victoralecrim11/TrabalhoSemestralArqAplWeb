using Back.Models;
using FluentValidation;
using MongoDB.Bson;

namespace Back.Validators
{
    public class LivroValidator : AbstractValidator<Livro>
    {
        public LivroValidator()
        {
            RuleFor(livro => livro.Id)
                .Must(id => string.IsNullOrWhiteSpace(id) || ObjectId.TryParse(id, out _))
                .WithMessage("ID do livro deve ser válido.");

            RuleFor(livro => livro.Titulo)
                .NotEmpty()
                .WithMessage("Título do livro é obrigatório.")
                .MinimumLength(2)
                .WithMessage("Título do livro deve ter no mínimo 2 caracteres.")
                .MaximumLength(200)
                .WithMessage("Título do livro deve ter no máximo 200 caracteres.");

            RuleFor(livro => livro.AutorId)
                .NotEmpty()
                .WithMessage("ID do autor é obrigatório.")
                .Must(id => ObjectId.TryParse(id, out _))
                .WithMessage("ID do autor deve ser válido.");

            RuleFor(livro => livro.ISBN)
                .MaximumLength(20)
                .WithMessage("ISBN deve ter no máximo 20 caracteres.");

            RuleFor(livro => livro.AnoPublicacao)
                .InclusiveBetween(1, DateTime.Today.Year + 1)
                .When(livro => livro.AnoPublicacao.HasValue)
                .WithMessage("Ano de publicação deve ser válido.");

            RuleFor(livro => livro.Editora)
                .MaximumLength(100)
                .WithMessage("Editora deve ter no máximo 100 caracteres.");

            RuleFor(livro => livro.Sinopse)
                .MaximumLength(3000)
                .WithMessage("Sinopse deve ter no máximo 3000 caracteres.");

            RuleFor(livro => livro.Categoria)
                .MaximumLength(80)
                .WithMessage("Categoria deve ter no máximo 80 caracteres.");

            RuleFor(livro => livro.DataAtualizacao)
                .GreaterThanOrEqualTo(livro => livro.DataCriacao)
                .When(livro => livro.DataAtualizacao.HasValue)
                .WithMessage("Data de atualização não pode ser anterior à data de criação.");
        }
    }
}
