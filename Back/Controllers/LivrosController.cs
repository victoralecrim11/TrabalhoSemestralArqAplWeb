using Back.Dtos.Livros;
using Back.Models;
using Back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/livros")]
    public class LivrosController : ControllerBase
    {

        private readonly ILivroService _livroService;

        public LivrosController(ILivroService livroService)
        {
            _livroService = livroService ?? throw new ArgumentNullException(nameof(livroService));
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
                var livros = await _livroService.GetAllAsync();
                return Ok(new { dados = livros, total = livros.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo livro (Requer perfil Administrador)
        /// </summary>
        /// <param name="dto">Dados do livro</param>
        /// <returns>Retorna o livro criado</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(Livro), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateLivro([FromBody] CriarLivroDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do livro são obrigatórios" });

                var livroCriado = await _livroService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetLivroById), new { id = livroCriado.Id }, livroCriado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensagem = ex.Message });
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

                var livro = await _livroService.GetByIdAsync(id);
                if (livro == null)
                    return NotFound(new { mensagem = $"Livro com ID {id} não encontrado" });

                return Ok(livro);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar livro", erro = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um livro por ID (Requer perfil Administrador)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(Livro), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLivro(int id, [FromBody] AtualizarLivroDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                if (dto == null)
                    return BadRequest(new { mensagem = "Dados para atualização são obrigatórios" });

                var resultado = await _livroService.UpdateAsync(id, dto);
                if (resultado == null)
                    return NotFound(new { mensagem = "Erro ao atualizar livro" });

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao atualizar livro", erro = ex.Message });
            }

        }

        /// <summary>
        /// Deleta um livro por ID (Requer perfil Administrador)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var resultado = await _livroService.DeleteAsync(id);

                if (!resultado)
                    return NotFound(new { mensagem = $"Livro com ID {id} não encontrado" });

                return Ok(new { mensagem = "Registro deletado com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao deletar livro", erro = ex.Message });
            }
        }
    }
}