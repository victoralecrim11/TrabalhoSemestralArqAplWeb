using Back.Dtos.Livros;
using Back.Models;
using Back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/livros")]
    [Produces("application/json")]
    public class LivrosController : ControllerBase
    {

        private readonly ILivroService _livroService;

        public LivrosController(ILivroService livroService)
        {
            _livroService = livroService ?? throw new ArgumentNullException(nameof(livroService));
        }

        /// <summary>
        /// Lista todos os livros.
        /// </summary>
        /// <returns>Retorna lista completa de livros.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Lista livros", Description = "Retorna todos os livros cadastrados.")]
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
                return BadRequest(new
                {
                    mensagem = ex.Message,
                    erroReal = ex.InnerException?.Message,
                    detalhe = ex.InnerException?.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Cria um novo livro (Requer perfil Administrador)
        /// </summary>
        /// <param name="dto">Dados para criação do livro.</param>
        /// <returns>Retorna mensagem de sucesso e o livro criado.</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Cria livro", Description = "Requer perfil admin. O livro deve ser vinculado a um autor já cadastrado pelo campo autorId.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateLivro([FromBody] CriarLivroDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do livro são obrigatórios" });

                var livroCriado = await _livroService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetLivroById), new { id = livroCriado.Id }, new
                {
                    mensagem = "Livro criado com sucesso",
                    livro = livroCriado
                });
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
        /// Obtém um livro por ID.
        /// </summary>
        /// <param name="id">ID do livro.</param>
        /// <returns>Retorna os detalhes do livro encontrado.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém livro por ID", Description = "Retorna os detalhes de um livro específico com base no ID fornecido.")]
        [ProducesResponseType(typeof(Livro), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLivroById(string id)
        {
            try
            {
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
        /// <param name="id">ID do livro.</param>
        /// <param name="dto">Dados para atualização do livro.</param>
        /// <returns>Retorna mensagem de sucesso e o livro atualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Atualiza livro por ID", Description = "Requer perfil admin. Atualiza os detalhes de um livro específico com base no ID fornecido.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateLivro(string id, [FromBody] AtualizarLivroDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados para atualização são obrigatórios" });

                var resultado = await _livroService.UpdateAsync(id, dto);
                if (resultado == null)
                    return NotFound(new { mensagem = "Erro ao atualizar livro" });

                return Ok(new
                {
                    mensagem = "Livro atualizado com sucesso",
                    livro = resultado
                });
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
        /// <param name="id">ID do livro.</param>
        /// <returns>Retorna mensagem de sucesso.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Deleta livro por ID", Description = "Requer perfil admin. Remove um livro específico com base no ID fornecido.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteLivro(string id)
        {
            try
            {
                var resultado = await _livroService.DeleteAsync(id);

                if (!resultado)
                    return NotFound(new { mensagem = $"Livro com ID {id} não encontrado" });

                return Ok(new { mensagem = "Livro deletado com sucesso" });
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
