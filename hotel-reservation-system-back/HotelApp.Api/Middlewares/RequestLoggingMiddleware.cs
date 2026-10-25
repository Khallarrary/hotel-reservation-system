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
            using var scope = _logger.BeginScope("TraceId: {TraceId}", context.TraceIdentifier);

            var cronometro = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                cronometro.Stop();

                _logger.LogInformation
                    (
                     "Requisicao concluida em {Metodo} {Caminho}. Status: {StatusCode}. Duracao: {DuracaoMs} ms. TraceId: {TraceId}",
                     context.Request.Method, 
                     context.Request.Path, 
                     context.Response.StatusCode, 
                     cronometro.ElapsedMilliseconds, 
                     context.TraceIdentifier
                    );
            }

        }
    }
}
