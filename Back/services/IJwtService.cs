using Back.Models;

namespace Back.Services
{
    public interface IJwtService
    {
        string GerarToken(Usuario usuario);
        DateTime ObterDataExpiracao();
    }
}
