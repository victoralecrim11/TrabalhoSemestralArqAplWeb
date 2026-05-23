using System;
using System.Collections.Generic;

namespace Back.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Nacionalidade { get; set; }
        public string? Biografia { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }

        // Navigation property
        public ICollection<Livro>? Livros { get; set; }
    }
}
