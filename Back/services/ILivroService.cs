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

        /// <summary>
        /// Obtém um livro específico por ID com tratamento de erros
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Livro encontrado ou null se não existir</returns>
        Task<Livro?> GetByIdAsync(int id);

        /// <summary>
        /// Obtém todos os livros de um autor específico
        /// </summary>
        /// <param name="autorId">ID do autor</param>
        /// <returns>Coleção de livros do autor</returns>
        Task<IEnumerable<Livro>> GetByAutorIdAsync(int autorId);

        /// <summary>
        /// Cria um novo livro com validações de negócio
        /// </summary>
        /// <param name="dto">DTO com dados do livro</param>
        /// <returns>Livro criado com ID gerado</returns>
        Task<Livro> CreateAsync(CriarLivroDto dto);

        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        /// <param name="id">ID do livro a atualizar</param>
        /// <param name="dto">DTO com novos dados</param>
        /// <returns>Livro atualizado</returns>
        Task<Livro?> UpdateAsync(int id, AtualizarLivroDto dto);

        /// <summary>
        /// Deleta um livro
        /// </summary>
        /// <param name="id">ID do livro a deletar</param>
        /// <returns>True se deletado com sucesso, false caso contrário</returns>
        Task<bool> DeleteAsync(int id);
    }
}
