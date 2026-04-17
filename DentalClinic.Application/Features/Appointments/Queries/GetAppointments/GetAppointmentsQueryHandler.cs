using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, IReadOnlyCollection<AppointmentDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetAppointmentsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _dbContext.GetAppointmentsAsync(
            request.From,
            request.To,
            request.DoctorId,
            request.PatientId,
            cancellationToken);

        return appointments
            .Select(appointment => appointment.ToDto())
            .ToArray();
    }
}
