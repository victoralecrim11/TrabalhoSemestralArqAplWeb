using Back.Data;
using Back.Models;
using Back.Services;
using MongoDB.Driver;

namespace Back.Repositories
{
    /// <summary>
    /// Repository de usuários usando MongoDB
    /// </summary>
    public class MongoUsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _collection;

        public MongoUsuarioRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Usuario>("usuarios");
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            usuario.DataCriacao = DateTime.UtcNow;
            await _collection.InsertOneAsync(usuario);
            return usuario;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _collection.DeleteOneAsync(u => u.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var all = await _collection.Find(_ => true).ToListAsync();
            return all;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            var u = await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();
            return u;
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            var u = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return u;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            var count = await _collection.CountDocumentsAsync(x => x.Id == id);
            return count > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var count = await _collection.CountDocumentsAsync(x => x.Email == email);
            return count > 0;
        }

        public async Task<Usuario?> UpdateAsync(string id, Usuario usuario)
        {
            var update = Builders<Usuario>.Update
                .Set(u => u.Email, usuario.Email)
                .Set(u => u.Nome, usuario.Nome)
                .Set(u => u.Perfil, usuario.Perfil)
                .Set(u => u.SenhaHash, usuario.SenhaHash)
                .Set(u => u.Ativo, usuario.Ativo)
                .Set(u => u.DataAtualizacao, DateTime.UtcNow);

            var filter = Builders<Usuario>.Filter.Eq(u => u.Id, id);
            var options = new FindOneAndUpdateOptions<Usuario, Usuario> { ReturnDocument = ReturnDocument.After };
            var res = await _collection.FindOneAndUpdateAsync(filter, update, options);
            return res;
        }
    }
}
