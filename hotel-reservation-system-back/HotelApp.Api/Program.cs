using HotelApp.Api.Contextos;
using HotelApp.Api.Handlers;
using HotelApp.Api.Middlewares;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Domain;
using HotelApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// Configuração do DbContext com PostgreSQL
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        name: "postgresql",
        tags: ["ready"]);

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
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var traceId =
                Activity.Current?.TraceId.ToString()
                ?? context.HttpContext.TraceIdentifier;

            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            return new BadRequestObjectResult(new
            {
                message = "Dados de entrada invalidos.",
                traceId,
                errors
            });
        };
    });
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

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

var origensPermitidas = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

if (origensPermitidas.Length == 0)
{
    throw new InvalidOperationException("Configure ao menos uma origem permitida em Cors:AllowedOrigins.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend",
        policy => policy
            .WithOrigins(origensPermitidas)
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

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("HotelApp.Api")
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment.name"] = builder.Environment.EnvironmentName
        }))
    .WithLogging(logging => logging.AddOtlpExporter(), options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
        options.ParseStateValues = true;
    })
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation(options =>
    {
        options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
    })
        .AddNpgsql()
        .AddOtlpExporter()
    );

var app = builder.Build();


app.UseMiddleware<RequestLoggingMiddleware>();
/// <summary>
/// Middleware global para tratamento de exceções
/// Centraliza erros e retorna respostas padronizadas para o cliente
/// </summary>
app.UseExceptionHandler();
app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;
    var traceId =
        Activity.Current?.TraceId.ToString()
        ?? httpContext.TraceIdentifier;

    await httpContext.Response.WriteAsJsonAsync(
        new
        {
            message = "A requisicao nao pode ser concluida.",
            traceId
        },
        httpContext.RequestAborted);
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
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/health"),
    branch => branch.Use(async (context, next) =>
    {
        using var scope = SuppressInstrumentationScope.Begin();
        await next(context);
    }));

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health"),
    branch => branch.UseHttpsRedirection());

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});
/// <summary>
/// Mapeamento dos endpoints dos controllers
/// </summary>
app.MapControllers();

app.Run();
