using Back.Models;
using Back.Services;

namespace Back.Data
{
    /// <summary>
    /// DataContext em memória para gerenciar as collections de Livros, Autores e Usuários
    /// </summary>
    public class DataContext
    {
        public List<Livro> Livros { get; set; } = new();
        public List<Autor> Autores { get; set; } = new();
        public List<Usuario> Usuarios { get; set; } = new();

        /// <summary>
        /// Inicializa dados de seed para testes
        /// </summary>
        public void InitializeSeedData()
        {
            // Se já tem dados, não adiciona novamente
            if (Autores.Count > 0)
                return;

            // Criar alguns autores de seed
            var autor1 = new Autor
            {
                Id = 1,
                Nome = "Machado de Assis",
                DataNascimento = new DateTime(1839, 6, 21),
                Nacionalidade = "Brasileira",
                Biografia = "Machado de Assis foi um escritor brasileiro considerado por muitos críticos o mais influente nome da literatura brasileira.",
                DataCriacao = DateTime.UtcNow
            };

            var autor2 = new Autor
            {
                Id = 2,
                Nome = "Clarice Lispector",
                DataNascimento = new DateTime(1920, 12, 10),
                Nacionalidade = "Brasileira",
                Biografia = "Clarice Lispector foi uma escritora e jornalista nascida na Ucrânia e naturalizada brasileira.",
                DataCriacao = DateTime.UtcNow
            };

            var autor3 = new Autor
            {
                Id = 3,
                Nome = "Jorge Amado",
                DataNascimento = new DateTime(1912, 8, 10),
                Nacionalidade = "Brasileira",
                Biografia = "Jorge Leal Amado de Faria foi um dos mais famosos e traduzidos escritores brasileiros de todos os tempos.",
                DataCriacao = DateTime.UtcNow
            };

            Autores.AddRange(new[] { autor1, autor2, autor3 });

            // Criar alguns livros de seed
            var livro1 = new Livro
            {
                Id = 1,
                Titulo = "Dom Casmurro",
                AutorId = 1,
                ISBN = "978-8535928784",
                AnoPublicacao = 1899,
                Editora = "Companhia das Letras",
                Sinopse = "Dom Casmurro é um romance de Machado de Assis que narra a história de Bento Santiago e sua obsessão pela possível traição de Capitu.",
                Categoria = "Romance",
                DataCriacao = DateTime.UtcNow
            };

            var livro2 = new Livro
            {
                Id = 2,
                Titulo = "Memórias Póstumas de Brás Cubas",
                AutorId = 1,
                ISBN = "978-8535914842",
                AnoPublicacao = 1881,
                Editora = "Companhia das Letras",
                Sinopse = "Memórias Póstumas de Brás Cubas é um romance em primeira pessoa que narra as memórias de um homem já falecido.",
                Categoria = "Romance",
                DataCriacao = DateTime.UtcNow
            };

            var livro3 = new Livro
            {
                Id = 3,
                Titulo = "A Hora da Estrela",
                AutorId = 2,
                ISBN = "978-8525407288",
                AnoPublicacao = 1977,
                Editora = "Rocco",
                Sinopse = "A Hora da Estrela é o último livro escrito por Clarice Lispector, narrando a história de Macabéa.",
                Categoria = "Novela",
                DataCriacao = DateTime.UtcNow
            };

            var livro4 = new Livro
            {
                Id = 4,
                Titulo = "Capitães da Areia",
                AutorId = 3,
                ISBN = "978-8501925537",
                AnoPublicacao = 1937,
                Editora = "Companhia das Letras",
                Sinopse = "Capitães da Areia é um romance que narra as aventuras de um grupo de meninos de rua em Salvador.",
                Categoria = "Romance",
                DataCriacao = DateTime.UtcNow
            };

            Livros.AddRange(new[] { livro1, livro2, livro3, livro4 });

            // Criar um usuário admin de seed
            var usuarioAdmin = new Usuario
            {
                Id = 1,
                Email = "admin@biblioteca.com",
                SenhaHash = PasswordHasher.Hash("admin123"), 
                Nome = "Administrador",
                Perfil = "admin",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            Usuarios.Add(usuarioAdmin);
        }
    }
}