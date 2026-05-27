using Back.Dtos.Auth;
using Back.Models;
using Back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Back.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("aplication/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        private readonly IJwtService _jwtService;

        public AuthController(IUsuarioService usuarioService, IJwtService jwtService)
        {
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }


        /// <summary>
        /// Registra publicamente um usuário comum.
        /// </summary>
        /// <param name="dto">Dados de registro do usuário</param>
        /// <returns>Retorna o resultado do usuario recém-criado</returns>
        [HttpPost("registro")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Registra um usuário",
            Description = "Registra publicamente um usuário comum com perfil padrão de 'usuario'. Não requer autenticação. Retorna token JWT.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
        {

            try
            {
                // Chama o service para registrar o usuário
                var usuario = await _usuarioService.RegisterAsync(dto);

                // Retorna o usuário recém-criado junto com o token JWT
                var token = _jwtService.GerarToken(usuario);
                return CreatedAtAction(nameof(Registro), new
                {
                    mensagem = "Usuário registrado com sucesso",
                    token,
                    expiraEm = _jwtService.ObterDataExpiracao(),
                    usuario = ToUsuarioResponse(usuario)
                });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao criar usuario", erro = ex.Message });
            }
        }

        /// <summary>
        /// Registra um administrador. Requer perfil admin.
        /// </summary>
        /// <param name="dto">Dados de registro do administrador.</param>
        /// <returns>Retorna os dados do administrador recém-criado com token JWT.</returns>
        [HttpPost("registro-admin")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RegistroAdmin([FromBody] RegistroDto dto)
        {
            try
            {
                var usuario = await _usuarioService.RegisterAdminAsync(dto);
                var token = _jwtService.GerarToken(usuario);
                return CreatedAtAction(nameof(RegistroAdmin), new
                {
                    mensagem = "Administrador registrado com sucesso",
                    token,
                    expiraEm = _jwtService.ObterDataExpiracao(),
                    usuario = ToUsuarioResponse(usuario)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao criar administrador", erro = ex.Message });
            }
        }



        /// <summary>
        /// Autentica usuário e retorna JWT.
        /// </summary>
        /// <param name="dto">Credenciais do usuário</param>
        /// <returns>Retorna os dados do usuário autenticado</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Autentica usuário e retorna JWT",
            Description = "Ponto central de autenticação para qualquer perfil do sistema. O perfil associado ao usuário será injetado no token gerado.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var usuario = await _usuarioService.AuthenticateAsync(dto);
                if (usuario == null)
                    return Unauthorized(new { mensagem = "Email ou senha inválidos" });
                var token = _jwtService.GerarToken(usuario);
                return Ok(new
                {
                    mensagem = "Login realizado com sucesso",
                    token = _jwtService.GerarToken(usuario),
                    expiraEm = _jwtService.ObterDataExpiracao(),
                    usuario = ToUsuarioResponse(usuario)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao autenticar", erro = ex.Message });
            }
        }

        private static object ToUsuarioResponse(Usuario usuario)
        {
            return new
            {
                usuario.Id,
                usuario.Email,
                usuario.Nome,
                usuario.Perfil,
                usuario.Ativo,
                usuario.DataCriacao,
                usuario.DataAtualizacao
            };
        }
    }
}