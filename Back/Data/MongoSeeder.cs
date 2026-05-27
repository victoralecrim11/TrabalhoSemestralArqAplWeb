using Back.Models;
using Back.Services;
using MongoDB.Driver;

namespace Back.Data
{
        public class MongoSeeder
        {
        private readonly MongoDbContext _context;

        public MongoSeeder(MongoDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var autores = await SeedAutoresAsync();
            await SeedLivrosAsync(autores);
            await SeedUsuariosAsync();
        }

        private async Task<Dictionary<string, string>> SeedAutoresAsync()
        {
            var collection = _context.GetCollection<Autor>("autores");
            var autores = new[]
            {
                new Autor
                {
                    Nome = "Machado de Assis",
                    DataNascimento = new DateTime(1839, 6, 21),
                    Nacionalidade = "Brasileira",
                    Biografia = "Machado de Assis foi um escritor brasileiro considerado por muitos críticos o mais influente nome da literatura brasileira.",
                    DataCriacao = DateTime.UtcNow
                },
                new Autor
                {
                    Nome = "Clarice Lispector",
                    DataNascimento = new DateTime(1920, 12, 10),
                    Nacionalidade = "Brasileira",
                    Biografia = "Clarice Lispector foi uma escritora e jornalista nascida na Ucrânia e naturalizada brasileira.",
                    DataCriacao = DateTime.UtcNow
                },
                new Autor
                {
                    Nome = "Jorge Amado",
                    DataNascimento = new DateTime(1912, 8, 10),
                    Nacionalidade = "Brasileira",
                    Biografia = "Jorge Leal Amado de Faria foi um dos mais famosos e traduzidos escritores brasileiros de todos os tempos.",
                    DataCriacao = DateTime.UtcNow
                }
            };

            var resultado = new Dictionary<string, string>();

            foreach (var autor in autores)
            {
                var existente = await collection.Find(a => a.Nome == autor.Nome).FirstOrDefaultAsync();
                if (existente == null)
                {
                    await collection.InsertOneAsync(autor);
                    existente = autor;
                }

                if (!string.IsNullOrWhiteSpace(existente.Id))
                    resultado[autor.Nome] = existente.Id;
            }

            return resultado;
        }

        private async Task SeedLivrosAsync(Dictionary<string, string> autores)
        {
            var collection = _context.GetCollection<Livro>("livros");
            var livros = new[]
            {
                new Livro
                {
                    Titulo = "Dom Casmurro",
                    AutorId = autores["Machado de Assis"],
                    ISBN = "978-8535928784",
                    AnoPublicacao = 1899,
                    Editora = "Companhia das Letras",
                    Sinopse = "Dom Casmurro é um romance de Machado de Assis que narra a história de Bento Santiago e sua obsessão pela possível traição de Capitu.",
                    Categoria = "Romance",
                    DataCriacao = DateTime.UtcNow
                },
                new Livro
                {
                    Titulo = "Memórias Póstumas de Brás Cubas",
                    AutorId = autores["Machado de Assis"],
                    ISBN = "978-8535914842",
                    AnoPublicacao = 1881,
                    Editora = "Companhia das Letras",
                    Sinopse = "Memórias Póstumas de Brás Cubas é um romance em primeira pessoa que narra as memórias de um homem já falecido.",
                    Categoria = "Romance",
                    DataCriacao = DateTime.UtcNow
                },
                new Livro
                {
                    Titulo = "A Hora da Estrela",
                    AutorId = autores["Clarice Lispector"],
                    ISBN = "978-8525407288",
                    AnoPublicacao = 1977,
                    Editora = "Rocco",
                    Sinopse = "A Hora da Estrela é o último livro escrito por Clarice Lispector, narrando a história de Macabéa.",
                    Categoria = "Novela",
                    DataCriacao = DateTime.UtcNow
                },
                new Livro
                {
                    Titulo = "Capitães da Areia",
                    AutorId = autores["Jorge Amado"],
                    ISBN = "978-8501925537",
                    AnoPublicacao = 1937,
                    Editora = "Companhia das Letras",
                    Sinopse = "Capitães da Areia é um romance que narra as aventuras de um grupo de meninos de rua em Salvador.",
                    Categoria = "Romance",
                    DataCriacao = DateTime.UtcNow
                }
            };

            foreach (var livro in livros)
            {
                var existe = await collection.Find(l => l.Titulo == livro.Titulo).AnyAsync();
                if (!existe)
                    await collection.InsertOneAsync(livro);
            }
        }

        private async Task SeedUsuariosAsync()
        {
            var collection = _context.GetCollection<Usuario>("usuarios");
            var emailAdmin = "admin@biblioteca.com";
            var usuarioAdmin = new Usuario
            {
                Email = emailAdmin,
                SenhaHash = PasswordHasher.Hash("admin123"),
                Nome = "Administrador",
                Perfil = "admin",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            var existente = await collection.Find(u => u.Email == emailAdmin).FirstOrDefaultAsync();
            if (existente == null)
            {
                await collection.InsertOneAsync(usuarioAdmin);
                return;
            }

            var update = Builders<Usuario>.Update
                .Set(u => u.SenhaHash, usuarioAdmin.SenhaHash)
                .Set(u => u.Nome, usuarioAdmin.Nome)
                .Set(u => u.Perfil, usuarioAdmin.Perfil)
                .Set(u => u.Ativo, true)
                .Set(u => u.DataAtualizacao, DateTime.UtcNow);

            await collection.UpdateOneAsync(u => u.Email == emailAdmin, update);
        }
    }
}
