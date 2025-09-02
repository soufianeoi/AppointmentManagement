using AppointmentManagement.Application.Abstractions;
using AppointmentManagement.Application.Appointments.Dtos;
using AppointmentManagement.Application.Common;
using AppointmentManagement.Domain.Enums;
using MediatR;

namespace AppointmentManagement.Application.Appointments.Queries.ListAppointments;

public record ListAppointmentsQuery(
    int Page = 1,
    int PageSize = 10,
    string? DoctorName = null,
    string? PatientName = null,
    AppointmentStatus? Status = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IRequest<Result<IPagedList<AppointmentDto>>>;