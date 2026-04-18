using DentalClinic.Domain.Entities;
using DentalClinic.Application.Common.Models;
using System.Data;

namespace DentalClinic.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    Task<Clinic?> GetClinicByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Clinic?> GetClinicByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddClinicAsync(Clinic clinic, CancellationToken cancellationToken = default);

    Task<User?> GetUserByEmailAsync(Guid clinicId, string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdInClinicAsync(Guid clinicId, Guid userId, CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenForUpdateAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddPatientAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Patient>> GetPatientsAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<Appointment?> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Appointment?> GetAppointmentForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Appointment>> GetAppointmentsAsync(
        DateTime? from,
        DateTime? to,
        Guid? doctorId,
        Guid? patientId,
        CancellationToken cancellationToken = default);
    Task<bool> HasDoctorOverlappingAppointmentAsync(
        Guid doctorId,
        DateTime appointmentDate,
        int durationInMinutes,
        Guid? excludeAppointmentId,
        CancellationToken cancellationToken = default);

    Task AddTreatmentAsync(Treatment treatment, CancellationToken cancellationToken = default);
    Task<Treatment?> GetTreatmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Treatment?> GetTreatmentForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Treatment>> GetTreatmentsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Treatment>> GetTreatmentsForInvoiceAsync(Guid patientId, IReadOnlyCollection<Guid> treatmentIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Guid>> GetTreatmentIdsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    void RemoveTreatment(Treatment treatment);

    Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Invoice>> GetInvoicesByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<bool> PaymentRequestExistsAsync(Guid invoiceId, string requestId, CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    Task AcquireDoctorScheduleLockAsync(Guid doctorId, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
