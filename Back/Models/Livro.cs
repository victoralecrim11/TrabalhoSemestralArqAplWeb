using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Back.Models
{
    public class Livro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("titulo")]
        public required string Titulo { get; set; }

        [BsonElement("autorId")]
        public required string AutorId { get; set; }

        [BsonElement("isbn")]
        public string? ISBN { get; set; }

        [BsonElement("anoPublicacao")]
        public int? AnoPublicacao { get; set; }

        [BsonElement("editora")]
        public string? Editora { get; set; }

        [BsonElement("sinopse")]
        public string? Sinopse { get; set; }

        [BsonElement("categoria")]
        public string? Categoria { get; set; }

        [BsonElement("dataCriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }
    }
}
