using System.Net;
using System.Text.Json;
using Neocare.Domain.Exceptions;

namespace Neocare.Infrastructure.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Um erro ocorreu",
            status = HttpStatusCode.InternalServerError,
            detail = exception.Message,
            instance = context.Request.Path
        };

        if (exception is DomainException domainEx)
        {
            context.Response.StatusCode = domainEx.StatusCode;
            var statusCodeString = domainEx.StatusCode switch
            {
                404 => "Recurso não encontrado",
                422 => "Erro de validação",
                _ => "Erro na requisição"
            };
            response = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = statusCodeString,
                status = (HttpStatusCode)domainEx.StatusCode,
                detail = exception.Message,
                instance = context.Request.Path
            };
        }
        else if (exception is InvalidOperationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "Requisição inválida",
                status = HttpStatusCode.BadRequest,
                detail = exception.Message,
                instance = context.Request.Path
            };
        }
        else if (exception is KeyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            response = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = "Recurso não encontrado",
                status = HttpStatusCode.NotFound,
                detail = exception.Message,
                instance = context.Request.Path
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}
