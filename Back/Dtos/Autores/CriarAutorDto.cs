namespace Back.Dtos.Autores
{
    public class CriarAutorDto
    {
        public required string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Nacionalidade { get; set; }
        public string? Biografia { get; set; }
    }
}
