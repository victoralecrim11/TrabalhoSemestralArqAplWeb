using Back.Dtos.Autores;
using Back.Dtos.Livros;
using Back.Models;
using Back.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/livros")]
    public class LivrosController : ControllerBase
    {

        private readonly ILivroRepository _livroRepository;
        private readonly IAutorRepository _autorRepository;

        public LivrosController(ILivroRepository livroRepository, IAutorRepository autorRepository)
        {
            _livroRepository = livroRepository ?? throw new ArgumentNullException(nameof(livroRepository));
            _autorRepository = autorRepository ?? throw new ArgumentNullException(nameof(autorRepository));
        }

        /// <summary>
        /// Lista todos os livros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Livro>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLivros()
        {
            try
            {
                var livros = await _livroRepository.GetAllAsync();
                return Ok(new { dados = livros, total = livros.Count() });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar livros", erro = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo livro
        /// </summary>
        /// <param name="dto">Dados do livro</param>
        /// <returns>Retorna o livro criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Livro), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateLivro([FromBody] CriarLivroDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Titulo))
                    return BadRequest(new { mensagem = "Título do livro é obrigatório" });
                if (dto.AutorId <= 0)
                    return BadRequest(new { mensagem = "ID do autor inválido" });

                // Verificar se o autor existe
                var autorExiste = await _autorRepository.ExistsAsync(dto.AutorId);
                if (!autorExiste)
                    return NotFound(new { mensagem = $"Autor com ID {dto.AutorId} não encontrado" });

                var livro = new Livro
                {
                    Titulo = dto.Titulo,
                    AutorId = dto.AutorId,
                    ISBN = dto.ISBN,
                    AnoPublicacao = dto.AnoPublicacao,
                    Editora = dto.Editora,
                    Sinopse = dto.Sinopse,
                    Categoria = dto.Categoria
                };

                var livroCriado = await _livroRepository.CreateAsync(livro);
                return CreatedAtAction(nameof(GetLivroById), new { id = livroCriado.Id }, livroCriado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao criar livro", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um livro por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<Livro>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLivroById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var livro = await _livroRepository.GetByIdAsync(id);
                if (livro == null)
                    return NotFound(new { mensagem = $"Livro com ID {id} não encontrado" });

                return Ok(livro);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar livro", erro = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Livro), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLivro(int id, [FromBody] AtualizarLivroDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var livroExistente = await _livroRepository.GetByIdAsync(id);
                if (livroExistente == null)
                    return NotFound(new { mensagem = $"Livro com ID {id} não encontrado" });

                // Verificar se o novo autor existe (se foi alterado)
                if (dto.AutorId.HasValue && dto.AutorId != livroExistente.AutorId)
                {
                    var autorExiste = await _autorRepository.ExistsAsync(dto.AutorId.Value);
                    if (!autorExiste)
                        return NotFound(new { mensagem = $"Autor com ID {dto.AutorId} não encontrado" });
                }

                // Atualizar os campos do livro existente com os dados do DTO
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

                var atualizado = await _livroRepository.UpdateAsync(id, livroAtualizado);
                return Ok(atualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao atualizar livro", erro = ex.Message });
            }

            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var livroExistente = await _livroRepository.GetByIdAsync(id);
                if (livroExistente == null)
                    return NotFound(new { mensagem = $"Livro com ID {id} não encontrado" });

                await _livroRepository.DeleteAsync(id);
                return Ok(new { mensagem = "Livro removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao deletar livro", erro = ex.Message });
            }
        }
    }
}