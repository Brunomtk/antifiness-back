using System.Text;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
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

// Serilog configuration
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

// Add services to the container.
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

// Add DI: DbContext, Repositories, UnitOfWork, Services, JWT Manager
builder.Services.AddDIServices(builder.Configuration);

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmpresasService, EmpresasService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IDietService, DietService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddSingleton<IJWTManager, JWTManager>();

// ------- CORS (produção + local) -------
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

// ------- Forwarded Headers (quando atrás de Nginx/Proxy) -------
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("209.97.156.138"));
    options.RequireHeaderSymmetry = false;
    // options.ForwardLimit = 2; // se houver mais de um proxy no caminho
});

// Authentication with JWT Bearer
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
    });

var app = builder.Build();

// Swagger UI em Dev e Prod (opcional ajustar)
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// EF Core timestamp behavior
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Deve vir cedo no pipeline (antes de autenticação/autorização)
app.UseForwardedHeaders();

// HTTPS redirection (mantenha se usa HTTPS direto; se Nginx termina TLS, ainda é ok)
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowSpecificOrigins");

// Serilog request logging
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

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Aplica migrações (extensão da sua infra)
app.MigrateDatabase();

// Map controllers
app.MapControllers();

app.Run();
