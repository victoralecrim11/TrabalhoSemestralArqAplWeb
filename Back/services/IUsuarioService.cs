using Back.Dtos.Auth;
using Back.Models;

namespace Back.Services
{
    /// <summary>
    /// Interface que define os contratos para operações de negócio relacionadas a Usuários.
    /// Implementa validações para autenticação e gerenciamento de perfis.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Registra um novo usuário comum.
        /// </summary>
        Task<Usuario> RegisterAsync(RegistroDto dto);

        /// <summary>
        /// Registra um novo administrador.
        /// </summary>
        Task<Usuario> RegisterAdminAsync(RegistroDto dto);

        /// <summary>
        /// Autentica um usuário por email e senha, independentemente do seu perfil.
        /// </summary>
        Task<Usuario?> AuthenticateAsync(LoginDto dto);

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        Task<IEnumerable<Usuario>> GetAllAsync();

        /// <summary>
        /// Obtém um usuário por ID.
        /// </summary>
        Task<Usuario?> GetByIdAsync(int id);

        /// <summary>
        /// Obtém um usuário por email para validação de login.
        /// </summary>
        Task<Usuario?> GetByEmailAsync(string email);

        /// <summary>
        /// Verifica se um email já está registrado no sistema.
        /// </summary>
        Task<bool> EmailExistsAsync(string email);
    }
}