using Microsoft.AspNetCore.Mvc;
using Back.Dtos.Autores;
using Back.Models;
using Back.Repositories;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/autores")]
    public class AutoresController : ControllerBase
    {

        private readonly IAutorRepository _autorRepository;
        private readonly ILivroRepository _livroRepository;

        public AutoresController(IAutorRepository autorRepository, ILivroRepository livroRepository)
        {

            _autorRepository = autorRepository ?? throw new ArgumentNullException(nameof(autorRepository));
            _livroRepository = livroRepository ?? throw new ArgumentNullException(nameof(livroRepository));
        }

        /// <summary>
        /// Lista todos os autores
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Autor>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetAutores()
        {
            try
            {
                var autores = await _autorRepository.GetAllAsync();
                return Ok(new { dados = autores, total = autores.Count() });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar autores", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um autor por ID
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Retorna o autor encontrado</returns>

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Autor), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAutorById(int id)
        {
            try
            {
                var autor = await _autorRepository.GetByIdAsync(id);
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });
                if (autor == null)
                {
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });
                }
                return Ok(autor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar autor", erro = ex.Message });
            }
        }

        [HttpGet("{id}/com-livros")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAutorWithLivros(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var autor = await _autorRepository.GetByIdAsync(id);
                if (autor == null)
                {
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });
                }

                var livros = await _livroRepository.GetByAutorIdAsync(id);
                return Ok(new
                {
                    autor,
                    livros = livros.ToList(),
                    totalLivros = livros.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar autor com livros", erro = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo autor. Requer perfil admin.
        /// </summary>
        /// <param name="dto">Dados do autor</param>
        /// <returns>Retorna o autor criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Autor), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAutor([FromBody] CriarAutorDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Nome))
                    return BadRequest(new { mensagem = "Nome do autor é obrigatório" });

                var autor = new Autor
                {
                    Nome = dto.Nome,
                    DataNascimento = dto.DataNascimento,
                    Nacionalidade = dto.Nacionalidade,
                    Biografia = dto.Biografia
                };

                var autorCriado = await _autorRepository.CreateAsync(autor);
                return CreatedAtAction(nameof(GetAutorById), new { id = autorCriado.Id }, autorCriado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao criar autor", erro = ex.Message });
            }

        }

        /// <summary>
        /// Obtém um autor por ID
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Retorna o autor encontrado</returns>

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAutor(int id, [FromBody] AtualizarAutorDto dto)
        {

            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var autorExistente = await _autorRepository.GetByIdAsync(id);
                if (autorExistente == null)
                {
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });
                }

                var autorAtualizado = new Autor
                {
                    Id = id,
                    Nome = dto.Nome,
                    DataNascimento = dto.DataNascimento,
                    Nacionalidade = dto.Nacionalidade,
                    Biografia = dto.Biografia
                };
                var resultado = await _autorRepository.UpdateAsync(id, autorAtualizado);
                if (resultado == null)
                    return NotFound(new { mensagem = "Erro ao atualizar autor" });

                return Ok(new { mensagem = $"Autor com ID {id} atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao atualizar autor", erro = ex.Message });
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove um autor. Requer perfil admin.
        /// </summary>
        /// <param name="id">ID do autor</param>
        /// <returns>Retorna mensagem de sucesso</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var autorExistente = await _autorRepository.GetByIdAsync(id);
                if (autorExistente == null)
                {
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });
                }

                var livrosDoAutor = await _livroRepository.GetByAutorIdAsync(id);

                // Verificar se tem livros associados ao autor
                if (livrosDoAutor.Any())
                {
                    return BadRequest(new { mensagem = "Não é possível deletar um autor que possui livros associados" });
                }

                var resultado = await _autorRepository.DeleteAsync(id);


                if (!resultado)
                    return NotFound(new { mensagem = "Erro ao deletar autor" });

                return Ok(new { mensagem = $"Autor com ID {id} deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao deletar autor", erro = ex.Message });
            }
        }
    }
}