using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries.GetPatients;

public sealed record GetPatientsQuery(int PageNumber = 1, int PageSize = 10, string? Search = null) : IRequest<PagedResult<PatientDto>>;
