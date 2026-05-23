namespace Back.Dtos.Auth
{
    public class RegistroDto
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required string Nome { get; set; }
    }
}
