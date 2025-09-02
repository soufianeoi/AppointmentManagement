using AppointmentManagement.Application.Common;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.ValueObjects;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Commands.MarkNoShow;

public class MarkNoShowCommandHandler : IRequestHandler<MarkNoShowCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNoShowCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(new AppointmentId(request.AppointmentId), cancellationToken);
            if (appointment == null)
                return Result.Failure("Appointment not found");

            appointment.MarkNoShow();
            
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
            return Result.Failure($"An error occurred while marking appointment as no-show: {ex.Message}");
        }
    }
}