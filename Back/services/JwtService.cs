using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Back.ConfigurationJWT;
using Back.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Back.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _configuracoesJwt;

        public JwtService(IOptions<JwtSettings> configuracoesJwt)
        {
            _configuracoesJwt = configuracoesJwt.Value;
        }

        public string GerarToken(Usuario usuario)
        {
            var emitidoEm = DateTimeOffset.UtcNow;

         
            var dadosUsuario = new List<Claim>
            {
                // Identificadores do JWT
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, emitidoEm.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                
                // Dados do usuário 
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email),
                
                // O segredo da Autorização: Define a Role (perfil) que o middleware vai validar
                new(ClaimTypes.Role, usuario.Perfil.ToLowerInvariant())
            };

            var chaveSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracoesJwt.ChaveSecreta));
            var credenciaisAssinatura = new SigningCredentials(chaveSecreta, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuracoesJwt.Emissor,
                audience: _configuracoesJwt.Publico,
                claims: dadosUsuario,
                // notBefore: emitidoEm.UtcDateTime,
                expires: ObterDataExpiracao(),
                signingCredentials: credenciaisAssinatura);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime ObterDataExpiracao()
        {
            return DateTime.UtcNow.AddMinutes(_configuracoesJwt.ExpiracaoMinutos);
        }
    }
}