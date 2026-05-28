using Back.Models;
using FluentValidation;
using MongoDB.Bson;

namespace Back.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(usuario => usuario.Id)
                .Must(id => string.IsNullOrWhiteSpace(id) || ObjectId.TryParse(id, out _))
                .WithMessage("ID do usuário deve ser válido.");

            RuleFor(usuario => usuario.Email)
                .NotEmpty()
                .WithMessage("Email é obrigatório.")
                .EmailAddress()
                .WithMessage("Email deve ser válido.")
                .MaximumLength(150)
                .WithMessage("Email deve ter no máximo 150 caracteres.");

            RuleFor(usuario => usuario.SenhaHash)
                .NotEmpty()
                .WithMessage("Senha criptografada é obrigatória.");

            RuleFor(usuario => usuario.Nome)
                .NotEmpty()
                .WithMessage("Nome é obrigatório.")
                .MinimumLength(3)
                .WithMessage("Nome deve ter no mínimo 3 caracteres.")
                .MaximumLength(100)
                .WithMessage("Nome deve ter no máximo 100 caracteres.");

            RuleFor(usuario => usuario.Perfil)
                .NotEmpty()
                .WithMessage("Perfil é obrigatório.")
                .Must(perfil => perfil == "admin" || perfil == "usuario")
                .WithMessage("Perfil deve ser 'admin' ou 'usuario'.");

            RuleFor(usuario => usuario.DataAtualizacao)
                .GreaterThanOrEqualTo(usuario => usuario.DataCriacao)
                .When(usuario => usuario.DataAtualizacao.HasValue)
                .WithMessage("Data de atualização não pode ser anterior à data de criação.");
        }
    }
}
