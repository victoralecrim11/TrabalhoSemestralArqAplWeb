using Back.Models;

namespace Back.Repositories
{
    /// <summary>
    /// Interface para operações CRUD de Livros
    /// </summary>
    public interface ILivroRepository
    {
        /// <summary>
        /// Obtém todos os livros
        /// </summary>
        Task<IEnumerable<Livro>> GetAllAsync();

        /// <summary>
        /// Obtém um livro por ID
        /// </summary>
        Task<Livro?> GetByIdAsync(int id);

        /// <summary>
        /// Obtém livros por ID do autor
        /// </summary>
        Task<IEnumerable<Livro>> GetByAutorIdAsync(int autorId);

        /// <summary>
        /// Cria um novo livro
        /// </summary>
        Task<Livro> CreateAsync(Livro livro);

        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        Task<Livro?> UpdateAsync(int id, Livro livro);

        /// <summary>
        /// Deleta um livro
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica se um livro existe
        /// </summary>
        Task<bool> ExistsAsync(int id);
    }
}
