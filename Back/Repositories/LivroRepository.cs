using Back.Data;
using Back.Models;

namespace Back.Repositories
{
    /// <summary>
    /// Implementação do repository para Livros usando DataContext em memória
    /// </summary>
    public class LivroRepository : ILivroRepository
    {
        private readonly DataContext _context;

        public LivroRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os livros
        /// </summary>
        public Task<IEnumerable<Livro>> GetAllAsync()
        {
            var livros = _context.Livros.AsEnumerable();
            return Task.FromResult(livros);
        }

        /// <summary>
        /// Obtém um livro por ID
        /// </summary>
        public Task<Livro?> GetByIdAsync(int id)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(livro);
        }

        /// <summary>
        /// Obtém livros por ID do autor
        /// </summary>
        public Task<IEnumerable<Livro>> GetByAutorIdAsync(int autorId)
        {
            var livros = _context.Livros.Where(l => l.AutorId == autorId).AsEnumerable();
            return Task.FromResult(livros);
        }

        /// <summary>
        /// Cria um novo livro
        /// </summary>
        public Task<Livro> CreateAsync(Livro livro)
        {
            if (_context.Livros.Count == 0)
            {
                livro.Id = 1;
            }
            else
            {
                livro.Id = _context.Livros.Max(l => l.Id) + 1;
            }

            livro.DataCriacao = DateTime.UtcNow;
            _context.Livros.Add(livro);
            return Task.FromResult(livro);
        }

        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        public Task<Livro?> UpdateAsync(int id, Livro livro)
        {
            var livroExistente = _context.Livros.FirstOrDefault(l => l.Id == id);

            if (livroExistente == null)
                return Task.FromResult((Livro?)null);

            livroExistente.Titulo = livro.Titulo;
            livroExistente.AutorId = livro.AutorId;
            livroExistente.ISBN = livro.ISBN;
            livroExistente.AnoPublicacao = livro.AnoPublicacao;
            livroExistente.Editora = livro.Editora;
            livroExistente.Sinopse = livro.Sinopse;
            livroExistente.Categoria = livro.Categoria;
            livroExistente.DataAtualizacao = DateTime.UtcNow;

            return Task.FromResult((Livro?)livroExistente);
        }

        /// <summary>
        /// Deleta um livro
        /// </summary>
        public Task<bool> DeleteAsync(int id)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);

            if (livro == null)
                return Task.FromResult(false);

            _context.Livros.Remove(livro);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Verifica se um livro existe
        /// </summary>
        public Task<bool> ExistsAsync(int id)
        {
            var existe = _context.Livros.Any(l => l.Id == id);
            return Task.FromResult(existe);
        }
    }
}
