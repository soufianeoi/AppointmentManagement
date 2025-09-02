using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.StartAppointment;

public record StartAppointmentCommand(Guid AppointmentId) : IRequest<Result>;