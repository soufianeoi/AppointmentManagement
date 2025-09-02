using AppointmentManagement.Application.Abstractions;
using AppointmentManagement.Application.Appointments.Dtos;
using AppointmentManagement.Application.Appointments.Mapping;
using AppointmentManagement.Application.Common;
using AppointmentManagement.Domain.Abstractions;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Queries.ListAppointments;

public class ListAppointmentsQueryHandler : IRequestHandler<ListAppointmentsQuery, Result<IPagedList<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ListAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<IPagedList<AppointmentDto>>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
            
            var filteredAppointments = appointments.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.DoctorName))
            {
                filteredAppointments = filteredAppointments.Where(a => 
                    a.DoctorName.Contains(request.DoctorName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.PatientName))
            {
                filteredAppointments = filteredAppointments.Where(a => 
                    a.PatientName.Contains(request.PatientName, StringComparison.OrdinalIgnoreCase));
            }

            if (request.Status.HasValue)
            {
                filteredAppointments = filteredAppointments.Where(a => a.Status == request.Status.Value);
            }

            if (request.StartDate.HasValue)
            {
                filteredAppointments = filteredAppointments.Where(a => a.DateRange.StartDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                filteredAppointments = filteredAppointments.Where(a => a.DateRange.EndDate <= request.EndDate.Value);
            }

            var appointmentDtos = filteredAppointments.Select(a => a.ToDto()).ToList();
            
            var pagedList = new PagedList<AppointmentDto>(
                appointmentDtos,
                request.Page,
                request.PageSize,
                appointmentDtos.Count);

            return Result.Success<IPagedList<AppointmentDto>>(pagedList);
        }
        catch (Exception ex)
        {
            return Result.Failure<IPagedList<AppointmentDto>>($"An error occurred while retrieving appointments: {ex.Message}");
        }
    }
}