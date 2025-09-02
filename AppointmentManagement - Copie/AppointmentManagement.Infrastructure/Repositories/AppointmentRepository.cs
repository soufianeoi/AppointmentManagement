using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;
using AppointmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagement.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentDbContext _context;

    public AppointmentRepository(AppointmentDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(AppointmentId id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .OrderBy(a => a.DateRange.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .OrderBy(a => a.DateRange.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByDoctorAsync(string doctorName, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.DoctorName.Contains(doctorName))
            .OrderBy(a => a.DateRange.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientAsync(string patientName, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.PatientName.Contains(patientName))
            .OrderBy(a => a.DateRange.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.DateRange.StartDate >= startDate && a.DateRange.EndDate <= endDate)
            .OrderBy(a => a.DateRange.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await _context.Appointments.AddAsync(appointment, cancellationToken);
    }

    public void Update(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
    }

    public void Remove(Appointment appointment)
    {
        _context.Appointments.Remove(appointment);
    }
}