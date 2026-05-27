using Back.Dtos.Livros;
using Back.Models;

namespace Back.Services
{
    /// <summary>
    /// Interface que define os contratos para operações de negócio relacionadas a Livros.
    /// Separa a lógica de negócio da camada de acesso a dados (Repository Pattern).
    /// </summary>
    public interface ILivroService
    {
        /// <summary>
        /// Obtém todos os livros com validações de negócio
        /// </summary>
        /// <returns>Coleção de livros disponíveis</returns>
        Task<IEnumerable<Livro>> GetAllAsync();

        Task<Livro?> GetByIdAsync(string id);

        Task<IEnumerable<Livro>> GetByAutorIdAsync(string autorId);

        Task<Livro> CreateAsync(CriarLivroDto dto);

        Task<Livro?> UpdateAsync(string id, AtualizarLivroDto dto);

        Task<bool> DeleteAsync(string id);
    }
}
