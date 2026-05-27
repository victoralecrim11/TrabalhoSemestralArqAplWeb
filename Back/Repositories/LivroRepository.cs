using Back.Data;
using Back.Models;

namespace Back.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly DataContext _context;

        public LivroRepository(DataContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Livro>> GetAllAsync()
        {
            var livros = _context.Livros.AsEnumerable();
            return Task.FromResult(livros);
        }

        public Task<Livro?> GetByIdAsync(string id)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(livro);
        }

        public Task<IEnumerable<Livro>> GetByAutorIdAsync(string autorId)
        {
            var livros = _context.Livros.Where(l => l.AutorId == autorId).AsEnumerable();
            return Task.FromResult(livros);
        }

        public Task<Livro> CreateAsync(Livro livro)
        {
            livro.DataCriacao = DateTime.UtcNow;
            _context.Livros.Add(livro);
            return Task.FromResult(livro);
        }

        public Task<Livro?> UpdateAsync(string id, Livro livro)
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

        public Task<bool> DeleteAsync(string id)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);

            if (livro == null)
                return Task.FromResult(false);

            _context.Livros.Remove(livro);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string id)
        {
            var existe = _context.Livros.Any(l => l.Id == id);
            return Task.FromResult(existe);
        }
    }
}
