using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid ClinicId { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}