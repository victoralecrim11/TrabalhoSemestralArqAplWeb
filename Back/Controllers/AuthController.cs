using Microsoft.AspNetCore.Mvc;
using Back.Dtos.Auth;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Registra um usuário com perfil admin ou usuário.
        /// </summary>
        /// <param name="dto">Dados de registro do usuário</param>
        /// <returns>Retorna o resultado do registro</returns>
        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
        {
            // TODO: Implementar lógica de registro
            return Ok("Usuário registrado com sucesso");
        }

        /// <summary>
        /// Autentica usuário e retorna JWT.
        /// </summary>
        /// <param name="dto">Credenciais do usuário</param>
        /// <returns>Retorna o token JWT</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // TODO: Implementar lógica de autenticação
            return Ok(new { token = "JWT_TOKEN" });
        }
    }
}