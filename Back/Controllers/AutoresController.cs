using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/autores")]
    public class AutoresController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAutores(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10,
            [FromQuery] string ordenacao = "nome",
            [FromQuery] string? busca = null,
            [FromQuery] string? nacionalidade = null)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAutorById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}/com-livros")]
        public async Task<IActionResult> GetAutorWithLivros(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("buscar/{nome}")]
        public async Task<IActionResult> SearchAutor(string nome)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAutor([FromBody] object dto)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAutor(int id, [FromBody] object dto)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            throw new NotImplementedException();
        }
    }
}