using System.Security.Claims;
using Back.ConfigurationJWT;
using Back.Data;
using Back.Filters;
using Back.Repositories;
using Back.Services;
using SwaggerThemes;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários para a aplicação, incluindo os serviços de negócio e repositórios.
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<IAutorRepository, AutorRepository>();

// Carrega a seção JwtSettings do appsettings.json.
var configuracoesJwt = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = configuracoesJwt.Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração JwtSettings não encontrada");

// Valida as configurações JWT para garantir que estão corretas e completas.
ValidarConfiguracoesJWT.ValidateJwtConfigurations(jwtSettings);

// Registra as configurações JWT e o serviço de geração de tokens JWT.
builder.Services.Configure<JwtSettings>(configuracoesJwt);
builder.Services.AddScoped<IJwtService, JwtService>();

//Registrar o DataContext como singleton para manter os dados em memória durante toda a execução da aplicação
builder.Services.AddSingleton<DataContext>(sp =>
{
    var contexto = new DataContext();
    contexto.InitializeSeedData();
    return contexto;
});


// Configura autenticação por Bearer token usando JWT.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Emissor,
            ValidAudience = jwtSettings.Publico,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSettings.ChaveSecreta)),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero // Elimina o tempo de tolerância para expiração do token
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            // Disparado quando o token está ausente ou inválido (401)
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        mensagem = "Acesso não autorizado. Você precisa estar autenticado para acessar este recurso.",
                        dica = "Faça login em /api/v1/auth/login e utilize o token JWT retornado."
                    })
                );
            },

            // Disparado quando o token é válido mas o perfil não tem permissão (403)
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        mensagem = "Acesso negado. Você não tem permissão para acessar este recurso.",
                        dica = "Este endpoint requer perfil de administrador."
                    })
                );
            }
        };
    });



builder.Services.AddSwaggerGen(opcoes =>
{
    opcoes.OperationFilter<AuthorizeCheckOperationFilter>();

    opcoes.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Biblioteca API",
        Description = "API para gerenciamento de livros e autores em uma biblioteca"
    });

    // Configura o Swagger para aceitar tokens JWT no cabeçalho Authorization e mostra quais Endpoints requerem autenticação.
    opcoes.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(Theme.Dracula);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseSwaggerUI(Theme.Dracula);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();