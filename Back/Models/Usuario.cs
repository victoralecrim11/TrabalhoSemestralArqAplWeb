using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Back.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("senhaHash")]
        public required string SenhaHash { get; set; }

        [BsonElement("nome")]
        public required string Nome { get; set; }

        [BsonElement("perfil")]
        public required string Perfil { get; set; } // "admin" ou "usuario"

        [BsonElement("dataCriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        [BsonElement("ativo")]
        public bool Ativo { get; set; } = true;
    }
}
