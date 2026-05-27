using Back.Dtos.Auth;
using Back.Models;
using Back.Repositories;
using System.Net.Mail;

namespace Back.Services
{
    /// <summary>
    /// Serviço de Usuários que encapsula a lógica de negócio.
    /// Responsável por validações de email, perfis e tratamento de erros.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> RegisterAsync(RegistroDto dto)
        {
            return await RegisterWithPerfilAsync(dto, "usuario");
        }

        public async Task<Usuario> RegisterAdminAsync(RegistroDto dto)
        {
            return await RegisterWithPerfilAsync(dto, "admin");
        }

        private async Task<Usuario> RegisterWithPerfilAsync(RegistroDto dto, string perfil)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de registro são obrigatórios");

            var email = NormalizeEmail(dto.Email);
            ValidateEmail(email);
            ValidateRequired(dto.Nome, "Nome é obrigatório", nameof(dto.Nome));
            ValidatePassword(dto.Senha);

            if (await _usuarioRepository.EmailExistsAsync(email))
                throw new InvalidOperationException("Email já cadastrado");

            var usuario = new Usuario
            {
                Email = email,
                Nome = dto.Nome.Trim(),
                Perfil = perfil,
                SenhaHash = PasswordHasher.Hash(dto.Senha),
                Ativo = true
            };

            return await _usuarioRepository.CreateAsync(usuario);
        }

        public async Task<Usuario?> AuthenticateAsync(LoginDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Credenciais são obrigatórias");

            var email = NormalizeEmail(dto.Email);
            ValidateEmail(email);
            ValidateRequired(dto.Senha, "Senha é obrigatória", nameof(dto.Senha));

            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null || !usuario.Ativo)
                return null;

            return PasswordHasher.Verify(dto.Senha, usuario.SenhaHash) ? usuario : null;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                return await _usuarioRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao obter usuários", ex);
            }
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("ID do usuário é obrigatório", nameof(id));

                return await _usuarioRepository.GetByIdAsync(id);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao obter usuário com ID {id}", ex);
            }
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            try
            {
                email = NormalizeEmail(email);
                ValidateEmail(email);
                return await _usuarioRepository.GetByEmailAsync(email);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao obter usuário com email {email}", ex);
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                email = NormalizeEmail(email);
                ValidateEmail(email);
                return await _usuarioRepository.EmailExistsAsync(email);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao verificar existência do email {email}", ex);
            }
        }

        private static string NormalizeEmail(string email)
        {
            return email?.Trim().ToLowerInvariant()
                ?? throw new ArgumentException("Email é obrigatório", nameof(email));
        }

        private static void ValidateEmail(string email)
        {
            ValidateRequired(email, "Email é obrigatório", nameof(email));

            try
            {
                _ = new MailAddress(email);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Email deve ser válido", nameof(email));
            }
        }

        private static void ValidatePassword(string password)
        {
            ValidateRequired(password, "Senha é obrigatória", nameof(password));
            if (password.Length < 6)
                throw new ArgumentException("Senha deve ter no mínimo 6 caracteres", nameof(password));
        }

        private static void ValidateRequired(string value, string message, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(message, paramName);
        }
    }
}