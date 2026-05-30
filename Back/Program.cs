using Back.ConfigurationJWT;
using Back.Data;
using Back.Dtos.Autores;
using Back.Filters;
using Back.Models;
using Back.Repositories;
using Back.Services;
using Back.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SwaggerThemes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Carrega a seção JwtSettings do appsettings.json.
var configuracoesJwt = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = configuracoesJwt.Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração JwtSettings não encontrada");

// Valida as configurações JWT para garantir que estão corretas e completas.
ValidarConfiguracoesJWT.ValidateJwtConfigurations(jwtSettings);

// Registra as configurações JWT e o serviço de geração de tokens JWT.
builder.Services.Configure<JwtSettings>(configuracoesJwt);
builder.Services.AddScoped<IJwtService, JwtService>();

// Configura o MongoDB se as configurações estiverem presentes, caso contrário, utiliza o DataContext em memória.
var mongoSection = builder.Configuration.GetSection("MongoSettings");

if (mongoSection.Exists())
{
    builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(mongoSection.Key));
    builder.Services.AddSingleton<MongoDbContext>();
    builder.Services.AddScoped<IUsuarioRepository, MongoUsuarioRepository>();
    builder.Services.AddScoped<ILivroRepository, MongoLivroRepository>();
    builder.Services.AddScoped<IAutorRepository, MongoAutorRepository>();
    builder.Services.AddScoped<MongoSeeder>();
}

// Registrar DataContext como singleton (em memória) apenas se Mongo não estiver configurado
if (!mongoSection.Exists())
{
    //Registrar o DataContext como singleton para manter os dados em memória durante toda a execução da aplicação
    builder.Services.AddSingleton<DataContext>(sp =>
    {
        var contexto = new DataContext();
        contexto.InitializeSeedData();
        return contexto;
    });
}

// Registrar repositórios com ciclo de vida Scoped (somente se Mongo não estiver configurado)
if (!mongoSection.Exists())
{
    builder.Services.AddScoped<ILivroRepository, LivroRepository>();
    builder.Services.AddScoped<IAutorRepository, AutorRepository>();
    builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
}


// Adiciona os serviços necessários para a aplicação, incluindo os serviços de negócio para livros, autores e usuários.
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IValidator<Autor>, AutorValidator>();
builder.Services.AddScoped<IValidator<Livro>, LivroValidator>();
builder.Services.AddScoped<IValidator<Usuario>, UsuarioValidator>();
builder.Services.AddScoped<IValidator<CriarAutorDto>, CriarAutorDtoValidator>();
builder.Services.AddScoped<IValidator<AtualizarAutorDto>, AtualizarAutorDtoValidator>();



// Configura CORS para permitir requisições de qualquer origem, o que é útil durante o desenvolvimento e testes.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Configura autenticação por Bearer token usando JWT com DefaultChallengeScheme definido.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authorization = context.Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(authorization))
                    return Task.CompletedTask;

                var token = authorization.Trim();
                while (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                context.Token = token.Trim('"');
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.HttpContext.Items["AuthError"] = context.Exception;
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var mensagem = "Acesso não autorizado. Você precisa estar autenticado para acessar este recurso.";
                var dica = "Faça login em /api/v1/auth/login e utilize o token JWT retornado.";

                if (context.HttpContext.Items["AuthError"] is Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
                {
                    mensagem = "Token expirado.";
                    dica = "Faça login novamente em /api/v1/auth/login e atualize o token no Swagger.";
                }
                else if (context.HttpContext.Items.ContainsKey("AuthError"))
                {
                    mensagem = "Token inválido.";
                    dica = "Informe apenas o JWT retornado no login. No Swagger, não digite o prefixo Bearer.";
                }

                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        mensagem,
                        dica
                    })
                );
            },

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

    // Habilita os comentarios dos Endpoints 
    opcoes.EnableAnnotations();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opcoes.IncludeXmlComments(xmlPath);

    // Configura o Swagger para aceitar tokens JWT no cabeçalho Authorization e mostra quais Endpoints requerem autenticação.
    opcoes.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira APENAS o token JWT, sem o prefixo 'Bearer'. Ex: eyJhbGci..."
    });

});

builder.Services.AddScoped<FluentValidatorFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidatorFilter>();
});

var app = builder.Build();

// Executar seeding do MongoDB se estiver configurado.
if (mongoSection.Exists())
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<MongoSeeder>();
        await seeder.SeedAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/swagger-ui/dracula.css"); // tema Dracula (forma correta)
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

        // Desabilita o syntax highlight que causa o loop
        c.ConfigObject.AdditionalItems["syntaxHighlight"] = false;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/swagger-ui/dracula.css"); // tema Dracula (forma correta)
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

        // Desabilita o syntax highlight que causa o loop
        c.ConfigObject.AdditionalItems["syntaxHighlight"] = false;
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
