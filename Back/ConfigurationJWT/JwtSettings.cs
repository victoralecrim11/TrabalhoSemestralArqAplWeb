namespace Back.ConfigurationJWT
{
    public class JwtSettings
    {
        public required string Emissor { get; set; }
        public required string Publico { get; set; }
        public required string ChaveSecreta { get; set; }
        public int ExpiracaoMinutos { get; set; }
    }
}
