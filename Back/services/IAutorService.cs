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

        Task<Autor?> GetByIdAsync(string id);

        Task<Autor> CreateAsync(CriarAutorDto dto);

        Task<Autor?> UpdateAsync(string id, AtualizarAutorDto dto);

        Task<bool> DeleteAsync(string id);
    }
}
