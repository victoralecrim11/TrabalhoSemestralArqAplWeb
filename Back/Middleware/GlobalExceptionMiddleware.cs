using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Back.Middleware
{
    // Middleware responsavel por capturar excecoes nao tratadas na aplicacao.
    // Ele evita que a API retorne uma tela/erro bruto e padroniza a resposta em JSON.
    public class GlobalExceptionMiddleware
    {
        // Representa o proximo middleware no pipeline da aplicacao.
        // Exemplo: autenticacao, autorizacao, controllers, etc.
        private readonly RequestDelegate _next;

        // RequestDelegate injetado automaticamente ao criar o middleware.
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Metodo executado a cada requisicao HTTP que passa por este middleware.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Tenta continuar o fluxo normal da requisicao.
                // Se nenhum erro acontecer nos proximos middlewares/controllers, a requisicao segue normalmente.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Se qualquer excecao nao tratada acontecer, a execucao cai neste bloco.
                // Define que a resposta sera enviada no formato JSON.
                context.Response.ContentType = "application/json";

                // Define o status HTTP 500, indicando erro interno do servidor.
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Cria o objeto que sera retornado no corpo da resposta.
                // error: codigo simples para identificar o tipo do erro.
                // message: mensagem da excecao capturada.
                var payload = new
                {
                    error = "internal_error",
                    message = ex.Message
                };

                // Converte o objeto de erro para JSON.
                var json = JsonSerializer.Serialize(payload);

                // Escreve o JSON na resposta HTTP enviada ao cliente.
                await context.Response.WriteAsync(json);
            }
        }
    }

    // Classe de extensao para registrar o middleware de forma mais limpa no Program.cs.
    public static class GlobalExceptionMiddlewareExtensions
    {
        // Permite usar: app.UseGlobalExceptionMiddleware();
        // Internamente, registra o GlobalExceptionMiddleware no pipeline.
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
