using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public sealed class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto>
{
    private readonly IAppDbContext _dbContext;

    public GetAppointmentByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentDto> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.GetAppointmentByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            throw new NotFoundException("Appointment not found.");
        }

        return appointment.ToDto();
    }
}
