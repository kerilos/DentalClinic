using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.DeletePatient;

public sealed class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Unit>
{
    private readonly IAppDbContext _dbContext;

    public DeletePatientCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientForUpdateByIdAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        patient.IsDeleted = true;
        patient.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
