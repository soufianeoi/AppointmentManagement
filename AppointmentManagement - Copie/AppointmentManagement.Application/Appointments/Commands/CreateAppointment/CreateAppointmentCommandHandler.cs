using AppointmentManagement.Application.Common;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dateRange = DateRange.Create(request.StartDate, request.EndDate);
            
            var appointment = Appointment.Create(
                request.Title,
                request.Description,
                dateRange,
                request.PatientName,
                request.PatientEmail,
                request.PatientPhone,
                request.DoctorName);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.Id.Value);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>($"An error occurred while creating the appointment: {ex.Message}");
        }
    }
}