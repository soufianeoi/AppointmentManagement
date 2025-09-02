using AppointmentManagement.Application.Common;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.ValueObjects;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.StartAppointment;

public class StartAppointmentCommandHandler : IRequestHandler<StartAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(StartAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(new AppointmentId(request.AppointmentId), cancellationToken);
            if (appointment == null)
                return Result.Failure("Appointment not found");

            appointment.MarkInProgress();
            
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while starting the appointment: {ex.Message}");
        }
    }
}