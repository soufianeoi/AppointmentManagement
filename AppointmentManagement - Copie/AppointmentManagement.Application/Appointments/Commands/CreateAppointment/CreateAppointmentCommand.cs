using AppointmentManagement.Application.Common;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.CreateAppointment;

public record CreateAppointmentCommand(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string PatientName,
    string PatientEmail,
    string PatientPhone,
    string DoctorName) : IRequest<Result<Guid>>;