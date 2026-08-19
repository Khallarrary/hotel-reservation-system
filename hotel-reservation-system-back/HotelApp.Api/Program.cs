using HotelApp.Api.Contextos;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;
using HotelApp.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

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

builder.Services.AddScoped<IContaReservaRepository,
ContaReservaRepository>();

builder.Services.AddScoped<ILancamentoContaRepository,
LancamentoContaRepository>();

builder.Services.AddScoped<IConsultaSaldoConta, ConsultaSaldoConta>();

builder.Services.AddScoped<CaixaService>();

builder.Services.AddScoped<IQuartoRepository, QuartoRepository>();
builder.Services.AddScoped<QuartoService>();

builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<HotelService>();


builder.Services.AddScoped<IUsuarioRepository, 
UsuarioRepository>();
builder.Services.AddScoped<ISenhaHasher, SenhaHasher>();
builder.Services.AddScoped<UsuarioService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHotelContexto, HotelContexto>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioContexto, UsuarioContexto>();

builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddScoped<ITransacao, Transacao>();

builder.Services.AddSingleton<IRelogioHotel, RelogioHotel>();



/// <summary>
/// Adiciona suporte a controllers (API)
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Configuração do Swagger (documentação da API)
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var chave = builder.Configuration["Jwt:Key"];

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(chave!))


        };
    }
    );

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
            ForbiddenException => StatusCodes.Status403Forbidden,
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

app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

/// <summary>
/// Mapeamento dos endpoints dos controllers
/// </summary>
app.MapControllers();

app.Run();