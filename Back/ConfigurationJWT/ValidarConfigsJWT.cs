using Back.ConfigurationJWT;
using System.Text;

namespace Back.ConfigurationJWT
{
  public class ValidarConfiguracoesJWT
  {
    public static void ValidateJwtConfigurations(JwtSettings configuracoes)
    {
      if (string.IsNullOrWhiteSpace(configuracoes.Emissor))
        throw new InvalidOperationException("JwtSettings:Emissor é obrigatório");

      if (string.IsNullOrWhiteSpace(configuracoes.Publico))
        throw new InvalidOperationException("JwtSettings:Publico é obrigatório");

      if (string.IsNullOrWhiteSpace(configuracoes.ChaveSecreta))
        throw new InvalidOperationException("JwtSettings:ChaveSecreta é obrigatória");

      if (Encoding.UTF8.GetByteCount(configuracoes.ChaveSecreta) < 32)
        throw new InvalidOperationException("JwtSettings:ChaveSecreta deve ter pelo menos 32 bytes");

      if (configuracoes.ExpiracaoMinutos <= 0)
        throw new InvalidOperationException("JwtSettings:ExpiracaoMinutos deve ser maior que zero");
    }
  }
}