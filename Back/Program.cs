using Back.Data;
using Back.Repositories;
using Back.Services;
using SwaggerThemes;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<IAutorService, AutorService>();   
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<IAutorRepository, AutorRepository>();

//Registrar o DataContext como singleton para manter os dados em memória durante toda a execução da aplicação
builder.Services.AddSingleton<DataContext>(sp =>
{
    var contexto = new DataContext();
    contexto.InitializeSeedData();
    return contexto;
});
builder.Services.AddSwaggerGen();

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


app.UseAuthorization();

app.MapControllers();

app.Run();