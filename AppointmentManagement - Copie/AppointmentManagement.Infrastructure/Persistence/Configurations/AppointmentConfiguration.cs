using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentManagement.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(
                appointmentId => appointmentId.Value,
                value => new AppointmentId(value));

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.OwnsOne(a => a.DateRange, dateRange =>
        {
            dateRange.Property(d => d.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            dateRange.Property(d => d.EndDate)
                .HasColumnName("EndDate")
                .IsRequired();
        });

        builder.Property(a => a.PatientName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.PatientEmail)
            .HasMaxLength(100);

        builder.Property(a => a.PatientPhone)
            .HasMaxLength(20);

        builder.Property(a => a.DoctorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

    }
}