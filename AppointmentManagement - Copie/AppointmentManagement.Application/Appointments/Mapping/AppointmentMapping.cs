using AppointmentManagement.Application.Appointments.Dtos;
using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Application.Appointments.Mapping;

public static class AppointmentMapping
{
    public static AppointmentDto ToDto(this Appointment appointment)
    {
        return new AppointmentDto(
            appointment.Id.Value,
            appointment.Title,
            appointment.Description,
            appointment.DateRange.StartDate,
            appointment.DateRange.EndDate,
            appointment.PatientName,
            appointment.PatientEmail,
            appointment.PatientPhone,
            appointment.DoctorName,
            appointment.Status,
            appointment.CreatedAt,
            appointment.UpdatedAt);
    }
}