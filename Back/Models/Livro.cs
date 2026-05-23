using System;

namespace Back.Models
{
    public class Livro
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public required int AutorId { get; set; }
        public string? ISBN { get; set; }
        public int? AnoPublicacao { get; set; }
        public string? Editora { get; set; }
        public string? Sinopse { get; set; }
        public string? Categoria { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }

        // Foreign key and navigation property
        public Autor? Autor { get; set; }
    }
}
