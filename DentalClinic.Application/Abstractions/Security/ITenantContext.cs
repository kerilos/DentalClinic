namespace DentalClinic.Application.Abstractions.Security;

public interface ITenantContext
{
    Guid? ClinicId { get; }
    bool HasTenant { get; }
}