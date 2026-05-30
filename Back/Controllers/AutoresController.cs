using Back.Dtos.Autores;
using Back.Dtos.Livros;
using Back.Models;
using Back.Repositories;
using Back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/autores")]
    [Produces("application/json")]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService;
        private readonly ILivroService _livroService;

        public AutoresController(IAutorService autorService, ILivroService livroService)
        {
            _autorService = autorService ?? throw new ArgumentNullException(nameof(autorService));
            _livroService = livroService ?? throw new ArgumentNullException(nameof(livroService));
        }

        /// <summary>
        /// Lista todos os autores.
        /// </summary>
        /// <returns>Retorna lista completa de autores.</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista autores", Description = "Retorna todos os autores cadastrados.")]
        [ProducesResponseType(typeof(IEnumerable<Autor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAutores()
        {
            try
            {
                var autores = await _autorService.GetAllAsync();
                return Ok(new { dados = autores, total = autores.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = "Erro ao obter autores",
                    erroReal = ex.Message,
                    detalhe = ex.InnerException?.Message
                });
            }
        }


        /// <summary>
        /// Obtém um autor por ID, incluindo seus livros.
        /// </summary>
        /// <param name="id">ID numérico do autor.</param>
        /// <returns>Retorna os detalhes do autor encontrado, incluindo seus livros.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obtém autor por ID", Description = "Retorna os detalhes de um autor específico com base no ID fornecido.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAutorById(string id)
        {
            try
            {
                var autor = await _autorService.GetByIdAsync(id);
                if (autor == null)
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });

                var livros = await _livroService.GetByAutorIdAsync(id);

                return Ok(new
                {
                    autor,
                    livros = livros.ToList(),
                    totalLivros = livros.Count()
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
                return BadRequest(new
                {
                    mensagem = "Erro ao obter autor",
                    erroReal = ex.Message,
                    detalhe = ex.InnerException?.Message
                });
            }
        }


        /// <summary>
        /// Cria um novo autor Requer perfil Administrador.
        /// </summary>
        /// <param name="dto">Dados para criação do autor.</param>
        /// <returns>Retorna mensagem de sucesso e o autor criado.</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Cria autor", Description = "Requer perfil admin.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAutor([FromBody] CriarAutorDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do autor são obrigatórios" });

                var autorCriado = await _autorService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAutorById), new { id = autorCriado.Id }, new
                {
                    mensagem = "Autor criado com sucesso",
                    autor = autorCriado
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um autor por ID.
        /// </summary>
        /// <param name="id">ID numérico do autor.</param>
        /// <param name="dto">Dados para atualização do autor.</param>
        /// <returns>Retorna mensagem de sucesso e o autor atualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Atualiza autor por ID", Description = "Requer perfil admin. Atualiza os detalhes de um autor específico com base no ID fornecido.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAutor(string id, [FromBody] AtualizarAutorDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do autor são obrigatórios" });

                var autorAtualizado = await _autorService.UpdateAsync(id, dto);
                if (autorAtualizado == null)
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado." });

                return Ok(new
                {
                    mensagem = "Autor atualizado com sucesso",
                    autor = autorAtualizado
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um autor por ID. Requer perfil admin.
        /// </summary>
        /// <param name="id">ID numérico do autor.</param>
        /// <returns>Retorna mensagem de sucesso.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Deleta autor por ID", Description = "Requer perfil admin. Remove um autor específico com base no ID fornecido.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAutor(string id)
        {
            try
            {
                var resultado = await _autorService.DeleteAsync(id);

                if (!resultado)
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });

                return Ok(new { mensagem = "Autor deletado com sucesso" });
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
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

