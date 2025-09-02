using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand(Guid AppointmentId) : IRequest<Result>;