using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid AppointmentId) : IRequest<Result>;