using HotelApp.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace HotelApp.Api.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {        
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var traceId =
                Activity.Current?.TraceId.ToString()
                ?? httpContext.TraceIdentifier;

            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                ArgumentException => StatusCodes.Status400BadRequest,
                ForbiddenException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;


            httpContext.Response.ContentType = "application/json";

            if (statusCode >= 500)
            {
                _logger.LogError(
                    exception,
                    "Erro inesperado em {Metodo} {Caminho}. Status: {StatusCode}. TraceId: {TraceId}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    statusCode,
                    traceId);
            }
            else
            {
                _logger.LogWarning(
                    "Requisicao rejeitada em {Metodo} {Caminho}. Status: {StatusCode}. Tipo: {TipoErro}. TraceId: {TraceId}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    statusCode,
                    exception.GetType().Name,
                    traceId);
            }

            var message = statusCode >= 500
            ? "Ocorreu um erro interno. Informe o TraceId ao suporte."
            : exception.Message;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    message,
                    traceId 
                },
            cancellationToken);


            return true;
        }
    }

    
}
