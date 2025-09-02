using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Domain.Abstractions;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(AppointmentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByDoctorAsync(string doctorName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByPatientAsync(string patientName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    void Update(Appointment appointment);
    void Remove(Appointment appointment);
}