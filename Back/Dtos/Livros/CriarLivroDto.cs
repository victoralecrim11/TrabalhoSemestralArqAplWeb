namespace Back.Dtos.Livros
{
    /// <summary>
    /// Dados para criação de um livro.
    /// </summary>
    public class CriarLivroDto
    {
        /// <summary>
        /// Título do livro.
        /// </summary>
        public required string Titulo { get; set; }

        /// <summary>
        /// ID de um autor já cadastrado.
        /// </summary>
        public required string AutorId { get; set; }

        /// <summary>
        /// ISBN do livro.
        /// </summary>
        public string? ISBN { get; set; }

        /// <summary>
        /// Ano de publicação do livro.
        /// </summary>
        public int? AnoPublicacao { get; set; }

        /// <summary>
        /// Editora do livro.
        /// </summary>
        public string? Editora { get; set; }

        /// <summary>
        /// Sinopse do livro.
        /// </summary>
        public string? Sinopse { get; set; }

        /// <summary>
        /// Categoria do livro.
        /// </summary>
        public string? Categoria { get; set; }
    }
}
