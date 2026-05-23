using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/livros")]
    public class LivrosController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLivros(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10,
            [FromQuery] string? busca = null,
            [FromQuery] string? categoria = null,
            [FromQuery] int? autorId = null)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLivroById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("autor/{autorId}")]
        public async Task<IActionResult> GetLivrosByAutor(int autorId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<IActionResult> GetLivrosByCategoria(string categoria)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLivro([FromBody] object dto)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLivro(int id, [FromBody] object dto)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            throw new NotImplementedException();
        }
    }
}