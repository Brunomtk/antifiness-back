using ControlApi.Middleware;
using Infrastructure.Authenticate;
using Infrastructure.Repositories;
using Infrastructure.ServiceExtension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Services;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// DI: Repositórios/Serviços + Db (feito dentro de AddDIServices)
// -----------------------------
builder.Services.AddDIServices(builder.Configuration);

// 🔹 Serviços (do primeiro código que você enviou)
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
builder.Services.AddSingleton<IJWTManager, JWTManager>();

// -----------------------------
// JWT
// -----------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

// -----------------------------
// Controllers / JSON
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

// -----------------------------
// Swagger (com JWT)
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Control.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization headers usando o esquema Bearer. Ex: \"Bearer {token}\"",
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// -----------------------------
// Serilog
// -----------------------------
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

// -----------------------------
// CORS (permitindo apenas origens específicas)
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;

                // Dev
                if (origin.Equals("http://localhost:3000", StringComparison.OrdinalIgnoreCase)) return true;
                if (origin.Equals("http://localhost:3001", StringComparison.OrdinalIgnoreCase)) return true;

                // Produção (Front)
                if (origin.Equals("https://antifitnessapp.vercel.app", StringComparison.OrdinalIgnoreCase)) return true;

                // IP público
                if (origin.Equals("http://209.97.156.138", StringComparison.OrdinalIgnoreCase)) return true;
                if (origin.Equals("https://209.97.156.138", StringComparison.OrdinalIgnoreCase)) return true;

                return false;
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// -----------------------------
// Forwarded Headers (proxy Nginx / LB)
// -----------------------------
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Proxy/LB conhecido (IP público do seu Nginx/LB)
    options.KnownProxies.Add(IPAddress.Parse("209.97.149.15")); // ajuste se necessário

    options.RequireHeaderSymmetry = false;
    // options.ForwardLimit = 2; // use se tiver mais de um proxy
});

var app = builder.Build();

// -----------------------------
// Pipeline
// -----------------------------
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseForwardedHeaders();

app.UseCors("AllowSpecificOrigins");
app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode > 499) return LogEventLevel.Error;
        if (httpContext.Response.StatusCode > 399) return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MigrateDatabase();
app.MapControllers();
app.Run();
