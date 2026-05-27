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

        Task<Usuario?> GetByIdAsync(string id);

        Task<Usuario?> GetByEmailAsync(string email);

        Task<Usuario> CreateAsync(Usuario usuario);

        Task<Usuario?> UpdateAsync(string id, Usuario usuario);

        Task<bool> DeleteAsync(string id);

        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Verifica se um email já está registrado
        /// </summary>
        Task<bool> EmailExistsAsync(string email);
    }
}
