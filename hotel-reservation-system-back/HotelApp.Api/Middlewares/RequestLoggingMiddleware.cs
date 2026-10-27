using System.Diagnostics;

namespace HotelApp.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId =
                Activity.Current?.TraceId.ToString()
                ?? context.TraceIdentifier;

            var metodo = context.Request.Method
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
            var caminho = context.Request.Path.Value?
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty) ?? string.Empty;

            using var scope = _logger.BeginScope("TraceId: {TraceId}", traceId);

            var cronometro = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                cronometro.Stop();

                var logLevel = context.Response.StatusCode switch
                {
                    >= StatusCodes.Status500InternalServerError => LogLevel.Error,
                    >= StatusCodes.Status400BadRequest => LogLevel.Warning,
                    _ => LogLevel.Information
                };

                _logger.Log
                    (
                     logLevel,
                     "Requisicao concluida em {Metodo} {Caminho}. Status: {StatusCode}. Duracao: {DuracaoMs} ms. TraceId: {TraceId}",
                     metodo,
                     caminho,
                     context.Response.StatusCode, 
                     cronometro.ElapsedMilliseconds, 
                     traceId
                    );
            }

        }
    }
}
