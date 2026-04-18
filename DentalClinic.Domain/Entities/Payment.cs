using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public Guid ClinicId { get; set; }
    public Guid InvoiceId { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Notes { get; set; }
}
