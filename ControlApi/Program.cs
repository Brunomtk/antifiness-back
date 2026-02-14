using System.Text;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using ControlApi.Middleware;
using Infrastructure;
using Infrastructure.Authenticate;
using Infrastructure.Repositories;
using Infrastructure.ServiceExtension;
using Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// SERILOG CONFIGURATION
// ---------------------------------------------------------
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .Enrich.WithExceptionDetails()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

// ---------------------------------------------------------
// SERVICES & SWAGGER
// ---------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AntiFitness.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                },
                Scheme = "oauth2",
                Name   = "Bearer",
                In     = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// ---------------------------------------------------------
// DEPENDENCY INJECTION
// ---------------------------------------------------------
builder.Services.AddDIServices(builder.Configuration);

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmpresasService, EmpresasService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IDietService, DietService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// ---------------------------------------------------------
// CORS POLICY (Local + Produção)
// ---------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "https://antifitness.com.br",
                "https://www.antifitness.com.br",
                "https://api.antifitness.com.br"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// ---------------------------------------------------------
// FORWARDED HEADERS (atrás de Nginx)
// ---------------------------------------------------------
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("209.97.156.138"));
    options.RequireHeaderSymmetry = false;
});

// ---------------------------------------------------------
// JWT Authentication
// ---------------------------------------------------------
var cfg = builder.Configuration;
var key = Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Jwt:Issuer"],
            ValidAudience = cfg["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                // Valida revogação em massa via TokenVersion (claim "tv").
                var userIdStr = context.Principal?.FindFirst("userId")?.Value
                               ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? context.Principal?.FindFirst("sub")?.Value;
                var tvStr = context.Principal?.FindFirst("tv")?.Value;

                if (!int.TryParse(userIdStr, out var userId) || !int.TryParse(tvStr, out var tokenVersion))
                {
                    context.Fail("Invalid token claims.");
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<DbContextClass>();
                var currentTv = await db.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => u.TokenVersion)
                    .FirstOrDefaultAsync();

                if (currentTv != tokenVersion)
                {
                    context.Fail("Token revoked.");
                }
            }
        };

    });

// ---------------------------------------------------------
// APP BUILD
// ---------------------------------------------------------
var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// EF Core timestamp behavior
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Deve vir cedo no pipeline (antes de autenticação/autorização)
app.UseForwardedHeaders();

// ---------------------------------------------------------
// CORS Middleware Manual (resolve bloqueios via proxy)
// ---------------------------------------------------------
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, PATCH, DELETE, OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Authorization, Origin, X-Requested-With, Content-Type, Accept";
    }

    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204;
        await context.Response.CompleteAsync();
        return;
    }

    await next();
});

// ---------------------------------------------------------
// RESTANTE DO PIPELINE
// ---------------------------------------------------------
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 400)
            return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Aplica migrações automáticas
app.MigrateDatabase();

// Controllers
app.MapControllers();

// Run
app.Run();
