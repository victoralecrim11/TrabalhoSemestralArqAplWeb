using Back.Data;
using Back.Models;

namespace Back.Repositories
{
    /// <summary>
    /// Implementação do repository para Autores usando DataContext em memória
    /// </summary>
    public class AutorRepository : IAutorRepository
    {
        private readonly DataContext _context;

        public AutorRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os autores
        /// </summary>
        public Task<IEnumerable<Autor>> GetAllAsync()
        {
            var autores = _context.Autores.AsEnumerable();
            return Task.FromResult(autores);
        }

        /// <summary>
        /// Obtém um autor por ID
        /// </summary>
        public Task<Autor?> GetByIdAsync(int id)
        {
            var autor = _context.Autores.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(autor);
        }

        /// <summary>
        /// Cria um novo autor
        /// </summary>
        public Task<Autor> CreateAsync(Autor autor)
        {
            if (_context.Autores.Count == 0)
            {
                autor.Id = 1;
            }
            else
            {
                autor.Id = _context.Autores.Max(a => a.Id) + 1;
            }

            autor.DataCriacao = DateTime.UtcNow;
            _context.Autores.Add(autor);
            return Task.FromResult(autor);
        }

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        public Task<Autor?> UpdateAsync(int id, Autor autor)
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

        /// <summary>
        /// Deleta um autor
        /// </summary>
        public Task<bool> DeleteAsync(int id)
        {
            var autor = _context.Autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
                return Task.FromResult(false);

            _context.Autores.Remove(autor);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Verifica se um autor existe
        /// </summary>
        public Task<bool> ExistsAsync(int id)
        {
            var existe = _context.Autores.Any(a => a.Id == id);
            return Task.FromResult(existe);
        }

        /// <summary>
        /// Verifica se um autor tem livros
        /// </summary>
        public Task<bool> HasLivrosAsync(int id)
        {
            var temLivros = _context.Livros.Any(l => l.AutorId == id);
            return Task.FromResult(temLivros);
        }
    }
}
