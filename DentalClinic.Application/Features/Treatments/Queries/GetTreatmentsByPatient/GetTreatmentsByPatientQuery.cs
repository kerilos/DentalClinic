using DentalClinic.Application.Features.Treatments.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Queries.GetTreatmentsByPatient;

public sealed record GetTreatmentsByPatientQuery(Guid PatientId) : IRequest<IReadOnlyCollection<TreatmentDto>>;
