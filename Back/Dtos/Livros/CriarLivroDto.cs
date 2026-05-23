namespace Back.Dtos.Livros
{
    public class CriarLivroDto
    {
        public required string Titulo { get; set; }
        public required int AutorId { get; set; }
        public string? ISBN { get; set; }
        public int? AnoPublicacao { get; set; }
        public string? Editora { get; set; }
        public string? Sinopse { get; set; }
        public string? Categoria { get; set; }
    }
}
