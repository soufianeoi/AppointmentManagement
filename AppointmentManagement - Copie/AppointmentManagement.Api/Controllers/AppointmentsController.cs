using AppointmentManagement.Application.Appointments.Commands.CancelAppointment;
using AppointmentManagement.Application.Appointments.Commands.CompleteAppointment;
using AppointmentManagement.Application.Appointments.Commands.ConfirmAppointment;
using AppointmentManagement.Application.Appointments.Commands.CreateAppointment;
using AppointmentManagement.Application.Appointments.Commands.MarkNoShow;
using AppointmentManagement.Application.Appointments.Commands.StartAppointment;
using AppointmentManagement.Application.Appointments.Commands.UpdateAppointment;
using AppointmentManagement.Application.Appointments.Queries.GetAppointmentById;
using AppointmentManagement.Application.Appointments.Queries.ListAppointments;
using AppointmentManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? doctorName = null,
        [FromQuery] string? patientName = null,
        [FromQuery] AppointmentStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListAppointmentsQuery(page, pageSize, doctorName, patientName, status, startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAppointment(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetAppointmentByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAppointmentCommand(
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.PatientName,
            request.PatientEmail,
            request.PatientPhone,
            request.DoctorName);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetAppointment), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAppointment(
        Guid id,
        [FromBody] UpdateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAppointmentCommand(
            id,
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new ConfirmAppointmentCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartAppointment(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new StartAppointmentCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteAppointment(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new CompleteAppointmentCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new CancelAppointmentCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/mark-no-show")]
    public async Task<IActionResult> MarkNoShow(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new MarkNoShowCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}

public record CreateAppointmentRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string PatientName,
    string PatientEmail,
    string PatientPhone,
    string DoctorName);

public record UpdateAppointmentRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate);