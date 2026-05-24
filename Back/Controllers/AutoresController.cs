using Back.Dtos.Autores;
using Back.Dtos.Livros;
using Back.Models;
using Back.Repositories;
using Back.Services;
using Microsoft.AspNetCore.Mvc;
namespace Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                return BadRequest(new { mensagem = ex.Message });
            }
        }
        
        
        /// <summary>
        /// Obtém um autor por ID, incluindo seus livros.
        /// </summary>
        /// <param name="id">ID numérico do autor.</param>
        /// <returns>Retorna os detalhes do autor encontrado, incluindo seus livros.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAutorById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

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
                return BadRequest(new { mensagem = ex.Message });
            }
        }


        /// <summary>
        /// Cria um novo autor.
        /// </summary>
        /// <param name="dto">Dados para criação do autor.</param>
        /// <returns>Retorna o autor criado com status 201 Created.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Autor), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Autor), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAutor([FromBody] CriarAutorDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do autor são obrigatórios" });

                var autorCriado = await _autorService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAutorById), new { id = autorCriado.Id }, autorCriado);
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
        /// Obtém um autor por ID.
        /// </summary>
        /// <param name="id">ID numérico do autor.</param>
        /// <returns>Retorna os detalhes do autor encontrado.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAutor(int id, [FromBody] AtualizarAutorDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do autor são obrigatórios" });

                var autorAtualizado = await _autorService.UpdateAsync(id, dto);
                if (autorAtualizado == null)
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado." });

                return NoContent();
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { mensagem = "ID inválido" });

                var resultado = await _autorService.DeleteAsync(id);

                if (!resultado)
                    return NotFound(new { mensagem = $"Autor com ID {id} não encontrado" });

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
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

