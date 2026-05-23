using Back.Models;

namespace Back.Repositories
{
    /// <summary>
    /// Interface para operações CRUD de Autores
    /// </summary>
    public interface IAutorRepository
    {
        /// <summary>
        /// Obtém todos os autores
        /// </summary>
        Task<IEnumerable<Autor>> GetAllAsync();

        /// <summary>
        /// Obtém um autor por ID
        /// </summary>
        Task<Autor?> GetByIdAsync(int id);

        /// <summary>
        /// Cria um novo autor
        /// </summary>
        Task<Autor> CreateAsync(Autor autor);

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        Task<Autor?> UpdateAsync(int id, Autor autor);

        /// <summary>
        /// Deleta um autor
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica se um autor existe
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Verifica se um autor tem livros
        /// </summary>
        Task<bool> HasLivrosAsync(int id);
    }
}
