using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public sealed class Patient : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? MedicalHistory { get; set; }
}