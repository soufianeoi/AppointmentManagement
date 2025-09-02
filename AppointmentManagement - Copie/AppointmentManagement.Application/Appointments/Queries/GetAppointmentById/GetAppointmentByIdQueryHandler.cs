using AppointmentManagement.Application.Appointments.Dtos;
using AppointmentManagement.Application.Appointments.Mapping;
using AppointmentManagement.Application.Common;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.ValueObjects;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(new AppointmentId(request.AppointmentId), cancellationToken);
            if (appointment == null)
                return Result.Failure<AppointmentDto>("Appointment not found");

            return Result.Success(appointment.ToDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"An error occurred while retrieving the appointment: {ex.Message}");
        }
    }
}