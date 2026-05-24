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

        /// <summary>
        /// Obtém um livro específico por ID
        /// </summary>
        public async Task<Livro?> GetByIdAsync(int id)
        {
            try
            {
                // Valida se o ID fornecido é válido (maior que 0)
                if (id <= 0)
                    throw new ArgumentException("ID do livro deve ser maior que 0", nameof(id));

                // Busca o livro no repositório
                var livro = await _livroRepository.GetByIdAsync(id);
                // Retorna o livro encontrado ou null se não existir
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

        /// <summary>
        /// Obtém todos os livros de um autor específico
        /// </summary>
        public async Task<IEnumerable<Livro>> GetByAutorIdAsync(int autorId)
        {
            try
            {
                // Valida se o ID do autor é válido
                if (autorId <= 0)
                    throw new ArgumentException("ID do autor deve ser maior que 0", nameof(autorId));

                // Verifica se o autor existe antes de buscar seus livros
                var autorExiste = await _autorRepository.ExistsAsync(autorId);
                if (!autorExiste)
                    throw new InvalidOperationException($"Autor com ID {autorId} não encontrado");

                // Busca todos os livros do autor
                var livros = await _livroRepository.GetByAutorIdAsync(autorId);
                // Retorna a coleção (pode estar vazia se o autor não tem livros)
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

                // Valida se o ID do autor é válido
                if (dto.AutorId <= 0)
                    throw new ArgumentException("ID do autor deve ser válido", nameof(dto.AutorId));

                // Verifica se o autor existe no banco de dados
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

        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        public async Task<Livro?> UpdateAsync(int id, AtualizarLivroDto dto)
        {
            try
            {
                // Valida se o ID fornecido é válido
                if (id <= 0)
                    throw new ArgumentException("ID do livro deve ser maior que 0", nameof(id));

                // Valida se o DTO foi fornecido
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Dados de atualização são obrigatórios");

                // Valida se pelo menos o título foi fornecido na atualização
                if (string.IsNullOrWhiteSpace(dto.Titulo))
                    throw new ArgumentException("Título do livro é obrigatório", nameof(dto.Titulo));

                // Busca o livro existente
                var livroExistente = await _livroRepository.GetByIdAsync(id);
                // Verifica se o livro foi encontrado
                if (livroExistente == null)
                    throw new InvalidOperationException($"Livro com ID {id} não encontrado");

                // Se um novo autor foi fornecido, valida sua existência
                if (dto.AutorId.HasValue && dto.AutorId.Value != livroExistente.AutorId)
                {
                    // Verifica se o novo autor existe
                    var autorExiste = await _autorRepository.ExistsAsync(dto.AutorId.Value);
                    if (!autorExiste)
                        throw new InvalidOperationException($"Autor com ID {dto.AutorId.Value} não encontrado");
                }

                // Cria um objeto Livro com os dados atualizados
                var livroAtualizado = new Livro
                {
                    // Mantém o ID do livro
                    Id = id,
                    // Usa o novo título fornecido
                    Titulo = dto.Titulo ?? livroExistente.Titulo,
                    // Usa o novo ID de autor ou mantém o existente
                    AutorId = dto.AutorId ?? livroExistente.AutorId,
                    // Usa o novo ISBN ou mantém o existente
                    ISBN = dto.ISBN ?? livroExistente.ISBN,
                    // Usa o novo ano de publicação ou mantém o existente
                    AnoPublicacao = dto.AnoPublicacao ?? livroExistente.AnoPublicacao,
                    // Usa a nova editora ou mantém a existente
                    Editora = dto.Editora ?? livroExistente.Editora,
                    // Usa a nova sinopse ou mantém a existente
                    Sinopse = dto.Sinopse ?? livroExistente.Sinopse,
                    // Usa a nova categoria ou mantém a existente
                    Categoria = dto.Categoria ?? livroExistente.Categoria
                };

                // Chama o repositório para atualizar o livro
                var resultado = await _livroRepository.UpdateAsync(id, livroAtualizado);
                // Retorna o livro atualizado
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

        /// <summary>
        /// Deleta um livro
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                // Valida se o ID fornecido é válido
                if (id <= 0)
                    throw new ArgumentException("ID do livro deve ser maior que 0", nameof(id));

                // Verifica se o livro existe antes de tentar deletar
                var livroExiste = await _livroRepository.ExistsAsync(id);
                if (!livroExiste)
                    throw new InvalidOperationException($"Livro com ID {id} não encontrado");

                // Chama o repositório para deletar o livro
                var resultado = await _livroRepository.DeleteAsync(id);
                // Retorna o resultado da deleção (true se sucesso)
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
