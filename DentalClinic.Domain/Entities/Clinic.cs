using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public sealed class Clinic : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}