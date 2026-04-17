using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.CancelAppointment;

public sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Unit>
{
    private readonly IAppDbContext _dbContext;

    public CancelAppointmentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.GetAppointmentForUpdateByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            throw new NotFoundException("Appointment not found.");
        }

        appointment.Status = AppointmentStatus.Cancelled;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
