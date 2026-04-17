using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Infrastructure.Authentication;
using DentalClinic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DentalClinic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = new JwtOptions
        {
            SecretKey = jwtSection[nameof(JwtOptions.SecretKey)] ?? throw new InvalidOperationException("Jwt:SecretKey is not configured."),
            Issuer = jwtSection[nameof(JwtOptions.Issuer)] ?? throw new InvalidOperationException("Jwt:Issuer is not configured."),
            Audience = jwtSection[nameof(JwtOptions.Audience)] ?? throw new InvalidOperationException("Jwt:Audience is not configured."),
            ExpirationMinutes = int.TryParse(jwtSection[nameof(JwtOptions.ExpirationMinutes)], out var expirationMinutes)
                ? expirationMinutes
                : throw new InvalidOperationException("Jwt:ExpirationMinutes is not configured.")
        };

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey) ||
            string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
            string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
            jwtOptions.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt settings must be fully configured.");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddSingleton<IOptions<JwtOptions>>(Options.Create(jwtOptions));
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}