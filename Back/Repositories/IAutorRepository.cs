using Back.Models;

namespace Back.Repositories
{
    public interface IAutorRepository
    {
        Task<IEnumerable<Autor>> GetAllAsync();

        Task<Autor?> GetByIdAsync(string id);

        Task<Autor> CreateAsync(Autor autor);

        Task<Autor?> UpdateAsync(string id, Autor autor);

        Task<bool> DeleteAsync(string id);

        Task<bool> ExistsAsync(string id);

        Task<bool> HasLivrosAsync(string id);
    }
}
