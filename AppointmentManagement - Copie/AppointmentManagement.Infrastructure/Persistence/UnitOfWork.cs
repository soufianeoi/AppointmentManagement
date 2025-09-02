using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Infrastructure.Persistence;

namespace AppointmentManagement.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppointmentDbContext _context;

    public UnitOfWork(AppointmentDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}