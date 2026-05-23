using Back.Models;

namespace Back.Repositories
{
    /// <summary>
    /// Interface para operações CRUD de Usuários
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        Task<IEnumerable<Usuario>> GetAllAsync();

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        Task<Usuario?> GetByIdAsync(int id);

        /// <summary>
        /// Obtém um usuário por email
        /// </summary>
        Task<Usuario?> GetByEmailAsync(string email);

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        Task<Usuario> CreateAsync(Usuario usuario);

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        Task<Usuario?> UpdateAsync(int id, Usuario usuario);

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica se um usuário existe
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Verifica se um email já está registrado
        /// </summary>
        Task<bool> EmailExistsAsync(string email);
    }
}
