using SwaggerThemes;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddSwaggerGen(); 

builder.Services.AddControllers(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

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
