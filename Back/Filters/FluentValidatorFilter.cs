using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Back.Filters
{
    // Filtro executado antes da action do controller.
    // Ele usa os validators do FluentValidation para validar os DTOs recebidos pela API.
    public class FluentValidatorFilter : IAsyncActionFilter
    {
        // IServiceProvider permite buscar servicos registrados no Program.cs.
        // Aqui ele sera usado para encontrar o validator correto de cada DTO.
        private readonly IServiceProvider _serviceProvider;

        // O ASP.NET injeta automaticamente o IServiceProvider quando cria o filtro.
        public FluentValidatorFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Metodo chamado automaticamente pelo ASP.NET antes da action do controller executar.
        // context contem os argumentos recebidos pela action.
        // next representa a proxima etapa do pipeline, ou seja, a execucao real da action.
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Dicionario que vai guardar os erros encontrados.
            // A chave sera o nome do campo, e o valor sera uma lista de mensagens de erro.
            var errors = new Dictionary<string, string[]>();

            // Percorre todos os argumentos recebidos pela action.
            // Exemplo: um DTO enviado no body da requisicao.
            foreach (var argument in context.ActionArguments.Values)
            {
                // Se algum argumento vier nulo, nao ha o que validar.
                if (argument is null)
                    continue;

                // Tenta encontrar um validator para o tipo do argumento atual.
                // Exemplo: se o argumento for RegistroDto, procura por IValidator<RegistroDto>.
                var validator = GetValidator(argument);

                // Se nao existir validator registrado para esse DTO, segue para o proximo argumento.
                if (validator is null)
                    continue;

                // Executa a validacao usando o validator encontrado.
                var result = await ValidateAsync(validator, argument);

                // Adiciona os erros encontrados no dicionario final.
                AddErrors(errors, result);
            }

            // Se houver pelo menos um erro de validacao, a action nao sera executada.
            if (errors.Count > 0)
            {
                // Define a resposta HTTP como 400 Bad Request.
                // O corpo da resposta contem uma mensagem geral e os erros separados por campo.
                context.Result = new BadRequestObjectResult(new
                {
                    mensagem = "Dados inválidos",
                    erros = errors
                });

                // Interrompe o pipeline para impedir que o controller continue.
                return;
            }

            // Se nao houve erro, permite que a action do controller execute normalmente.
            await next();
        }

        // Busca no container de injecao de dependencia um validator compativel com o objeto recebido.
        private object? GetValidator(object argument)
        {
            // Monta dinamicamente o tipo IValidator<T> usando o tipo real do argumento.
            // Exemplo: typeof(IValidator<RegistroDto>).
            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            // Retorna o validator registrado, ou null se nenhum validator existir para esse tipo.
            return _serviceProvider.GetService(validatorType);
        }

        // Executa o ValidateAsync do FluentValidation.
        // O uso de dynamic deixa o codigo mais simples aqui, pois o tipo T so e conhecido em tempo de execucao.
        private static async Task<ValidationResult> ValidateAsync(
            dynamic validator,
            dynamic argument)
        {
            // Retorna o resultado da validacao, contendo a lista de erros encontrados.
            return await validator.ValidateAsync(argument);
        }

        // Copia os erros do FluentValidation para o dicionario que sera enviado na resposta da API.
        private static void AddErrors(
            Dictionary<string, string[]> errors,
            ValidationResult result)
        {
            // Agrupa os erros pelo nome da propriedade.
            // Assim, todas as mensagens do mesmo campo ficam juntas.
            foreach (var group in result.Errors.GroupBy(error => error.PropertyName))
            {
                // Salva no dicionario:
                // chave = nome do campo
                // valor = mensagens de erro daquele campo
                errors[group.Key] = group
                    .Select(error => error.ErrorMessage)
                    .ToArray();
            }
        }
    }
}
