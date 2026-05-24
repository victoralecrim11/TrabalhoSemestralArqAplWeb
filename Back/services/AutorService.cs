using Back.Dtos.Autores;
using Back.Models;
using Back.Repositories;

namespace Back.Services
{
    /// <summary>
    /// Serviço de Autores que encapsula a lógica de negócio.
    /// Responsável por validações, verificações de integridade referencial e tratamento de erros.
    /// </summary>
    public class AutorService : IAutorService
    {
        // Injeção de dependência do repositório de autores
        private readonly IAutorRepository _autorRepository;
        // Injeção de dependência do repositório de livros para validar integridade referencial
        private readonly ILivroRepository _livroRepository;

        /// <summary>
        /// Construtor com injeção de dependências
        /// </summary>
        /// <param name="autorRepository">Repositório para acesso a dados de autores</param>
        /// <param name="livroRepository">Repositório para validar livros associados</param>
        public AutorService(IAutorRepository autorRepository, ILivroRepository livroRepository)
        {
            // Valida se o repositório de autores foi fornecido
            _autorRepository = autorRepository ?? throw new ArgumentNullException(nameof(autorRepository));
            // Valida se o repositório de livros foi fornecido
            _livroRepository = livroRepository ?? throw new ArgumentNullException(nameof(livroRepository));
        }

        /// <summary>
        /// Obtém todos os autores cadastrados
        /// </summary>
        public async Task<IEnumerable<Autor>> GetAllAsync()
        {
            try
            {
                // Chama o repositório para buscar todos os autores
                var autores = await _autorRepository.GetAllAsync();
                // Retorna a coleção de autores (pode estar vazia)
                return autores;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa para o controlador
                throw new InvalidOperationException("Erro ao obter autores", ex);
            }
        }

        /// <summary>
        /// Obtém um autor específico por ID
        /// </summary>
        public async Task<Autor?> GetByIdAsync(int id)
        {
            try
            {
                // Valida se o ID fornecido é válido (maior que 0)
                if (id <= 0)
                    throw new ArgumentException("ID do autor deve ser maior que 0", nameof(id));

                // Busca o autor no repositório
                var autor = await _autorRepository.GetByIdAsync(id);
                // Retorna o autor encontrado ou null se não existir
                return autor;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException($"Erro ao obter autor com ID {id}", ex);
            }
        }

        /// <summary>
        /// Cria um novo autor com validações de negócio
        /// </summary>
        public async Task<Autor> CreateAsync(CriarAutorDto dto)
        {
            try
            {
                // Valida se o DTO foi fornecido
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Dados do autor são obrigatórios");

                // Valida se o nome do autor foi fornecido e não está vazio
                if (string.IsNullOrWhiteSpace(dto.Nome))
                    throw new ArgumentException("Nome do autor é obrigatório", nameof(dto.Nome));

                // Valida se o nome possui comprimento mínimo (pelo menos 3 caracteres)
                if (dto.Nome.Length < 3)
                    throw new ArgumentException("Nome do autor deve ter no mínimo 3 caracteres", nameof(dto.Nome));

                // Cria uma nova instância de Autor com os dados do DTO
                var novoAutor = new Autor
                {
                    // Atribui o nome fornecido (já validado)
                    Nome = dto.Nome,
                    // Atribui a data de nascimento (pode ser nula)
                    DataNascimento = dto.DataNascimento,
                    // Atribui a nacionalidade (pode ser nula)
                    Nacionalidade = dto.Nacionalidade,
                    // Atribui a biografia (pode ser nula)
                    Biografia = dto.Biografia
                };

                // Chama o repositório para persistir o novo autor
                var autorCriado = await _autorRepository.CreateAsync(novoAutor);
                // Retorna o autor criado com ID gerado
                return autorCriado;
            }
            catch (ArgumentException)
            {
                // Re-lança exceções de validação (inclui ArgumentNullException)
                throw;
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais informativa
                throw new InvalidOperationException("Erro ao criar autor", ex);
            }
        }

        /// <summary>
        /// Atualiza um autor existente
        /// </summary>
        public async Task<Autor?> UpdateAsync(int id, AtualizarAutorDto dto)
        {
            try
            {
                // Valida se o ID fornecido é válido
                if (id <= 0)
                    throw new ArgumentException("ID do autor deve ser maior que 0", nameof(id));

                // Valida se o DTO foi fornecido
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Dados de atualização são obrigatórios");

                // Valida se pelo menos o nome foi fornecido na atualização
                if (string.IsNullOrWhiteSpace(dto.Nome))
                    throw new ArgumentException("Nome do autor é obrigatório", nameof(dto.Nome));

                // Valida comprimento mínimo do nome
                if (dto.Nome.Length < 3)
                    throw new ArgumentException("Nome do autor deve ter no mínimo 3 caracteres", nameof(dto.Nome));

                // Busca o autor existente
                var autorExistente = await _autorRepository.GetByIdAsync(id);
                // Verifica se o autor foi encontrado
                if (autorExistente == null)
                    throw new InvalidOperationException($"Autor com ID {id} não encontrado");

                // Cria um objeto Autor com os dados atualizados
                var autorAtualizado = new Autor
                {
                    // Mantém o ID do autor
                    Id = id,
                    // Usa o novo nome fornecido
                    Nome = dto.Nome ?? autorExistente.Nome,
                    // Usa a nova data de nascimento ou mantém a existente
                    DataNascimento = dto.DataNascimento ?? autorExistente.DataNascimento,
                    // Usa a nova nacionalidade ou mantém a existente
                    Nacionalidade = dto.Nacionalidade ?? autorExistente.Nacionalidade,
                    // Usa a nova biografia ou mantém a existente
                    Biografia = dto.Biografia ?? autorExistente.Biografia
                };

                // Chama o repositório para atualizar o autor
                var resultado = await _autorRepository.UpdateAsync(id, autorAtualizado);
                // Retorna o autor atualizado
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
                throw new InvalidOperationException($"Erro ao atualizar autor {id}", ex);
            }
        }

        /// <summary>
        /// Deleta um autor com verificação de integridade referencial (livros associados)
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                // Valida se o ID fornecido é válido
                if (id <= 0)
                    throw new ArgumentException("ID do autor deve ser maior que 0", nameof(id));

                // Verifica se o autor existe antes de tentar deletar
                var autorExiste = await _autorRepository.ExistsAsync(id);
                if (!autorExiste)
                    throw new InvalidOperationException($"Autor com ID {id} não encontrado");

                // Verifica se o autor possui livros associados (integridade referencial)
                var temLivros = await _autorRepository.HasLivrosAsync(id);
                if (temLivros)
                    // Impede a deleção de autores que possuem livros
                    throw new InvalidOperationException(
                        $"Não é possível deletar o autor {id} pois possui livros associados. " +
                        "Remova os livros primeiro ou atualize sua autoria.");

                // Chama o repositório para deletar o autor
                var resultado = await _autorRepository.DeleteAsync(id);
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
                throw new InvalidOperationException($"Erro ao deletar autor {id}", ex);
            }
        }
    }
}
