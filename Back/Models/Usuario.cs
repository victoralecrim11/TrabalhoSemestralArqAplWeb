using System;

namespace Back.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string SenhaHash { get; set; }
        public required string Nome { get; set; }
        public required string Perfil { get; set; } // "admin" ou "usuario"
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
