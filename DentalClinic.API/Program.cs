using System.Text;
using DentalClinic.API.Middleware;
using DentalClinic.Application;
using DentalClinic.Infrastructure;
using DentalClinic.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    Log.Information("Starting DentalClinic API host");

    builder.Host.UseSerilog((context, _, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.AddPolicy("AuthPolicy", context =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ip,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
        });
    });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

    var jwtSection = builder.Configuration.GetSection("Jwt");
    var jwtOptions = jwtSection.Get<JwtOptions>();
    if (jwtOptions is null ||
        string.IsNullOrWhiteSpace(jwtOptions.SecretKey) ||
        string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
        string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
        jwtOptions.ExpirationMinutes <= 0)
    {
        if (!builder.Environment.IsDevelopment())
        {
            throw new InvalidOperationException("Jwt settings must be fully configured.");
        }

        jwtOptions = new JwtOptions
        {
            SecretKey = "DevelopmentOnly-Local-Secret-Key-Change-In-Production-1234567890",
            Issuer = "DentalClinic.API",
            Audience = "DentalClinic.Client",
            ExpirationMinutes = 60
        };
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("ClinicalStaff", policy => policy.RequireRole("Admin", "Doctor", "Receptionist"));
        options.AddPolicy("DoctorOrAdmin", policy => policy.RequireRole("Admin", "Doctor"));
        options.AddPolicy("BillingStaff", policy => policy.RequireRole("Admin", "Receptionist"));
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGet("/", () => Results.Ok(new
    {
        service = "DentalClinic.API",
        status = "running",
        health = "/health"
    }));
    app.MapHealthChecks("/health");
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "DentalClinic API terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
