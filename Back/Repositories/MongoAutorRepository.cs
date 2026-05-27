using Back.Data;
using Back.Models;
using Back.Services;
using MongoDB.Driver;

namespace Back.Repositories
{
    /// <summary>
    /// Repository de autores usando MongoDB
    /// </summary>
    public class MongoAutorRepository : IAutorRepository
    {
        private readonly IMongoCollection<Autor> _collection;
        private readonly MongoDbContext _context;

        public MongoAutorRepository(MongoDbContext context)
        {
            _context = context;
            _collection = context.GetCollection<Autor>("autores");
        }

        public async Task<Autor> CreateAsync(Autor autor)
        {
            autor.DataCriacao = DateTime.UtcNow;
            await _collection.InsertOneAsync(autor);
            return autor;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _collection.DeleteOneAsync(a => a.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Autor>> GetAllAsync()
        {
            var all = await _collection.Find(_ => true).ToListAsync();
            return all;
        }

        public async Task<Autor?> GetByIdAsync(string id)
        {
            var autor = await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
            return autor;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            var count = await _collection.CountDocumentsAsync(a => a.Id == id);
            return count > 0;
        }

        public async Task<Autor?> UpdateAsync(string id, Autor autor)
        {
            var update = Builders<Autor>.Update
                .Set(a => a.Nome, autor.Nome)
                .Set(a => a.DataNascimento, autor.DataNascimento)
                .Set(a => a.Nacionalidade, autor.Nacionalidade)
                .Set(a => a.Biografia, autor.Biografia)
                .Set(a => a.DataAtualizacao, DateTime.UtcNow);

            var filter = Builders<Autor>.Filter.Eq(a => a.Id, id);
            var options = new FindOneAndUpdateOptions<Autor, Autor> { ReturnDocument = ReturnDocument.After };
            var res = await _collection.FindOneAndUpdateAsync(filter, update, options);
            return res;
        }

        public async Task<bool> HasLivrosAsync(string id)
        {
            var livrosCollection = _context.GetCollection<Livro>("livros");
            var count = await livrosCollection.CountDocumentsAsync(l => l.AutorId == id);
            return count > 0;
        }
    }
}
