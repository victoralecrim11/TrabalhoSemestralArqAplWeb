using Back.Data;
using Back.Models;
using Back.Services;
using MongoDB.Driver;

namespace Back.Repositories
{
    /// <summary>
    /// Repository de livros usando MongoDB
    /// </summary>
    public class MongoLivroRepository : ILivroRepository
    {
        private readonly IMongoCollection<Livro> _collection;

        public MongoLivroRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Livro>("livros");
        }

        public async Task<Livro> CreateAsync(Livro livro)
        {
            livro.DataCriacao = DateTime.UtcNow;
            await _collection.InsertOneAsync(livro);
            return livro;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _collection.DeleteOneAsync(l => l.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Livro>> GetAllAsync()
        {
            var all = await _collection.Find(_ => true).ToListAsync();
            return all;
        }

        public async Task<Livro?> GetByIdAsync(string id)
        {
            var livro = await _collection.Find(l => l.Id == id).FirstOrDefaultAsync();
            return livro;
        }

        public async Task<IEnumerable<Livro>> GetByAutorIdAsync(string autorId)
        {
            var livros = await _collection.Find(l => l.AutorId == autorId).ToListAsync();
            return livros;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            var count = await _collection.CountDocumentsAsync(l => l.Id == id);
            return count > 0;
        }

        public async Task<Livro?> UpdateAsync(string id, Livro livro)
        {
            var update = Builders<Livro>.Update
                .Set(l => l.Titulo, livro.Titulo)
                .Set(l => l.AutorId, livro.AutorId)
                .Set(l => l.ISBN, livro.ISBN)
                .Set(l => l.AnoPublicacao, livro.AnoPublicacao)
                .Set(l => l.Editora, livro.Editora)
                .Set(l => l.Sinopse, livro.Sinopse)
                .Set(l => l.Categoria, livro.Categoria)
                .Set(l => l.DataAtualizacao, DateTime.UtcNow);

            var filter = Builders<Livro>.Filter.Eq(l => l.Id, id);
            var options = new FindOneAndUpdateOptions<Livro, Livro> { ReturnDocument = ReturnDocument.After };
            var res = await _collection.FindOneAndUpdateAsync(filter, update, options);
            return res;
        }
    }
}
