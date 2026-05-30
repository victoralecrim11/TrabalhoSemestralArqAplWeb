using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Back.Models
{
    [BsonIgnoreExtraElements]
    public class Autor
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nome")]
        public required string Nome { get; set; }

        [BsonElement("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [BsonElement("nacionalidade")]
        public string? Nacionalidade { get; set; }

        [BsonElement("biografia")]
        public string? Biografia { get; set; }

        [BsonElement("dataCriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

    }
}
