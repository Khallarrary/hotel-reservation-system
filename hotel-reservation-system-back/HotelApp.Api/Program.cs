using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração do DbContext com PostgreSQL
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Injeção de dependência (DI)
/// Repositórios e Services
/// </summary>
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<ReservaService>();

builder.Services.AddScoped<IQuartoRepository, QuartoRepository>();
builder.Services.AddScoped<QuartoService>();

/// <summary>
/// Adiciona suporte a controllers (API)
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Configuração do Swagger (documentação da API)
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

/// <summary>
/// Middleware global para tratamento de exceções
/// Centraliza erros e retorna respostas padronizadas para o cliente
/// </summary>
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.ContentType = "application/json";

        // Mapeamento de exceções para status HTTP
        context.Response.StatusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        // Retorno padrão de erro
        await context.Response.WriteAsJsonAsync(new
        {
            message = exception?.Message
        });
    });
});

/// <summary>
/// Swagger disponível apenas em ambiente de desenvolvimento
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Redireciona a rota raiz para o Swagger
/// </summary>
app.MapGet("/", () => Results.Redirect("/swagger"));

/// <summary>
/// Middleware padrão do pipeline HTTP
/// </summary>
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

/// <summary>
/// Mapeamento dos endpoints dos controllers
/// </summary>
app.MapControllers();

app.Run();