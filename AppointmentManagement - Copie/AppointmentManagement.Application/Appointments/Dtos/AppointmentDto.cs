using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Application.Appointments.Dtos;

public record AppointmentDto(
    Guid Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string PatientName,
    string PatientEmail,
    string PatientPhone,
    string DoctorName,
    AppointmentStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);