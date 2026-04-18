using DentalClinic.Application.Abstractions.Security;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http;

namespace DentalClinic.Infrastructure.Security;

public sealed class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? ClinicId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var claimValue = user.FindFirst("clinic_id")?.Value;
            if (Guid.TryParse(claimValue, out var clinicId))
            {
                return clinicId;
            }

            throw new AuthenticationException("Missing or invalid clinic context.");
        }
    }

    public bool HasTenant
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var claimValue = user.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(claimValue, out _);
        }
    }
}
