using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido: {Message}", ex.Message);
                await EscreverRespostaAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operação inválida: {Message}", ex.Message);
                await EscreverRespostaAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado: {Message}", ex.Message);
                await EscreverRespostaAsync(context, StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.");
            }
        }

        private static async Task EscreverRespostaAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = ResolverTitulo(statusCode),
                Detail = message,
                Instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static string ResolverTitulo(int statusCode) => statusCode switch
        {
            400 => "Requisição inválida",
            500 => "Erro interno do servidor",
            _   => "Erro"
        };
    }
}
