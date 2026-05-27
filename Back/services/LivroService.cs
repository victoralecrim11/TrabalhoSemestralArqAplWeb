using Back.Dtos.Livros;
using Back.Models;
using Back.Repositories;

namespace Back.Services
{
    /// <summary>
    /// Serviço de Livros que encapsula a lógica de negócio.
    /// Atua como intermediário entre Controller e Repository.
    /// Responsável por validações, regras de negócio e transformação de dados.
    /// </summary>
    public class LivroService : ILivroService
    {
        // Injeção de dependência do repositório de livros
        private readonly ILivroRepository _livroRepository;
        // Injeção de dependência do repositório de autores para validações
        private readonly IAutorRepository _autorRepository;

        /// <summary>
        /// Construtor com injeção de dependências
        /// </summary>
        /// <param name="livroRepository">Repositório para acesso a dados de livros</param>
        /// <param name="autorRepository">Repositório para validar existência de autores</param>
        public LivroService(ILivroRepository livroRepository, IAutorRepository autorRepository)
        {
            // Valida se o repositório de livros foi fornecido
            _livroRepository = livroRepository ?? throw new ArgumentNullException(nameof(livroRepository));
            // Valida se o repositório de autores foi fornecido
            _autorRepository = autorRepository ?? throw new ArgumentNullException(nameof(autorRepository));
        }

        /// <summary>
        /// Obtém todos os livros sem filtros
        /// </summary>
        public async Task<IEnumerable<Livro>> GetAllAsync()
        {
            try
            {
                // Chama o repositório para buscar todos os livros
                var livros = await _livroRepository.GetAllAsync();
                // Retorna a coleção de livros (pode estar vazia)
                return livros;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa para o controlador
                throw new InvalidOperationException("Erro ao obter livros", ex);
            }
        }

        public async Task<Livro?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("ID do livro não pode ser vazio", nameof(id));

                var livro = await _livroRepository.GetByIdAsync(id);
                return livro;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException($"Erro ao obter livro com ID {id}", ex);
            }
        }

        public async Task<IEnumerable<Livro>> GetByAutorIdAsync(string autorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(autorId))
                    throw new ArgumentException("ID do autor não pode ser vazio", nameof(autorId));

                var autorExiste = await _autorRepository.ExistsAsync(autorId);
                if (!autorExiste)
                    throw new InvalidOperationException($"Autor com ID {autorId} não encontrado");

                var livros = await _livroRepository.GetByAutorIdAsync(autorId);
                return livros;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação
                throw;
            }
            catch (InvalidOperationException)
            {
                // Re-lança exceções de negócio
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException($"Erro ao obter livros do autor {autorId}", ex);
            }
        }

        /// <summary>
        /// Cria um novo livro com validações de negócio
        /// </summary>
        public async Task<Livro> CreateAsync(CriarLivroDto dto)
        {
            try
            {
                // Valida se o DTO foi fornecido
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Dados do livro são obrigatórios");

                // Valida se o título do livro foi fornecido e não está vazio
                if (string.IsNullOrWhiteSpace(dto.Titulo))
                    throw new ArgumentException("Título do livro é obrigatório", nameof(dto.Titulo));

                if (string.IsNullOrWhiteSpace(dto.AutorId))
                    throw new ArgumentException("ID do autor deve ser válido", nameof(dto.AutorId));

                var autorExiste = await _autorRepository.ExistsAsync(dto.AutorId);
                if (!autorExiste)
                    throw new InvalidOperationException($"Autor com ID {dto.AutorId} não encontrado");

                // Cria uma nova instância de Livro com os dados do DTO
                var novoLivro = new Livro
                {
                    // Atribui o título fornecido
                    Titulo = dto.Titulo,
                    // Atribui o ID do autor (já validado)
                    AutorId = dto.AutorId,
                    // Atribui o ISBN (pode ser nulo)
                    ISBN = dto.ISBN,
                    // Atribui o ano de publicação (pode ser nulo)
                    AnoPublicacao = dto.AnoPublicacao,
                    // Atribui a editora (pode ser nula)
                    Editora = dto.Editora,
                    // Atribui a sinopse (pode ser nula)
                    Sinopse = dto.Sinopse,
                    // Atribui a categoria (pode ser nula)
                    Categoria = dto.Categoria
                };

                // Chama o repositório para persistir o novo livro
                var livroCriado = await _livroRepository.CreateAsync(novoLivro);
                // Retorna o livro criado com ID gerado
                return livroCriado;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação (inclui ArgumentNullException)
                throw;
            }
            catch (InvalidOperationException)
            {
                // Re-lança exceções de negócio
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException("Erro ao criar livro", ex);
            }
        }

        public async Task<Livro?> UpdateAsync(string id, AtualizarLivroDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("ID do livro não pode ser vazio", nameof(id));

                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Dados de atualização são obrigatórios");

                if (string.IsNullOrWhiteSpace(dto.Titulo))
                    throw new ArgumentException("Título do livro é obrigatório", nameof(dto.Titulo));

                var livroExistente = await _livroRepository.GetByIdAsync(id);

                if (livroExistente == null)
                    throw new InvalidOperationException($"Livro com ID {id} não encontrado");

                if (!string.IsNullOrWhiteSpace(dto.AutorId) && dto.AutorId != livroExistente.AutorId)
                {
                    var autorExiste = await _autorRepository.ExistsAsync(dto.AutorId);
                    if (!autorExiste)
                        throw new InvalidOperationException($"Autor com ID {dto.AutorId} não encontrado");
                }

                var livroAtualizado = new Livro
                {
                    Id = id,
                    Titulo = dto.Titulo ?? livroExistente.Titulo,
                    AutorId = dto.AutorId ?? livroExistente.AutorId,
                    ISBN = dto.ISBN ?? livroExistente.ISBN,
                    AnoPublicacao = dto.AnoPublicacao ?? livroExistente.AnoPublicacao,
                    Editora = dto.Editora ?? livroExistente.Editora,
                    Sinopse = dto.Sinopse ?? livroExistente.Sinopse,
                    Categoria = dto.Categoria ?? livroExistente.Categoria
                };

                var resultado = await _livroRepository.UpdateAsync(id, livroAtualizado);
                return resultado;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação (inclui ArgumentNullException)
                throw;
            }
            catch (InvalidOperationException)
            {
                // Re-lança exceções de negócio
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException($"Erro ao atualizar livro {id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("ID do livro não pode ser vazio", nameof(id));

                var livroExiste = await _livroRepository.ExistsAsync(id);
                if (!livroExiste)
                    throw new InvalidOperationException($"Livro com ID {id} não encontrado");

                var resultado = await _livroRepository.DeleteAsync(id);
                return resultado;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação
                throw;
            }
            catch (InvalidOperationException)
            {
                // Re-lança exceções de negócio
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException($"Erro ao deletar livro {id}", ex);
            }
        }
    }
}
