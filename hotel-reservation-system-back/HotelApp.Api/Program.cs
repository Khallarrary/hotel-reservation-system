using HotelApp.Api.Contextos;
using HotelApp.Api.Handlers;
using HotelApp.Api.Middlewares;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Globalization;

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
            .AllowAnyMethod()
            .WithExposedHeaders("Retry-After"));
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


var demoHotelId =
    builder.Configuration.GetValue<int>("RateLimiting:DemoHotelId");

var demoHotelPermitLimit =
    builder.Configuration.GetValue<int>("RateLimiting:DemoHotelPermitLimit");

var demoHotelWindowSeconds =
    builder.Configuration.GetValue<int>("RateLimiting:DemoHotelWindowSeconds");

if (demoHotelPermitLimit <= 0)
{
    throw new InvalidOperationException(
        "RateLimiting:DemoHotelPermitLimit deve ser maior que zero.");
}

if (demoHotelWindowSeconds <= 0)
{
    throw new InvalidOperationException(
        "RateLimiting:DemoHotelWindowSeconds deve ser maior que zero.");
}

if (builder.Environment.IsProduction() && demoHotelId <= 0)
{
    throw new InvalidOperationException(
        "RateLimiting:DemoHotelId deve ser maior que zero em produção.");
}


builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
            {
                return RateLimitPartition.GetNoLimiter(
                    partitionKey: "health-check");
            }

            var hotelId =
                httpContext.User.FindFirst("hotelId")?.Value;

            var demoHotelIdTexto =
                demoHotelId.ToString(CultureInfo.InvariantCulture);

            if (demoHotelId <= 0 || hotelId != demoHotelIdTexto)
            {
                return RateLimitPartition.GetNoLimiter(
                    partitionKey: "hotel-sem-limite");
            }

            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: $"demo-hotel:{hotelId}",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = demoHotelPermitLimit,
                    Window = TimeSpan.FromSeconds(
                        demoHotelWindowSeconds),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst,
                    AutoReplenishment = true
                });
        });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {


        if (context.Lease.TryGetMetadata(
                MetadataName.RetryAfter,
                out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).
                ToString(CultureInfo.InvariantCulture);
        }

        var traceId = Activity.Current?.TraceId.ToString()
            ?? context.HttpContext.TraceIdentifier;

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                message = "Limite de requisicoes excedido. Tente novamente mais tarde.",
                traceId
            },
            cancellationToken);
    };

    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey:
                httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "ip-desconhecido",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            }
            )
        );
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
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter())
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

app.UseRouting();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
}).DisableHttpMetrics(); ;

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
}).DisableHttpMetrics(); ;
/// <summary>
/// Mapeamento dos endpoints dos controllers
/// </summary>
app.MapControllers();

app.Run();
