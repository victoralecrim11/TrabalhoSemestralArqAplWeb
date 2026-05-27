using Back.Data;
using Back.Models;

namespace Back.Repositories
{
    public class AutorRepository : IAutorRepository
    {
        private readonly DataContext _context;

        public AutorRepository(DataContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Autor>> GetAllAsync()
        {
            var autores = _context.Autores.AsEnumerable();
            return Task.FromResult(autores);
        }

        public Task<Autor?> GetByIdAsync(string id)
        {
            var autor = _context.Autores.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(autor);
        }

        public Task<Autor> CreateAsync(Autor autor)
        {
            autor.DataCriacao = DateTime.UtcNow;
            _context.Autores.Add(autor);
            return Task.FromResult(autor);
        }

        public Task<Autor?> UpdateAsync(string id, Autor autor)
        {
            var autorExistente = _context.Autores.FirstOrDefault(a => a.Id == id);

            if (autorExistente == null)
                return Task.FromResult((Autor?)null);

            autorExistente.Nome = autor.Nome;
            autorExistente.DataNascimento = autor.DataNascimento;
            autorExistente.Nacionalidade = autor.Nacionalidade;
            autorExistente.Biografia = autor.Biografia;
            autorExistente.DataAtualizacao = DateTime.UtcNow;

            return Task.FromResult((Autor?)autorExistente);
        }

        public Task<bool> DeleteAsync(string id)
        {
            var autor = _context.Autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
                return Task.FromResult(false);

            _context.Autores.Remove(autor);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string id)
        {
            var existe = _context.Autores.Any(a => a.Id == id);
            return Task.FromResult(existe);
        }

        public Task<bool> HasLivrosAsync(string id)
        {
            var temLivros = _context.Livros.Any(l => l.AutorId == id);
            return Task.FromResult(temLivros);
        }
    }
}
