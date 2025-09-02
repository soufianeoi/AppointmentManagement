using AppointmentManagement.Application.Appointments.Dtos;
using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;