using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.MarkNoShow;

public record MarkNoShowCommand(Guid AppointmentId) : IRequest<Result>;