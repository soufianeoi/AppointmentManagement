using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.ConfirmAppointment;

public record ConfirmAppointmentCommand(Guid AppointmentId) : IRequest<Result>;