using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public sealed class Treatment : BaseEntity
{
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? InvoiceId { get; set; }
    public int ToothNumber { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Cost { get; set; }
    public DateTime TreatmentDate { get; set; }
}
