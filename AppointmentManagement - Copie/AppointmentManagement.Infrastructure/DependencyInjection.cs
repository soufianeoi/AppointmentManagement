using AppointmentManagement.Application.Abstractions;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Infrastructure.Persistence;
using AppointmentManagement.Infrastructure.Repositories;
using AppointmentManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppointmentDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}