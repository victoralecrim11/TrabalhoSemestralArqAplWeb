using Back.Dtos.Autores;
using Back.Models;

namespace Back.Services
{
    /// <summary>
    /// Interface que define os contratos para operações de negócio relacionadas a Autores.
    /// Implementa validações, regras de negócio e tratamento de exceções.
    /// </summary>
    public interface IAutorService
    {
        /// <summary>
        /// Obtém todos os autores cadastrados
        /// </summary>
        /// <returns>Coleção de todos os autores</returns>
        Task<IEnumerable<Autor>> GetAllAsync();

        /// <summary>
        /// Obtém um autor específico por ID
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Autor encontrado ou null se não existir</returns>
        Task<Autor?> GetByIdAsync(int id);

        /// <summary>
        /// Cria um novo autor com validações de dados obrigatórios
        /// </summary>
        /// <param name="dto">DTO com dados do autor</param>
        /// <returns>Autor criado com ID gerado</returns>
        /// <exception cref="ArgumentException">Lançada se dados obrigatórios forem inválidos</exception>
        Task<Autor> CreateAsync(CriarAutorDto dto);

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        /// <param name="id">ID do autor a atualizar</param>
        /// <param name="dto">DTO com novos dados</param>
        /// <returns>Autor atualizado ou null se não encontrado</returns>
        /// <exception cref="InvalidOperationException">Lançada se autor não existir</exception>
        Task<Autor?> UpdateAsync(int id, AtualizarAutorDto dto);

        /// <summary>
        /// Deleta um autor, com verificação de livros associados
        /// </summary>
        /// <param name="id">ID do autor a deletar</param>
        /// <returns>True se deletado com sucesso, false caso contrário</returns>
        /// <exception cref="InvalidOperationException">Lançada se autor possuir livros</exception>
        Task<bool> DeleteAsync(int id);
    }
}
