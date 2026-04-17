using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.DeleteTreatment;

public sealed class DeleteTreatmentCommandHandler : IRequestHandler<DeleteTreatmentCommand, Unit>
{
    private readonly IAppDbContext _dbContext;

    public DeleteTreatmentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteTreatmentCommand request, CancellationToken cancellationToken)
    {
        var treatment = await _dbContext.GetTreatmentForUpdateByIdAsync(request.Id, cancellationToken);
        if (treatment is null)
        {
            throw new NotFoundException("Treatment not found.");
        }

        _dbContext.RemoveTreatment(treatment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
