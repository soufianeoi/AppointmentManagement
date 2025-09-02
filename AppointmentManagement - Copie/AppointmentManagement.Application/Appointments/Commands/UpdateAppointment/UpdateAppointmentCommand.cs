using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.UpdateAppointment;

public record UpdateAppointmentCommand(
    Guid AppointmentId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate) : IRequest<Result>;