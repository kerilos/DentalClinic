using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Infrastructure.Authentication;
using DentalClinic.Infrastructure.Persistence;
using DentalClinic.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DentalClinic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = new JwtOptions
        {
            SecretKey = jwtSection[nameof(JwtOptions.SecretKey)] ?? string.Empty,
            Issuer = jwtSection[nameof(JwtOptions.Issuer)] ?? string.Empty,
            Audience = jwtSection[nameof(JwtOptions.Audience)] ?? string.Empty,
            ExpirationMinutes = int.TryParse(jwtSection[nameof(JwtOptions.ExpirationMinutes)], out var expirationMinutes)
                ? expirationMinutes
                : 60
        };

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey) ||
            string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
            string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
            jwtOptions.ExpirationMinutes <= 0)
        {
            if (!environment.IsDevelopment())
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

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddSingleton<IOptions<JwtOptions>>(Options.Create(jwtOptions));
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}